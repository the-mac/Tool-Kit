// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

namespace EventBuddy.Validation
{
    public class RequiredValidator : IValidator
    {
        private bool isValid;

        public void Validate(object value)
        {
            var text = value as string;
            this.isValid = !string.IsNullOrWhiteSpace(text);
        }

        public bool IsValid()
        {
            return this.isValid;
        }
    }
}
