'****************************** Module Header ******************************\
' Module Name:    VideoZoom.xaml.vb
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

Partial Public Class VideoZoom
    Inherits PhoneApplicationPage
    Private strLocalName As String = "howto.wmv"    ' Local video.
    Private maxHight As Double = 0                  ' Max hight.
    Private myMaxWidth As Double = 0                ' Max width.
    Private changeHight As Double = 0               ' The amount of the increase or decrease of the high.
    Private changeWidth As Double = 0               ' The amount of the increase or decrease of the width.   

    ' Constructor
    Public Sub New()
        InitializeComponent()

        maxHight = VideoPlayer.ActualHeight
        myMaxWidth = VideoPlayer.ActualWidth
        changeHight = maxHight / 20
        changeWidth = myMaxWidth / 20

        VideoPlayer.Source = New Uri(strLocalName, UriKind.Relative)
        AddHandler VideoPlayer.MediaEnded, AddressOf VideoPlayer_MediaEnded

        ' Prepare ApplicationBar and buttons.
        PhoneAppBar = DirectCast(ApplicationBar, ApplicationBar)
        PhoneAppBar.IsVisible = True
        StartZoomIn = DirectCast(ApplicationBar.Buttons(0), ApplicationBarIconButton)
        StartZoomOut = DirectCast(ApplicationBar.Buttons(1), ApplicationBarIconButton)
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)
    End Sub

    ''' <summary>
    ''' Display the viewfinder when playback ends.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Sub VideoPlayer_MediaEnded(sender As Object, e As RoutedEventArgs)
        If VideoPlayer.CurrentState <> MediaElementState.Playing Then
            VideoPlayer.[Stop]()
            VideoPlayer.Play()
        End If
    End Sub

    ''' <summary>
    ''' Zoom in.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnZoomIn_Click(sender As Object, e As EventArgs)
        If VideoPlayer.Height <= maxHight AndAlso VideoPlayer.Width <= myMaxWidth Then
            VideoPlayer.Pause()
            VideoPlayer.Height += changeHight
            VideoPlayer.Width += changeWidth
            VideoPlayer.Play()
        End If
    End Sub

    ''' <summary>
    ''' Zoom out.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnZoomOut_Click(sender As Object, e As EventArgs)
        If VideoPlayer.Height > changeHight AndAlso VideoPlayer.Width > changeWidth Then
            VideoPlayer.[Stop]()
            VideoPlayer.Height -= changeHight
            VideoPlayer.Width -= changeWidth
            VideoPlayer.Play()
        End If
    End Sub
End Class
