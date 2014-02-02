/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace sdkAppEnvironmentWP8CS.ViewModels
{
    // This recipe determines whether the device on which the sample is running is 
    // using the light or dark theme.
    // For more info, see http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff769552(v=vs.105).aspx#BKMK_ThemeVisibilityAndOpacity
    public class ThemeDetectionRecipe : EnvironmentRecipe
    {
        public override string RecipeName
        {
            get
            {
                return "theme";
            }
        }

        public override string RecipeDescription
        {
            get 
            {
                return "Detects whether the phone is running in the light or dark theme.";
            }
        }

        internal override void RunRecipe()
        {
            Visibility lightThemeVisibility = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"];
            if (lightThemeVisibility == Visibility.Visible)
            {
                RecipeResult = "using Light theme";
            }
            else
            {
                RecipeResult = "using Dark theme";
            }
        }

        private string _snippet = string.Empty;
        public override string CodeSnippet
        {
            get
            {
                if (string.IsNullOrEmpty(_snippet))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Visibility lightThemeVisibility = (Visibility)Application.Current.Resources[\"PhoneLightThemeVisibility\"];");
                    sb.AppendLine("if (lightThemeVisibility == Visibility.Visible)");
                    sb.AppendLine("{");
                    sb.AppendLine("    RecipeResult = \"using Light theme\";");
                    sb.AppendLine("}");
                    sb.AppendLine("else");
                    sb.AppendLine("{");
                    sb.AppendLine("    RecipeResult = \"using dark theme\";");
                    sb.AppendLine("}");

                    _snippet = sb.ToString();
                }

                return _snippet;
            }
        }
    }
}
