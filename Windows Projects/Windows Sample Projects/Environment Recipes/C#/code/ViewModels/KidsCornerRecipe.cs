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
using Microsoft.Devices;

namespace sdkAppEnvironmentWP8CS.ViewModels
{
    // This recipe detects whrther the sample is running inside Kid's Corner.
    // For more info about Kid's Corner, see http://msdn.microsoft.com/en-us/library/windowsphone/develop/dn168931(v=vs.105).aspx
    public class KidsCornerRecipe : EnvironmentRecipe
    {
        public override string RecipeName
        {
            get
            {
                return "kidsCorner";
            }
        }

        public override string RecipeDescription
        {
            get 
            {
                return "Checks whether the app is running in Kid's Corner.";
            }
        }

        internal override void RunRecipe()
        {
            if (Windows.Phone.ApplicationModel.ApplicationProfile.Modes 
                == Windows.Phone.ApplicationModel.ApplicationProfileModes.Alternate)
            {
                this.RecipeResult = "running in Kid's Corner";
            }
            else
            {
                this.RecipeResult = "not running in Kid's Corner";
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
                    sb.AppendLine("if (Windows.Phone.ApplicationModel.ApplicationProfile.Modes ");
                    sb.AppendLine("    == Windows.Phone.ApplicationModel.ApplicationProfileModes.Alternate)");
                    sb.AppendLine("{");
                    sb.AppendLine("    this.RecipeResult = \"running in Kid's Corner\";");
                    sb.AppendLine("}");
                    sb.AppendLine("else");
                    sb.AppendLine("{");
                    sb.AppendLine("    this.RecipeResult = \"not running in Kid's Corner\";");
                    sb.AppendLine("}");

                    _snippet = sb.ToString();
                }

                return _snippet;
            }
        }
    }
}
