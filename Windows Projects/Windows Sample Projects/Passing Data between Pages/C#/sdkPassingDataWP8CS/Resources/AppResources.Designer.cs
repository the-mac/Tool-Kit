/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
namespace sdkPassingDataWP8CS.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AppResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("sdkPassingDataWP8CS.Resources.AppResources", typeof(AppResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to add.
        /// </summary>
        public static string AppBarButtonText {
            get {
                return ResourceManager.GetString("AppBarButtonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Menu Item.
        /// </summary>
        public static string AppBarMenuItemText {
            get {
                return ResourceManager.GetString("AppBarMenuItemText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PASSING DATA FROM PAGE TO PAGE.
        /// </summary>
        public static string ApplicationTitle {
            get {
                return ResourceManager.GetString("ApplicationTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method:.
        /// </summary>
        public static string MethodTextBlockText {
            get {
                return ResourceManager.GetString("MethodTextBlockText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pass data in Page Uri.
        /// </summary>
        public static string Recipe1UriButtonText {
            get {
                return ResourceManager.GetString("Recipe1UriButtonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pass data in OnNavigatedFrom.
        /// </summary>
        public static string Recipe2EventButtonText {
            get {
                return ResourceManager.GetString("Recipe2EventButtonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pass data in app state.
        /// </summary>
        public static string Recipe3StateButtonText {
            get {
                return ResourceManager.GetString("Recipe3StateButtonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pass data in app variable.
        /// </summary>
        public static string Recipe4VarButtonText {
            get {
                return ResourceManager.GetString("Recipe4VarButtonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pass data in file.
        /// </summary>
        public static string Recipe5FileButtonText {
            get {
                return ResourceManager.GetString("Recipe5FileButtonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LeftToRight.
        /// </summary>
        public static string ResourceFlowDirection {
            get {
                return ResourceManager.GetString("ResourceFlowDirection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to en-US.
        /// </summary>
        public static string ResourceLanguage {
            get {
                return ResourceManager.GetString("ResourceLanguage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This text is passed from the source page to the target page..
        /// </summary>
        public static string SampleData {
            get {
                return ResourceManager.GetString("SampleData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to source page.
        /// </summary>
        public static string SourcePageTitle {
            get {
                return ResourceManager.GetString("SourcePageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to target page.
        /// </summary>
        public static string TargetPageTitle {
            get {
                return ResourceManager.GetString("TargetPageTitle", resourceCulture);
            }
        }
    }
}
