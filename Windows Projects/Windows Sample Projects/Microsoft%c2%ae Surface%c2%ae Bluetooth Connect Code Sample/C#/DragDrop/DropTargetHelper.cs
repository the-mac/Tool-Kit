using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MobileConnectSample.DragDrop
{
    class DropTargetHelper
    {
        private Window dragDropEventsTarget;

        public DropTargetHelper(FrameworkElement owner)
        {
            owner.Loaded += OnLoaded;
            owner.Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            dragDropEventsTarget = Window.GetWindow((FrameworkElement) sender);

            dragDropEventsTarget.AddHandler(DragDropManager.DragDropBegin, new EventHandler<DragDropEventArgs>(OnDragDropBegin));
            dragDropEventsTarget.AddHandler(DragDropManager.DragDropMoved, new EventHandler<DragDropEventArgs>(OnDragDropMoved));
            dragDropEventsTarget.AddHandler(DragDropManager.DragDropCursorClaimChanged, new EventHandler<DragDropCursorClaimChangedEventArgs>(OnDragDropClaimChanged));
            dragDropEventsTarget.AddHandler(DragDropManager.DragDropEnded, new EventHandler<DragDropEventArgs>(OnDragDropEnded));
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            dragDropEventsTarget.RemoveHandler(DragDropManager.DragDropBegin, new EventHandler<DragDropEventArgs>(OnDragDropBegin));
            dragDropEventsTarget.RemoveHandler(DragDropManager.DragDropMoved, new EventHandler<DragDropEventArgs>(OnDragDropMoved));
            dragDropEventsTarget.RemoveHandler(DragDropManager.DragDropCursorClaimChanged, new EventHandler<DragDropCursorClaimChangedEventArgs>(OnDragDropClaimChanged));
            dragDropEventsTarget.RemoveHandler(DragDropManager.DragDropEnded, new EventHandler<DragDropEventArgs>(OnDragDropEnded));

            dragDropEventsTarget = null;
        }

        public EventHandler<DragDropEventArgs> Begin;
        public EventHandler<DragDropEventArgs> Moved;
        public EventHandler<DragDropCursorClaimChangedEventArgs> ClaimChanged;
        public EventHandler<DragDropEventArgs> Ended;

        private void OnDragDropBegin(object sender, DragDropEventArgs dragDropArgs)
        {
            if(Begin != null)
            {
                Begin(sender, dragDropArgs);
            }
        }

        private void OnDragDropMoved(object sender, DragDropEventArgs dragDropArgs)
        {
            if(Moved != null)
            {
                Moved(sender, dragDropArgs);
            }
        }

        private void OnDragDropClaimChanged(object sender, DragDropCursorClaimChangedEventArgs claimChangedArgs)
        {
            if(ClaimChanged != null)
            {
                ClaimChanged(sender, claimChangedArgs);
            }
        }

        private void OnDragDropEnded(object sender, DragDropEventArgs dragDropArgs)
        {
            if(Ended != null)
            {
                Ended(sender, dragDropArgs);
            }
        }

        /// <summary>
        /// Helper to get the DataContext of a FrameworkElement in a safe way.
        /// </summary>
        public static bool TryGetDataContext<T>(FrameworkElement element, out T dataContext) where T : class
        {
            dataContext = null;
            if (element != null)
            {
                dataContext = element.DataContext as T;
            }
            return (dataContext != null);
        }
    }
}
