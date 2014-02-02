//-----------------------------------------------------------------------------------
// <copyright file="<file>" company="Microsoft">
//        Copyright (c) Microsoft Corporation 2008. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Microsoft.Surface.Presentation.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Surface.Presentation;
using System.Windows.Input;

namespace MobileConnectSample.DragDrop
{
    public static class DragDropManager
    {
        /// <summary>
        /// List of all the dragging ScatterViewItems we are managing.
        /// </summary>
        private static readonly List<ScatterViewItem> draggingItems = new List<ScatterViewItem>();

        /// <summary>
        /// Record of who is claiming each cursor.
        /// </summary>
        private static readonly Dictionary<ScatterViewItem, FrameworkElement> cursorClaims = new Dictionary<ScatterViewItem, FrameworkElement>();

        /// <summary>
        /// Event bubbled up from the cursor when a drag-drop operation is beginning. Recipients can claim the cursor if they think they should receive it when it is dropped.
        /// </summary>
        public static readonly RoutedEvent DragDropBegin = EventManager.RegisterRoutedEvent("DragDropBegin", RoutingStrategy.Bubble, typeof(EventHandler<DragDropEventArgs>), typeof(DragDropManager));

        /// <summary>
        /// Event bubbled up from the cursor when it moves.  Recipients can claim the cursor if they think they should receive it when it is dropped.
        /// </summary>
        public static readonly RoutedEvent DragDropMoved = EventManager.RegisterRoutedEvent("DragDropMoved", RoutingStrategy.Bubble, typeof(EventHandler<DragDropEventArgs>), typeof(DragDropManager));

        /// <summary>
        /// Event bubbled up from the cursor when the claim on it changes.
        /// </summary>
        public static readonly RoutedEvent DragDropCursorClaimChanged = EventManager.RegisterRoutedEvent("DragDropCursorClaimChanged", RoutingStrategy.Bubble, typeof(EventHandler<DragDropCursorClaimChangedEventArgs>), typeof(DragDropManager));

        /// <summary>
        /// Event bubbled up from the cursor when it is released by the user.  Whoever has the claim on it should handle the drop operstion.
        /// </summary>
        public static readonly RoutedEvent DragDropEnded = EventManager.RegisterRoutedEvent("DragDropEnded", RoutingStrategy.Bubble, typeof(EventHandler<DragDropEventArgs>), typeof(DragDropManager));

        /// <summary>
        /// Attached DependencyProperty that is set on the drag drop cursor to true
        /// when it is claimed by a drop target.
        /// </summary>
        public static readonly DependencyProperty CursorIsClaimedProperty =
            DependencyProperty.RegisterAttached("CursorIsClaimed", typeof(bool), typeof(DragDropManager), new UIPropertyMetadata(false));

        public static bool GetCursorIsClaimed(DependencyObject obj)
        {
            return (bool)obj.GetValue(CursorIsClaimedProperty);
        }
        public static void SetCursorIsClaimed(DependencyObject obj, bool value)
        {
            if (value)
            {
                obj.SetValue(CursorIsClaimedProperty, true);
            }
            else
            {
                obj.ClearValue(CursorIsClaimedProperty);
            }
        }

        public static readonly DependencyProperty DragDropSourceProperty =
            DependencyProperty.RegisterAttached("DragDropSource", typeof(object), typeof(DragDropManager), new UIPropertyMetadata(null));
        /// <summary>
        /// Set on a cursor.  The source of the drag drop operation.  
        /// It is up to the source to say what this is.
        /// This is used by drop targets to determine if
        /// they should accept things from this source.
        /// </summary>
        public static object GetDragDropSource(DependencyObject obj)
        {
            return obj.GetValue(DragDropSourceProperty);
        }
        public static void SetDragDropSource(DependencyObject obj, object value)
        {
            obj.SetValue(DragDropSourceProperty, value);
        }

        public static readonly DependencyProperty IsTransientCursorProperty =
            DependencyProperty.RegisterAttached("IsTransientCursor", typeof(bool), typeof(DragDropManager), new UIPropertyMetadata(false));
        /// <summary>
        /// Set on a cursor.  If the cursor is still in the ScatterView after
        /// the dragdrop operation has completed, it will be removed by the
        /// DragDropManager.
        /// </summary>
        public static bool GetIsTransientCursor(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTransientCursorProperty);
        }
        public static void SetIsTransientCursor(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTransientCursorProperty, value);
        }

        /// <summary>
        /// Attached property that is set on cursors at the start of a drag and
        /// drop operation. It is the ActualCenter of the item when the drag
        /// operation started.  It is used to animate a item back into place
        /// at the end of a copy operation.
        /// </summary>
        public static readonly DependencyProperty DragDropStartingPositionProperty =
            DependencyProperty.RegisterAttached("DragDropStartingPosition", typeof(Point), typeof(DragDropManager), new UIPropertyMetadata(new Point(double.NaN, double.NaN)));

        public static Point GetDragDropStartingPosition(DependencyObject obj)
        {
            return (Point)obj.GetValue(DragDropStartingPositionProperty);
        }

        public static void SetDragDropStartingPosition(DependencyObject obj, Point value)
        {
            obj.SetValue(DragDropStartingPositionProperty, value);
        }

        /// <summary>
        /// Attached DependencyProperty set on cursors to say if a scatter manipulation
        /// has started.
        /// </summary>
        /// <remarks>
        /// This is here to work around a problem with the
        /// ScatterView.  See comment for OnLostContactCapture.
        /// </remarks>
        private static readonly DependencyProperty HasScatterManipulationStartedProperty =
            DependencyProperty.RegisterAttached("HasScatterManipulationStarted", typeof(bool), typeof(DragDropManager), new UIPropertyMetadata(false));

        private static bool GetHasScatterManipulationStarted(ScatterViewItem obj)
        {
            return (bool)obj.GetValue(HasScatterManipulationStartedProperty);
        }
        private static void SetHasScatterManipulationStarted(ScatterViewItem obj, bool value)
        {
            obj.SetValue(HasScatterManipulationStartedProperty, value);
        }

        /// <summary>
        /// Sets up event handlers on a ScatterViewItem that make it send the drag and drop events.
        /// </summary>
        /// <param name="scatterViewItem"></param>
        public static void EnableDragDropForItem(ScatterViewItem scatterViewItem)
        {
            // Add event handlers on the scatter view item that let us send appropriate drag and drop events
            scatterViewItem.ScatterManipulationStarted += ScatterManipulationStarted;
            scatterViewItem.ScatterManipulationDelta += ScatterManipulationDelta;
            scatterViewItem.ScatterManipulationCompleted += ScatterManipulationCompleted;
            scatterViewItem.AddHandler(Contacts.LostContactCaptureEvent, new ContactEventHandler(OnLostContactCapture), true);
        }

        private static void ScatterManipulationStarted(object sender, ScatterManipulationStartedEventArgs scatterViewArgs)
        {
            ScatterViewItem item = (ScatterViewItem)sender;

            SetHasScatterManipulationStarted(item, true);

            SetDragDropStartingPosition(item, item.ActualCenter);

            OnCursorMoved(DragDropBegin, item);
        }

        private static void ScatterManipulationDelta(object sender, ScatterManipulationDeltaEventArgs scatterViewArgs)
        {
            ScatterViewItem item = (ScatterViewItem)sender;

            OnCursorMoved(DragDropMoved, item);
        }

        private static void OnCursorMoved(RoutedEvent routedEvent, ScatterViewItem item)
        {
            if(!draggingItems.Contains(item))
            {
                draggingItems.Add(item);
            }

            FrameworkElement oldClaimingElement;
            cursorClaims.TryGetValue(item, out oldClaimingElement);

            DragDropEventArgs movedArgs = new DragDropEventArgs(routedEvent, item, draggingItems);
            item.RaiseEvent(movedArgs);

            FrameworkElement newClaimingElement = movedArgs.ClaimingElement;
            if(oldClaimingElement != newClaimingElement)
            {
                // Tell anyone listening that the claim on the cursor has changed
                cursorClaims[item] = newClaimingElement;
                DragDropCursorClaimChangedEventArgs claimChangedArgs = new DragDropCursorClaimChangedEventArgs(oldClaimingElement, newClaimingElement);
                item.RaiseEvent(claimChangedArgs);

                // Set this so the cursor can change its appearance based on if it is
                // claimed or not.
                SetCursorIsClaimed(item, newClaimingElement != null);
            }
        }

        private static void ScatterManipulationCompleted(object sender, ScatterManipulationCompletedEventArgs scatterViewArgs)
        {
            ScatterViewItem item = (ScatterViewItem)sender;

            OnCursorDropped(item);
        }

        /// <summary>
        /// This is here to work around a problem in ScatterView.  While we
        /// successfully transfer capture of the contact that started the
        /// drag&drop to the ScatterViewItem, we may never get the  
        /// ScatterManipulation* events sent to us.  The result will be
        /// transient cursors left in the drag drop layer.
        /// This only seems to happen with contacts, not the mouse.
        /// </summary> 
        private static void OnLostContactCapture(object sender, ContactEventArgs args)
        {
            ScatterViewItem cursor = (ScatterViewItem)sender;

            if (GetHasScatterManipulationStarted(cursor))
            {
                // A scatter manipulation has started.
                // No need to do the workaround.
                return;
            }

            if (!GetIsTransientCursor(cursor))
            {
                // We only have to do the work around for
                // transient cursors
                return;
            }

            if (Contacts.GetIsAnyContactOrMouseCapturedWithin(cursor))
            {
                // The cursor still has a contact or a mouse captured
                // so there is no need to work around.
                return;
            }

            // OK, so we have a transient cursor that just lost it's last
            // captured manipulator and we never got a manipulation
            // started on it.  Pretend like we did and get rid of
            // the item..

            // We do this as a BeginInvoke the Surface Presentation
            // layer behaves incorrectly when re-entered this way.
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() => OnCursorDropped(cursor)));
        }

        private static void OnCursorDropped(ScatterViewItem item)
        {
            ScatterView dragDropScatterView = (ScatterView) item.Parent;

            if(draggingItems.Contains(item))
            {
                draggingItems.Remove(item);
            }

            FrameworkElement claimingElement;
            cursorClaims.TryGetValue(item, out claimingElement);

            DragDropEventArgs args = new DragDropEventArgs(DragDropEnded, item, draggingItems, claimingElement);
            item.RaiseEvent(args);

            if (claimingElement != null)
            {
                cursorClaims.Remove(item);
                DragDropCursorClaimChangedEventArgs claimChangedArgs = new DragDropCursorClaimChangedEventArgs(claimingElement, null);
                item.RaiseEvent(claimChangedArgs);
                SetCursorIsClaimed(item, false);
            }

            // If the cursor was a "Transient" cursor and it is still in the ScatterView that handled the drag and drop, remove it.
            if (GetIsTransientCursor(item))
            {
                ScatterView scatterView = item.Parent as ScatterView;

                if (scatterView != null && scatterView == dragDropScatterView)
                {
                    // TODO - maybe do some more graceful animation
                    scatterView.Items.Remove(item);
                }
            }
        }

        /// <summary>
        /// Helper for drop targets.  Called when a drag drop copy operation completes.
        /// Puts the curser back where it was when the drag drop operation started.
        /// </summary>
        public static void AnimateCursorBackToDragStart(ScatterViewItem cursor, TimeSpan beginIn)
        {
            const double offScreenDelta = 50;

            Point startingCenter = GetDragDropStartingPosition(cursor);
            if (!double.IsNaN(startingCenter.X) && !double.IsNaN(startingCenter.Y))
            {
                // Calculate the point off screen from which we will animate the cursor back on to the screen
                ScatterView parent = (ScatterView) cursor.Parent;
                Point from = new Point();
                from.X = startingCenter.X < (parent.ActualWidth/2.0)
                             ? -offScreenDelta
                             : parent.ActualWidth + offScreenDelta;
                from.Y = startingCenter.Y < (parent.ActualHeight / 2.0)
                             ? -offScreenDelta
                             : parent.ActualHeight + offScreenDelta;

                // duration of return animation is dependent of the distance
                double duration = 0.0006 * Point.Subtract(from, startingCenter).Length + 0.2;

                // Creaate the animation to move the item
                PointAnimation moveAnimation = new PointAnimation();
                moveAnimation.From = from;
                moveAnimation.To = startingCenter;
                moveAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration));
                moveAnimation.DecelerationRatio = 1.0;
                moveAnimation.BeginTime = beginIn;

                moveAnimation.Completed +=
                    delegate
                    {
                        cursor.Center = startingCenter;
                        cursor.BeginAnimation(ScatterViewItem.CenterProperty, null);
                    };

                cursor.BeginAnimation(ScatterViewItem.CenterProperty, moveAnimation);
            }
        }
    }
}
