'****************************** Module Header ******************************\
' Module Name:    Result.xaml.vb
' Project:        VBWP8PassNonstringpara
' Copyright (c) Microsoft Corporation
'
' This sample will demo how to pass no-string data between two pages.
' This is result page.
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Imports System.IO.IsolatedStorage
Imports System.IO
Imports System.Runtime.Serialization.Json

Partial Public Class Result
    Inherits PhoneApplicationPage

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Store test data.
        Dim listString As New List(Of String)()

        ' Request parameter. The identification of the source page.
        Dim parameter As String = NavigationContext.QueryString("name")

        Select Case parameter
            Case "1"
                Dim myParameter = NavigationService.GetLastNavigationData()

                If myParameter IsNot Nothing Then
                    listString = DirectCast(myParameter, List(Of String))
                End If
                Exit Select
            Case "2"
                If App.ObjectNavigationData IsNot Nothing Then
                    listString = DirectCast(App.ObjectNavigationData, List(Of String))
                End If

                ' Reset.
                App.ObjectNavigationData = Nothing
                Exit Select
            Case "3"
                Dim filePath As String = "objectData"
                Using isolatedStorage As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
                    If isolatedStorage.FileExists(filePath) Then
                        Using fileStream As IsolatedStorageFileStream = isolatedStorage.OpenFile(filePath, FileMode.Open, FileAccess.Read)
                            Dim strObjData As String = String.Empty
                            Using reader As New StreamReader(fileStream)
                                strObjData = reader.ReadToEnd()
                            End Using
                            Dim byteArray As Byte() = System.Text.Encoding.Unicode.GetBytes(strObjData)
                            Dim stream As New MemoryStream(byteArray)

                            Dim serializer As New DataContractJsonSerializer(GetType(List(Of String)))
                            listString = DirectCast(serializer.ReadObject(stream), List(Of String))
                        End Using
                    End If

                    ' Reset.
                    isolatedStorage.DeleteFile(filePath)
                End Using
                Exit Select
            Case Else
                Exit Select
        End Select

        ' Bind to control.
        longListSelector.ItemsSource = listString
    End Sub

End Class
