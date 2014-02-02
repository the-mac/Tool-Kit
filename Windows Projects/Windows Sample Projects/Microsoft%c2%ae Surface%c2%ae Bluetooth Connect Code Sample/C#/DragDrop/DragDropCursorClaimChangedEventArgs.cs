using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MobileConnectSample.DragDrop
{
    /// <summary>
    /// Arguments sent when the claim on the cursor changes.
    /// </summary>
    class DragDropCursorClaimChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The FrameworkElement that was previously claiming the cursor (may be null)
        /// </summary>
        public FrameworkElement OldClaimingElement { get; private set; }

        /// <summary>
        /// The FrameworkElement that is now claiming the cursor.
        /// </summary>
        public FrameworkElement NewClaimingElement { get; private set; }

        public DragDropCursorClaimChangedEventArgs(FrameworkElement oldElement, FrameworkElement newElement)
            : base(DragDropManager.DragDropCursorClaimChanged)
        {
            OldClaimingElement = oldElement;
            NewClaimingElement = newElement;
        }
    }
}
