// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EventBuddy.Validation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed class ValidatingTextBox : TextBox
    {
        private static DependencyProperty watermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(ValidatingTextBox), new PropertyMetadata(null));

        private static DependencyProperty validatorProperty = DependencyProperty.Register("Validator", typeof(IValidator), typeof(ValidatingTextBox), new PropertyMetadata(null));

        public ValidatingTextBox()
        {
            this.DefaultStyleKey = typeof(ValidatingTextBox);
            this.GotFocus += this.WatermarkTextBoxGotFocus;
            this.LostFocus += this.WatermarkTextBoxLostFocus;
            this.TextChanged += this.OnTextChanged;
        }

        public static DependencyProperty WatermarkTemplateProperty
        {
            get { return watermarkTemplateProperty; }
        }

        public static DependencyProperty ValidatorProperty
        {
            get { return validatorProperty; }
        }

        public IValidator Validator
        {
            get { return (IValidator)GetValue(ValidatorProperty); }
            set { this.SetValue(ValidatorProperty, value); }
        }

        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { this.SetValue(WatermarkTemplateProperty, value); }
        }

        public void Validate()
        {
            this.Validator.Validate(this.Text);
        }

        public bool IsValid()
        {
            return this.Validator.IsValid();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // we need to set the initial state of the watermark
            this.GoToWatermarkVisualState(false);
        }

        private void WatermarkTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            this.GoToWatermarkVisualState();
        }

        private void WatermarkTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.GoToWatermarkVisualState(false);
        }

        private void GoToWatermarkVisualState(bool hasFocus = true)
        {
            this.Validator.Validate(this.Text);

            // if our text is empty and our control doesn't have focus then show the watermark
            // otherwise the control eirther has text or has focus which in either case we need to hide the watermark
            if (!this.Validator.IsValid() && !hasFocus)
            {
                this.GoToVisualState("ValidationFailed");
            }
            else
            {
                this.GoToVisualState("ValidationSucceded");
            }
        }

        private void GoToVisualState(string stateName, bool useTransitions = true)
        {
            VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            this.GoToWatermarkVisualState();
        }
    }
}
