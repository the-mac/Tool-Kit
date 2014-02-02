'***************************** Module Header *******************************\
' Module Name:  MainPage.xaml.vb
' Project:      VBWP7MultiTouchDrawing
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to draw pictures with Canvas Control on a multi-touch screen.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Windows.Media.Imaging
Imports Microsoft.Xna.Framework.Media
Imports System.IO
Imports System.Windows

Partial Public Class MainPage
    Inherits PhoneApplicationPage
    ' Constructor
    Public Sub New()
        InitializeComponent()
        AddHandler Touch.FrameReported, AddressOf Me.Touch_FrameReported
    End Sub

    ' preXArray and preYArray are used to store the start point 
    ' for each touch point. currently silverlight support 4 muliti-touch
    ' here declare as 10 points for further needs. 
    Private preXArray As Double() = New Double(9) {}
    Private preYArray As Double() = New Double(9) {}

    ''' <summary>
    ''' Every touch action will rise this event handler. 
    ''' </summary>
    Private Sub Touch_FrameReported(sender As Object, e As TouchFrameEventArgs)
        Dim pointsNumber As Integer = e.GetTouchPoints(drawCanvas).Count
        Dim pointCollection As TouchPointCollection = e.GetTouchPoints(drawCanvas)

        For i As Integer = 0 To pointsNumber - 1
            If pointCollection(i).Action = TouchAction.Down Then
                preXArray(i) = pointCollection(i).Position.X
                preYArray(i) = pointCollection(i).Position.Y
            End If
            If pointCollection(i).Action = TouchAction.Move Then
                Dim line As New Line()

                line.X1 = preXArray(i)
                line.Y1 = preYArray(i)
                line.X2 = pointCollection(i).Position.X
                line.Y2 = pointCollection(i).Position.Y

                line.Stroke = New SolidColorBrush(Colors.Black)
                line.Fill = New SolidColorBrush(Colors.Black)
                drawCanvas.Children.Add(line)

                preXArray(i) = pointCollection(i).Position.X
                preYArray(i) = pointCollection(i).Position.Y
            End If
        Next
    End Sub

    ''' <summary>
    ''' Save image to Media Library so that we can view pictures we created
    ''' </summary>
    Private Sub Save_Click(sender As Object, e As RoutedEventArgs)
        Dim library As New MediaLibrary()
        Dim bitMap As New WriteableBitmap(drawCanvas, Nothing)
        Dim ms As New MemoryStream()
        Extensions.SaveJpeg(bitMap, ms, bitMap.PixelWidth, bitMap.PixelHeight, 0, 100)
        ms.Seek(0, SeekOrigin.Begin)
        library.SavePicture(String.Format("Images\{0}.jpg", Guid.NewGuid()), ms)
    End Sub

    Private Sub New_Click(sender As Object, e As RoutedEventArgs)
        drawCanvas.Children.Clear()
    End Sub
End Class