using System.Collections.Generic;
using System.Windows;
using Microsoft.Surface.Presentation.Controls;

namespace MobileConnectSample.DragDrop
{
    /// <summary>
    /// Arguments for events sent from the "cursor" of a drag drop operation
    /// </summary>
    public class DragDropEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The ScatterViewItem that is being dragged and dropped
        /// </summary>
        public ScatterViewItem Cursor { get; private set;}

        /// <summary>
        /// The list of all active cursors.
        /// </summary>
        public IEnumerable<ScatterViewItem> ActiveCursors { get; private set; }

        /// <summary>
        /// The DataContexts of all the active cursors
        /// </summary>
        public IEnumerable<object> ActiveCursorDataContexts
        {
            get
            {
                foreach(ScatterViewItem cursor in ActiveCursors)
                {
                    yield return cursor.DataContext;
                }
            }
        }

        /// <summary>
        /// The element that has claimed the cursor (may be null)
        /// </summary>
        public FrameworkElement ClaimingElement { get; private set; }

        public DragDropEventArgs(RoutedEvent routedEvent, ScatterViewItem cursor, IEnumerable<ScatterViewItem> draggingItems)
            : this(routedEvent, cursor, draggingItems, null)
        {
        }

        public DragDropEventArgs(RoutedEvent routedEvent, ScatterViewItem cursor, IEnumerable<ScatterViewItem> draggingItems, FrameworkElement claimingElement)
            : base(routedEvent)
        {
            Cursor = cursor;
            ActiveCursors = draggingItems;
            ClaimingElement = claimingElement;
        }

        /// <summary>
        /// Attemptts to claim the cursor.  Will not claim it if someone else has already done so.
        /// Drop targets should claim the cursor when they want to be the one to own it when
        /// it is dropped.
        /// </summary>
        public void TryClaimCursor(FrameworkElement newClaimingElement)
        {
            if(ClaimingElement == null)
            {
                ClaimingElement = newClaimingElement;
            }
        }
    }
}
