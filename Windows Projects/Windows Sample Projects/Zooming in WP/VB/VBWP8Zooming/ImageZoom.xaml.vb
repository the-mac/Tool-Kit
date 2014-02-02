'****************************** Module Header ******************************\
' Module Name:    ImageZoom.xaml.vb
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
Imports System.Windows.Resources
Imports System.IO
Imports System.Windows.Media.Imaging
Imports System.IO.IsolatedStorage
Imports System.Diagnostics

Partial Public Class ImageZoom
    Inherits PhoneApplicationPage
    Private initHight As Integer = 0              ' Initial high.
    Private initWidth As Integer = 0              ' Initial width.
    Private maxHight As Double = 0                ' Max hight.
    Private myMaxWidth As Double = 0              ' Max width.
    Private changeHight As Integer = 0            ' The amount of the increase or decrease of the high.
    Private changeWidth As Integer = 0            ' The amount of the increase or decrease of the width.
    Private strPath As String = "1code.jpg"       ' Image path.
    Private strImageName As String = "test.jpg"   ' Image name.
    Private isLoad As Boolean = False             ' Flag for initialization. 

    ' Constructor
    Public Sub New()
        InitializeComponent()
        ' Save image to isolated storage. You can capture an image from camera instead of local image.
        SaveImageToIsolatedStorage(initWidth, initHight)

        ' Load image and rendering.
        LoadImageFromIsolatedStorage(0, 0)

        ' Prevent repeat initialized.
        If Not isLoad Then
            myMaxWidth = img.Width
            maxHight = img.Height
            changeHight = Convert.ToInt32(((maxHight - initHight) / 20))
            changeWidth = Convert.ToInt32((myMaxWidth - initWidth) / 20)
            isLoad = True
        End If
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)
    End Sub

    ''' <summary>
    ''' Save image to IsolatedStorage.
    ''' </summary>     
    Private Sub SaveImageToIsolatedStorage(intPixelWidth As Integer, intPixelHeight As Integer)
        ' Use Uri to get local images.
        Dim sri As StreamResourceInfo = Nothing
        Dim uri As New Uri(strPath, UriKind.Relative)
        sri = Application.GetResourceStream(uri)

        Using iso As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
            If iso.FileExists(strImageName) Then
                iso.DeleteFile(strImageName)
            End If

            Using isostream As IsolatedStorageFileStream = iso.CreateFile(strImageName)
                Dim bitmap As New BitmapImage()
                bitmap.SetSource(sri.Stream)
                Dim wb As New WriteableBitmap(bitmap)

                If intPixelHeight > 0 Then
                    ' Encode WriteableBitmap object to a JPEG stream.
                    Extensions.SaveJpeg(wb, isostream, intPixelWidth, intPixelHeight, 0, 85)
                Else
                    ' Encode WriteableBitmap object to a JPEG stream.
                    Extensions.SaveJpeg(wb, isostream, wb.PixelWidth, wb.PixelHeight, 0, 85)
                End If
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Sample code for loading image from IsolatedStorage
    ''' </summary> 
    Private Sub LoadImageFromIsolatedStorage(intPixelWidth As Integer, intPixelHeight As Integer)
        ' The image will be read from isolated storage into the following byte array
        Dim data As Byte()

        ' Read the entire image in one go into a byte array
        Try
            Using isf As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
                Using isfs As IsolatedStorageFileStream = isf.OpenFile(strImageName, FileMode.Open, FileAccess.Read)
                    ' Allocate an array large enough for the entire file
                    data = New Byte(isfs.Length - 1) {}

                    ' Read the entire file and then close it
                    isfs.Read(data, 0, data.Length)
                End Using
            End Using

            ' Create memory stream and bitmap
            Dim ms As New MemoryStream(data)
            Dim bi As New BitmapImage()

            ' Set bitmap source to memory stream
            bi.SetSource(ms)

            ' Note: You must remove previous picture.
            LayoutRoot.Children.Remove(img)
            img = New Image()

            ' Set size of image to bitmap size for this demonstration
            If intPixelHeight > 0 Then
                img.Height = intPixelHeight
                img.Width = intPixelWidth
            Else
                img.Height = bi.PixelHeight
                img.Width = bi.PixelWidth
            End If

            img.Source = bi
            LayoutRoot.Children.Add(img)
        Catch e As Exception
            ' handle the exception
            Debug.WriteLine(e.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Zoom in.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnZoomIn_Click(sender As Object, e As EventArgs)
        If img.Width <= myMaxWidth AndAlso img.Height <= maxHight Then
            Dim width As Integer = GetInt(img.Width)            ' Current width.
            Dim hight As Integer = GetInt(img.Height)           ' Current hight.
            width = width + changeWidth
            hight = hight + changeHight
            SaveImageToIsolatedStorage(width, hight)
            LoadImageFromIsolatedStorage(width, hight)
        End If
    End Sub

    ''' <summary>
    ''' Zoom out.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnZoomOut_Click(sender As Object, e As EventArgs)
        If img.Width > changeWidth AndAlso img.Height > changeHight Then
            Dim width As Integer = GetInt(img.Width)            ' Current width.
            Dim hight As Integer = GetInt(img.Height)           ' Current hight.
            width = width - changeWidth
            hight = hight - changeHight
            SaveImageToIsolatedStorage(width, hight)
            LoadImageFromIsolatedStorage(width, hight)
        End If
    End Sub

    ''' <summary>
    ''' Convert method
    ''' </summary>
    ''' <param name="p"></param>
    ''' <returns></returns>
    Private Function GetInt(p As Double) As Integer
        Return Convert.ToInt32(p)
    End Function
End Class

