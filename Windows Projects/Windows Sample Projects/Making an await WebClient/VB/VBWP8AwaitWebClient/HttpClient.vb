'****************************** Module Header ******************************\
' Module Name:    HttpClient.vb
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

Imports System.Threading
Imports System.Threading.Tasks

Public Class HttpClient
    Inherits WebClient
    Private cookieContainer As New CookieContainer()
    Dim tcsGetByteArray As New TaskCompletionSource(Of Byte())()
    Dim tcsGetString As New TaskCompletionSource(Of String)()

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Get the string by URI string.
    ''' </summary>
    ''' <param name="strUri">The Uri the request is sent to.</param>
    ''' <returns>string</returns>
    Public Async Function GetStringAsync(strUri As String) As Task(Of String)
        Dim uri As New Uri(strUri)
        Dim result As String = Await GetStringAsync(uri)
        Return result
    End Function

    ''' <summary>
    ''' Get the string by URI.
    ''' </summary>
    ''' <param name="requestUri">The Uri the request is sent to.</param>
    ''' <returns>string</returns>
    Public Function GetStringAsync(requestUri As Uri) As Task(Of String)
        Try
            AddHandler Me.DownloadStringCompleted, AddressOf MyDownloadStringCompleted
            Me.DownloadStringAsync(requestUri)
        Catch ex As Exception
            tcsGetString.TrySetException(ex)
        End Try

        If tcsGetString.Task.Exception IsNot Nothing Then
            Throw tcsGetString.Task.Exception
        End If

        Return tcsGetString.Task
    End Function

    ''' <summary>
    ''' DownloadCompleted event of String.
    ''' </summary>
    ''' <param name="sender">Me.DownloadStringCompleted</param>
    ''' <param name="e">DownloadStringCompletedEventArgs</param>
    ''' <returns>Nothing</returns>
    ''' <remarks></remarks>
    Function MyDownloadStringCompleted(ByVal sender As Object, _
ByVal e As System.Net.DownloadStringCompletedEventArgs)
        If e.[Error] Is Nothing Then
            tcsGetString.TrySetResult(e.Result)
        Else
            tcsGetString.TrySetException(e.[Error])
        End If
        Return Nothing
    End Function

    ''' <summary>
    '''  Send a GET request to the specified Uri and return the response body as a
    '''  byte array in an asynchronous operation.
    ''' </summary>
    ''' <param name="requestUri">The Uri the request is sent to.</param>
    ''' <returns>
    ''' Returns System.Threading.Tasks.Task TResult>.The task object
    ''' representing the asynchronous operation.
    '''  </returns>
    Public Async Function GetByteArrayAsync(requestUri As String) As Task(Of Byte())
        Dim uri As New Uri(requestUri)
        Dim data As Byte() = Await GetByteArrayAsync(uri)
        Return data
    End Function

    ''' <summary>
    '''  Send a GET request to the specified Uri and return the response body as a
    '''  byte array in an asynchronous operation.
    ''' </summary>
    ''' <param name="requestUri">The Uri the request is sent to.</param>
    ''' <returns>Returns byte array.The task object
    ''' representing the asynchronous operation.</returns>
    Public Function GetByteArrayAsync(requestUri As Uri) As Task(Of Byte())
        Try
            AddHandler Me.DownloadStringCompleted, AddressOf MyDownloadByteArrayCompleted

            Me.DownloadStringAsync(requestUri)
        Catch ex As Exception
            tcsGetByteArray.TrySetException(ex)
        End Try

        If tcsGetByteArray.Task.Exception IsNot Nothing Then
            Throw tcsGetByteArray.Task.Exception
        End If

        Return tcsGetByteArray.Task
    End Function

    ''' <summary>
    ''' DownloadCompleted event of ByteArray.
    ''' </summary>
    ''' <param name="sender">Me.DownloadStringCompleted</param>
    ''' <param name="e">DownloadStringCompletedEventArgs</param>
    ''' <returns>Nothing</returns>
    ''' <remarks></remarks>
    Function MyDownloadByteArrayCompleted(ByVal sender As Object, _
ByVal e As System.Net.DownloadStringCompletedEventArgs)
        If e.[Error] Is Nothing Then
            Dim data As Byte() = ConvertStringToByte(e.Result)
            tcsGetByteArray.TrySetResult(data)
        Else
            tcsGetByteArray.TrySetException(e.[Error])
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Override the GetWebRequest method.
    ''' </summary>
    ''' <param name="address">The Uri the request is sent to.</param>
    ''' <returns>HttpWebRequest</returns>
    Protected Overrides Function GetWebRequest(address As Uri) As WebRequest
        Dim request As HttpWebRequest = TryCast(MyBase.GetWebRequest(address), HttpWebRequest)

        If request IsNot Nothing Then
            request.Method = "GET"
            request.CookieContainer = cookieContainer
        End If

        Return request
    End Function

    ''' <summary>
    ''' Convert String to byte array.
    ''' </summary>
    ''' <param name="strTemp">string</param>
    ''' <returns>byte array</returns>
    Private Shared Function ConvertStringToByte(strTemp As String) As Byte()
        Dim encoding As New System.Text.UTF8Encoding()
        Dim data As Byte() = encoding.GetBytes(strTemp)

        Return data
    End Function
End Class

