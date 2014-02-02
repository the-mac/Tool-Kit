/****************************** Module Header ******************************\
* Module Name:    Utilities.cs
* Project:        CSWP8AccessItemIndex
* Copyright (c) Microsoft Corporation
*
* Helper class is used to traverse the Visual Tree and get 
* FrameworkElement's index in ItemsControl.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace CSWP8AccessItemIndex
{
    public static class Utilities
    {
        /// <summary>
        /// Finds the parent.
        /// </summary>
        /// <param name="element">The element to start with.</param>
        /// <param name="filter">The filter criteria.</param>
        /// <returns></returns>
        public static DependencyObject FindParent(this DependencyObject element, Func<DependencyObject, bool> filter)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
       
            if (parent != null)
            {
                if (filter(parent))
                {
                    return parent;
                }

                return FindParent(parent, filter);
            }

            return null;
        }

        /// <summary>
        /// Get index of the UIElement by using IndexFromContainer
        /// </summary>
        /// <param name="element">FrameworkElement</param>
        /// <returns>index</returns>
        public static int GetIndexOfUIElement(FrameworkElement element)
        {
            int intIndex = -1;
            ItemsControl parent = element.FindParent(x => x is ItemsControl) as ItemsControl;

            if (parent != null)
            {
                DependencyObject container =
                    parent.ItemContainerGenerator.ContainerFromItem(
                    element.DataContext);

                if (container != null)
                {
                    intIndex = parent.ItemContainerGenerator.IndexFromContainer(
                        container);
                }
            }
            return intIndex;
        }
    }
}