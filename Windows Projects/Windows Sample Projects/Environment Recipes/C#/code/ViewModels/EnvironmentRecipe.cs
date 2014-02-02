/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sdkAppEnvironmentWP8CS.ViewModels
{
    /// <summary>
    /// Base class for all recipes in this sample.
    /// The most relevant part of each recipe will be what's implemented in
    /// the RunRecipe() method. The rest of the properties/commands are used
    /// to hook the recipe nicely into the sample. 
    /// </summary>
    public abstract class EnvironmentRecipe : SdkHelper.PropertyChangedNotifier
    {
        /// <summary>
        /// The name of the recipe
        /// </summary>
        public abstract string RecipeName { get; }

        /// <summary>
        /// A short description of the recipe
        /// </summary>
        public abstract string RecipeDescription { get;  }

        /// <summary>
        /// A string representation of the key code snippet for this recipe. 
        /// This is generated manually in each derived class.
        /// </summary>
        public abstract string CodeSnippet { get; }

        /// <summary>
        /// The work carried out by the recipe.
        /// </summary>
        internal abstract void RunRecipe();

        /// <summary>
        /// Run the recipe.
        /// </summary>
        /// <remarks> The results of the recipe execution are displayed in the RecipeResult property.</remarks>
        private SdkHelper.RelayCommand _runRecipeCommand;
        public SdkHelper.RelayCommand RunRecipeCommand
        {
            get
            {
                if (_runRecipeCommand == null)
                {
                    _runRecipeCommand = new SdkHelper.RelayCommand(
                        param => RunRecipe(),
                        param => true
                    );
                }
                return _runRecipeCommand;
            }
        }

        private string _result = string.Empty;

        /// <summary>
        /// The result string of the recipe, which is set when the recipe code is executed using the 
        /// RunRecipeCommand. 
        /// </summary>
        public string RecipeResult 
        {
            get
            {
                return _result;
            }
            set
            {
                this.SetPropertyAndRaisePropertyChanged(ref this._result, value);
            }
        }

        

        private SdkHelper.RelayCommand _copySnippetCommand;
        public SdkHelper.RelayCommand CopySnippetCommand
        {
            get
            {
                if (_copySnippetCommand == null)
                {
                    this._copySnippetCommand = new SdkHelper.RelayCommand(
                        param => CopySnippetToClipboard(),
                        param => true
                        );
                }

                return _copySnippetCommand;
            }
        }

        private void CopySnippetToClipboard()
        {
            Clipboard.SetText(this.CodeSnippet);
        }

        private SdkHelper.RelayCommand _emailSnippetCommand;
        public SdkHelper.RelayCommand EmailSnippetCommand
        {
            get
            {
                if (_emailSnippetCommand == null)
                {
                    this._emailSnippetCommand = new SdkHelper.RelayCommand(
                        param => ShareSnippet(),
                        param => true
                        );
                }

                return _emailSnippetCommand;
            }
        }

        private void ShareSnippet()
        {
            EmailComposeTask email = new EmailComposeTask();
            email.Body = this.CodeSnippet;
            email.Subject = String.Format("Code Snippet: {0} ", this.RecipeName);
            email.Show();
        }



    }
}
