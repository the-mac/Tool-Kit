'****************************** Module Header ******************************\
' Module Name:    NavigationExtensions.vb
' Project:        VBWP8PassNonstringpara
' Copyright (c) Microsoft Corporation
'
' This sample will demo how to pass no-string data between two pages.
' This is custom extension for NavigationService.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Public Module NavigationExtensions
    ' Store parameters to be passed.
    Private _navigationData As Object = Nothing

    ''' <summary>
    ''' Set data.
    ''' </summary>
    ''' <param name="service">NavigationService</param>
    ''' <param name="page">Target page.</param>
    ''' <param name="data">Parameter data.</param>
    <System.Runtime.CompilerServices.Extension> _
    Public Sub Navigate(service As NavigationService, page As String, data As Object)
        _navigationData = data
        Try
            service.Navigate(New Uri(page, UriKind.Relative))
        Catch generatedExceptionName As Exception
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Get data.
    ''' </summary>
    ''' <param name="service">NavigationService</param>
    ''' <returns></returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function GetLastNavigationData(service As NavigationService) As Object
        Dim data As Object = _navigationData
        ' Reset
        _navigationData = Nothing
        Return data
    End Function
End Module


