'****************************** Module Header ******************************\
' Module Name:    MainPage.xaml.vb
' Project:        VBWP8AwaitWebClient
' Copyright (c) Microsoft Corporation
'
' This demo shows how to make an await WebClient
' (similar to HttpClient in Windows 8).
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

        AddHandler Loaded, AddressOf MainPage_Loaded
    End Sub

    Private Async Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs)
        Await getResult()
    End Sub

    ''' <summary>
    ''' Get content of URL.
    ''' </summary>
    Private Async Function getResult() As Tasks.Task
        Dim strRequestUri As String = "http://www.bing.com"
        Dim strResult As String = String.Empty

        Try
            ' Create a new instance.
            Dim httpClient As New HttpClient()

            Try
                ' GetStringAsync
                strResult = Await httpClient.GetStringAsync(strRequestUri)

                ' GetByteArrayAsync
                Dim dataArray As System.Byte() = Await httpClient.GetByteArrayAsync(strRequestUri)
                ' strResult = CStr(dataArray.Length)
            Catch ex As Exception
                strResult = ex.Message
            End Try
        Catch ex As Exception
            strResult = ex.Message
        Finally
            ' Show the result.
            Me.Dispatcher.BeginInvoke(Sub() tbResult.Text = strResult)
        End Try
    End Function

End Class