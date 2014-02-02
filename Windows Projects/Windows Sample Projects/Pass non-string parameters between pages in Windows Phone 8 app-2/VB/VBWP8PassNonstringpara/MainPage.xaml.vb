'****************************** Module Header ******************************\
' Module Name:    MainPage.xaml.vb
' Project:        VBWP8PassNonstringpara
' Copyright (c) Microsoft Corporation
'
' This sample will demo how to pass no-string data between two pages.
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
Imports System.IO.IsolatedStorage
Imports System.IO
Imports System.Runtime.Serialization.Json

Partial Public Class MainPage
    Inherits PhoneApplicationPage

    ' Test data.
    Dim listString As List(Of String) = New List(Of String)

    ' Constructor
    Public Sub New()
        InitializeComponent()

        SupportedOrientations = SupportedPageOrientation.Portrait Or SupportedPageOrientation.Landscape

        For index = 1 To 20
            listString.Add("Current Item:" & index.ToString())
        Next

    End Sub

    Private Sub btnMethod1_Click(sender As Object, e As RoutedEventArgs)
        NavigationService.Navigate("/Result.xaml?name=1", listString)
    End Sub

    Private Sub btnMethod2_Click(sender As Object, e As RoutedEventArgs)
        App.ObjectNavigationData = listString
        NavigationService.Navigate(New Uri("/Result.xaml?name=2", UriKind.Relative))
    End Sub

    Private Sub btnMethod3_Click(sender As Object, e As RoutedEventArgs)
        Dim filePath As String = "objectData"

        Using isolatedStorage As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
            If isolatedStorage.FileExists(filePath) Then
                isolatedStorage.DeleteFile(filePath)
            End If

            Using fileStream As IsolatedStorageFileStream = isolatedStorage.OpenFile(filePath, FileMode.Create, FileAccess.Write)
                Dim writeDate As String = String.Empty

                ' Json serialization.
                Using ms As New MemoryStream()
                    Dim ser = New DataContractJsonSerializer(GetType(List(Of String)))
                    ser.WriteObject(ms, listString)
                    ms.Seek(0, SeekOrigin.Begin)
                    Dim reader = New StreamReader(ms)
                    writeDate = reader.ReadToEnd()
                End Using

                ' Save to IsolatedStorage.
                Using writer As New StreamWriter(fileStream)
                    writer.WriteLine(writeDate)
                End Using
            End Using
        End Using

        NavigationService.Navigate(New Uri("/Result.xaml?name=3", UriKind.Relative))
    End Sub
End Class