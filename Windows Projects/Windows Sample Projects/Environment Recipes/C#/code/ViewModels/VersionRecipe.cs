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

namespace sdkAppEnvironmentWP8CS.ViewModels
{
    // This recipes checks the version of Windows Phone on which the sample is running. 
    public class VersionRecipe : EnvironmentRecipe
    {
        public override string RecipeName
        {
            get
            {
                return "version";
            }
        }

        public override string RecipeDescription
        {
            get 
            {
                return "Checks the OS version.";
            }
        }

        internal override void RunRecipe()
        {
            // In this example, we'll verify whether the OS is 7.8 or above.
            this.RecipeResult = String.Format(" OS Version: {0} \n 7.8 or above? {1}"
                , Environment.OSVersion.Version.ToString(), Environment.OSVersion.Version > new Version(7, 10, 8858));
        }

        private string _snippet = string.Empty;
        public override string CodeSnippet
        {
            get
            {
                if (string.IsNullOrEmpty(_snippet))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("// In this example, we'll verify whether the OS is 7.8 or above.");
                    sb.AppendLine("this.RecipeResult = String.Format(\" OS Version: {0} \n 7.8 or above? {1}\"");
                    sb.AppendLine("    , Environment.OSVersion.Version.ToString(), Environment.OSVersion.Version > new Version(7, 10, 8858));");

                    _snippet = sb.ToString();
                }

                return _snippet;
            }
        }
    }
}
