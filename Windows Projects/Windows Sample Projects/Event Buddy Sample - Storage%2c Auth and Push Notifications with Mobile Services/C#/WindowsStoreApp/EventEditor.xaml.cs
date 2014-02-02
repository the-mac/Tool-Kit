// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class EventEditor : UserControl
    {
        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register("Shown", typeof(bool), typeof(EventEditor), new PropertyMetadata(false, ShownChanged));

        public EventEditor()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "hidden", false);
        }

        public delegate void CancelledEditor(object sender, CancelledEditorEventArgs args);

        public delegate void SaveEditor(object sender, SaveEditorEventArgs args);

        public event CancelledEditor Cancelled;

        public event SaveEditor Save;

        public bool Shown
        {
            get { return (bool)GetValue(ShownProperty); }
            set { this.SetValue(ShownProperty, (bool)value); }
        }

        public void InvokeCancelled()
        {
            var cncl = this.Cancelled;
            if (cncl != null)
            {
                cncl.Invoke(this, new CancelledEditorEventArgs());
            }
        }

        public void InvokeSave()
        {
            var accpt = this.Save;
            if (accpt != null)
            {
                accpt.Invoke(this, new SaveEditorEventArgs());
            }
        }

        public void Show()
        {
            VisualStateManager.GoToState(this, "shown", true);
            txtTitle.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        public void Hide()
        {
            VisualStateManager.GoToState(this, "hidden", true);
        }

        private static void ShownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EventEditor;
            if (editor != null)
            {
                if ((bool)e.NewValue)
                {
                    editor.Show();
                }
                else
                {
                    editor.Hide();
                }
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Shown = false;
            this.InvokeCancelled();
            this.Hide();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (txtTitle.IsValid() && endDate.IsValid() && startDate.IsValid())
            {
                this.Hide();
                this.Shown = false;

                this.InvokeSave();
            }
        }

        public class CancelledEditorEventArgs : EventArgs
        {
        }

        public class SaveEditorEventArgs : EventArgs
        {
        }
    }
}
