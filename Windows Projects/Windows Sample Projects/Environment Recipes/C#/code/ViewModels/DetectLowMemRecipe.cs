/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdkAppEnvironmentWP8CS.ViewModels
{
    // This recipe detects whether the sample is running on a low memory device. 
    // For info about developing apps for low-memory phones, see http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh855081(v=vs.105).aspx
    public class DetectLowMemRecipe : EnvironmentRecipe
    {
        public override string RecipeName
        {
            get
            {
                return "lowmem";
            }
        }

        public override string RecipeDescription
        {
            get 
            {
                return "Detects whether app is running on a low memory device.";
            }
        }

        internal override void RunRecipe()
        {
            long appWorkingSetLimit = DeviceStatus.ApplicationMemoryUsageLimit;

            // We are only checking for Windows Phone 8 devices. 
            // Windows Phone OS 7 devices have different memory limitations. 
            if (Environment.OSVersion.Version >= new Version(8, 0))
            {
                // A low-memory Windows Phone 8 device has a working set limit of 
                // 150 MB, which is 157286400 bytes.
                if (appWorkingSetLimit <= 157286400)
                {
                    RecipeResult = "low memory Windows Phone 8 device";
                }
                else
                {
                    RecipeResult = "normal Windows Phone 8 device";
                }
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
                    sb.AppendLine("if (Environment.OSVersion.Version > new Version(8, 0))");
                    sb.AppendLine("{");
                    sb.AppendLine("    if (appWorkingSetLimit <= 157286400)");
                    sb.AppendLine("    {");
                    sb.AppendLine("        RecipeResult = \"low memory Windows Phone 8 device\";");
                    sb.AppendLine("    }");
                    sb.AppendLine("    else");
                    sb.AppendLine("    {");
                    sb.AppendLine("        RecipeResult = \"normal Windows Phone 8 device\";");
                    sb.AppendLine("    }");
                    sb.AppendLine("}");

                    _snippet = sb.ToString();
                }

                return _snippet;
            }
        }
    }
}
