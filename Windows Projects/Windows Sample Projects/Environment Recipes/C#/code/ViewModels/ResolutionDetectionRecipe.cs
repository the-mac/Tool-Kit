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
    // This recipe reports the screen resolution of the device on which the sample is running.
    // For more info, see http://msdn.microsoft.com/en-US/library/windowsphone/develop/jj206974(v=vs.105).aspx
    public class ResolutionDetectionRecipe : EnvironmentRecipe
    {
        public override string RecipeName
        {
            get
            {
                return "resolution";
            }
        }

        public override string RecipeDescription
        {
            get 
            {
                return "Detects the resolution of the device on which the app is running.";
            }
        }

        internal override void RunRecipe()
        {
            string resolution = string.Empty;
            switch (App.Current.Host.Content.ScaleFactor)
            {
                case 100:
                    resolution = "WVGA";
                    break;
                case 160:
                    resolution = "WXGA";
                    break;
                case 150:
                    resolution = "720p";
                    break;
                default:
                    resolution = "unknown";
                    break;
            }

            RecipeResult = String.Format("Screen resolution: {0} ", resolution);
        }

        private string _snippet = string.Empty;
        public override string CodeSnippet
        {
            get
            {
                if (string.IsNullOrEmpty(_snippet))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("switch (App.Current.Host.Content.ScaleFactor)");
                    sb.AppendLine("{");
                    sb.AppendLine("    case 100:");
                    sb.AppendLine("        resolution = \"WVGA\";");
                    sb.AppendLine("        break;");
                    sb.AppendLine("    case 160:");
                    sb.AppendLine("        resolution = \"WXGA\";");
                    sb.AppendLine("        break;");
                    sb.AppendLine("    case 150:");
                    sb.AppendLine("        resolution = \"720p\";");
                    sb.AppendLine("        break;");
                    sb.AppendLine("    default:");
                    sb.AppendLine("        resolution = \"unknown\";");
                    sb.AppendLine("        break;");
                    sb.AppendLine("}");


                    _snippet = sb.ToString();
                }

                return _snippet;
            }
        }
    }
}
