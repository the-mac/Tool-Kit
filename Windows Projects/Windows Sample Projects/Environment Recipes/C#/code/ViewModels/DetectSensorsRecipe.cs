/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdkAppEnvironmentWP8CS.ViewModels
{
    // This recipe detects the availability of sensors on the device running the sample.
    // For more info about sensors for Windows Phone, see http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202968(v=vs.105).aspx
    public class DetectSensorsRecipe : EnvironmentRecipe
    {
        public override string RecipeName
        {
            get
            {
                return "sensors";
            }
        }

        public override string RecipeDescription
        {
            get 
            {
                return "Detects the availability of sensors on the device.";
            }
        }

        internal override void RunRecipe()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" Compass? ");
            sb.Append(Compass.IsSupported ? "Yes" : "No");
            sb.Append(" Accelerometer? ");
            sb.Append(Accelerometer.IsSupported ? "Yes" : "No");
            sb.Append(" Gyroscope? ");
            sb.Append(Gyroscope.IsSupported ? "Yes" : "No");
            sb.Append(" Motion? ");
            sb.Append(Motion.IsSupported ? "Yes" : "No");

            RecipeResult = sb.ToString();
        }

        private string _snippet = string.Empty;
        public override string CodeSnippet
        {
            get
            {
                if (string.IsNullOrEmpty(_snippet))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("StringBuilder sb = new StringBuilder();");
                    sb.AppendLine("sb.Append(\" Compass? \");");
                    sb.AppendLine("sb.Append(Compass.IsSupported ? \"Yes\" : \"No\");");
                    sb.AppendLine("sb.Append(\" Accelerometer? \");");
                    sb.AppendLine("sb.Append(Accelerometer.IsSupported ? \"Yes\" : \"No\");");
                    sb.AppendLine("sb.Append(\" Gyroscope? \");");
                    sb.AppendLine("sb.Append(Gyroscope.IsSupported ? \"Yes\" : \"No\");");
                    sb.AppendLine("sb.Append(\" Motion? \");");
                    sb.AppendLine("sb.Append(Motion.IsSupported ? \"Yes\" : \"No\");");
                    sb.AppendLine("RecipeResult = sb.ToString();");

                    _snippet = sb.ToString();
                }

                return _snippet;
            }
        }
    }
}
