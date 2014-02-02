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
    // This recipe detects whether the sample is running in the emulator.
    // For more info about the emulator, see http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff402563(v=vs.105).aspx
    public class EmulatorModeRecipe : EnvironmentRecipe
    {
        public override string RecipeName
        {
            get
            {
                return "emulator";
            }
        }

        public override string RecipeDescription
        {
            get 
            {
                return "Checks whether the app is running on the emulator or on a device.";
            }
        }

        internal override void RunRecipe()
        {
            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
            {
                this.RecipeResult = "running in emulator";
            }
            else
            {
                this.RecipeResult = "running on a device";
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
                    sb.AppendLine("if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)");
                    sb.AppendLine("{");
                    sb.AppendLine("    this.RecipeResult = \"running in emulator\";");
                    sb.AppendLine("}");
                    sb.AppendLine("else");
                    sb.AppendLine("{");
                    sb.AppendLine("    this.RecipeResult = \"running on a device\";");
                    sb.AppendLine("}");

                    _snippet = sb.ToString();
                }

                return _snippet;
            }
        }
    }
}
