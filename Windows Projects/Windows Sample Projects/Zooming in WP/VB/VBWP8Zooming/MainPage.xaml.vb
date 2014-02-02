'****************************** Module Header ******************************\
' Module Name:    MainPage.xaml.vb
' Project:        VBWP8Zooming
' Copyright (c) Microsoft Corporation
'
' This demo shows how to zoom in/out image and video.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/
Imports System
Imports System.Threading
Imports System.Windows.Controls
Imports Microsoft.Phone.Controls
Imports Microsoft.Phone.Shell

Partial Public Class MainPage
    Inherits PhoneApplicationPage

    ' Constructor
    Public Sub New()
        InitializeComponent()

        SupportedOrientations = SupportedPageOrientation.Portrait Or SupportedPageOrientation.Landscape

        ' Sample code to localize the ApplicationBar
        'BuildLocalizedApplicationBar()

    End Sub

    Private Sub ImageZoom(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New Uri("/ImageZoom.xaml", UriKind.Relative))
    End Sub

    Private Sub VideoZoom(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate(New Uri("/VideoZoom.xaml", UriKind.Relative))
    End Sub
End Class