// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.Validation
{
    using System;

    public class DateValidator : IValidator
    {
        private bool isValid;

        public void Validate(object value)
        {
            var text = value as string;

            DateTime date;
            this.isValid = !string.IsNullOrWhiteSpace(text) && DateTime.TryParse(text, out date);
        }

        public bool IsValid()
        {
            return this.isValid;
        }
    }
}
