'***************************** Module Header ******************************\
' Module Name:    MainPage.xaml.vb
' Project:        VBWP8ImageOperation
' Copyright (c) Microsoft Corporation
'
' The project illustrates how to dynamically create ,display and remove images
' at run time. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************

Imports System
Imports System.Threading
Imports System.Windows.Controls
Imports Microsoft.Phone.Controls
Imports Microsoft.Phone.Shell
Imports System.Windows.Media.Imaging

Partial Public Class MainPage
    Inherits PhoneApplicationPage

    ' Constructor
    Public Sub New()
        InitializeComponent()
        SupportedOrientations = SupportedPageOrientation.Portrait Or SupportedPageOrientation.Landscape
        DynamicImages()
        RotationSetUp()
    End Sub

    Private intCurrentIndex As Integer = 0

    ' The index of the selected image.
    Private imgDefaultName As String = "img"

    ' Default Name of the image.
    Private newImg As Image = Nothing

    ' The selected image.
    ''' <summary>
    ''' Create images and associated events to them.
    ''' </summary>
    Private Sub DynamicImages()
        For ctr As Integer = 0 To 3
            Dim img As New Image()
            img.Stretch = Stretch.Uniform
            img.Name = imgDefaultName & ctr.ToString()
            img.Height = 240
            img.Width = 180
            AddHandler img.MouseLeftButtonDown, AddressOf img_MouseLeftButtonDown

            If ctr < 2 Then
                img.Margin = New Thickness(ctr * (img.ActualWidth / 10) + 20, img.ActualHeight / 10, 0, 0)
                canvas1.Children.Add(img)
                Canvas.SetLeft(img, ctr * img.ActualWidth)
                Canvas.SetTop(img, img.ActualHeight / 10)
            Else
                img.Margin = New Thickness((ctr - 2) * (img.ActualWidth / 10) + 20, img.ActualHeight / 10, 0, 0)
                canvas1.Children.Add(img)
                Canvas.SetLeft(img, ((ctr - 2) * img.ActualWidth))
                Canvas.SetTop(img, img.ActualHeight + img.ActualHeight / 10)
            End If

            img.Source = New BitmapImage(New Uri("1code.jpg", UriKind.Relative))
        Next
    End Sub

    ''' <summary>
    ''' Remove Images
    ''' </summary>       
    Public Sub DynamicImagesDel()
        ' Demo the remove the selected images.
        canvas1.Children.RemoveAt(intCurrentIndex)

        ' [-or-] 

        ' Demo the remove all images.
        ' canvas1.Children.Clear()   

        ' [-or-] 

        ' Demo the remove all images.
        ' Dim listDel As New List(Of UIElement)()
        ' listDel = canvas1.Children.ToList()
        ' For Each item As UIElement In listDel
        '    newImg = TryCast(item, Image)
        '    intCurrentIndex = canvas1.Children.IndexOf(newImg)
        '    canvas1.Children.RemoveAt(intCurrentIndex)
        ' Next
    End Sub

#Region "Properties for Animation."
    Private pp1 As New PlaneProjection()
    Private sB1 As New Storyboard()
    Private dA1 As New DoubleAnimation()
    Private tSpan As New TimeSpan(0, 0, 0, 0, 3000)
#End Region

    ''' <summary>
    ''' Animation of the selected images.
    ''' </summary>
    Private Sub RotationSetUp()
        dA1.Duration = tSpan
        dA1.From = 0
        dA1.[To] = -180

        pp1.CenterOfRotationZ = -0.5
        Storyboard.SetTarget(dA1, pp1)
        Storyboard.SetTargetProperty(dA1, New PropertyPath(PlaneProjection.RotationZProperty))
        AddHandler sB1.Completed, AddressOf sB1_Completed
        sB1.AutoReverse = True
        sB1.Children.Add(dA1)
    End Sub

    ' Don't allow another animations until current one is finished.   
    Private done As Boolean = True

    ''' <summary>
    ''' This will handle the Storyboard's Completed event. And reset the flag(done).          
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub sB1_Completed(sender As Object, e As EventArgs)
        ' Remove the projection or it will rotate with another pictures. 
        newImg.Projection = Nothing

        ' Reset the flag.
        done = True

        ' Remove the image.
        DynamicImagesDel()
    End Sub

    ''' <summary>
    ''' We will use the images' MouseLeftButtonDown event to start Storyboard.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub img_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        If Not done Then
            Return   ' The animation can't start yet.
        End If

        done = False ' Clear the flag. 

        newImg = TryCast(e.OriginalSource, Image)
        newImg.Projection = pp1

        ' Store the index of the selected image.
        intCurrentIndex = canvas1.Children.IndexOf(newImg)

        ' Animation of the selected images.
        sB1.Begin()
    End Sub
End Class