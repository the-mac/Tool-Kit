/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTaskAgent1
{
  // This class handles uploading the file to the service.
  internal class AsyncHttpPostHelper
  {
    internal RequestState requestState;
    internal HttpWebRequest request;
    // This delegate references the method passed by the ServiceUploadHelper. It
    // allows the logic for finding the file to upload and uploading the file to
    // the service be separated. It is set in the BeginSend() method.
    internal delegate void OnHttpPostCompleteDelegate(Picture picture);
    OnHttpPostCompleteDelegate httpPostCompleteCallbackDelegate;

    // TODO: Adjust as needed.
    // The ConstantStrings class contains most of the settings that the
    // developer should modify. A full description of each component is
    // contained below.
    public class ConstantStrings
    { 
      // apiUri: the URI to post the content. The string stored in the
      // settingsKey is appended to the apiUri when uploading.
      public static string apiUri = "https://api.contoso.com/files?access_token=";
      public static string settingsKey = "AccessToken";
      // contentType: Set in the HTTP request
      public static string contentType = "multipart/form-data; boundary=A300x";
      public static string method = "POST";
      // headerString: sent before uploading the file, adjust as necessary
      public static string headerString = "--A300x\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
      // footerString: sent after the file has been submitted
      public static string footerString = "\r\n--A300x--";
      // albumName: Determines which album from which to upload photos.
      public static string albumName = "Camera Roll";
    }

    // The RequestState is passed between the callbacks to keep track of the image being uploaded.
    internal class RequestState
    {
      public HttpWebRequest request; // the HTTP request
      public Picture picture; // picture being uploaded
      public Stream source; // source stream, taken from the Picture
      public Stream destination; // destination stream to the service
      public byte[] postData = null; // buffers the data to send, set in the RequestState constructor
      public byte[] buffer; // buffer for other data.
      public byte[] headerBytes; // buffer for bytes sent in the header
      public byte[] footerBytes = Encoding.UTF8.GetBytes(ConstantStrings.footerString);
      public int bytesWritten; // tracks the number of bytes sent. Must match the ContentLength

      public RequestState(Picture p, Uri u)
      {
        picture = p;
        headerBytes = System.Text.Encoding.UTF8.GetBytes(String.Format(ConstantStrings.headerString, HttpUtility.UrlEncode(picture.Name)));
        request = (HttpWebRequest)WebRequest.Create(u);
        request.Method = ConstantStrings.method;
        request.AllowWriteStreamBuffering = false; // important, allows data to be sent immediately, avoiding OOM exceptions
        request.ContentType = ConstantStrings.contentType;
        request.ContentLength = picture.GetImage().Length + headerBytes.Length + footerBytes.Length;
        bytesWritten = 0;
        postData = new byte[4 * 1024];
      }
    }



       //  BeginSend() starts the asynchronous operation by calling
       //  GetRequestStreamCallback().

       //  Once the request stream has been received, GetRequestStreamCallback()
       //  ends the operation by saving the stream in the RequestState and then
       //  calls SendNextChunk() to post the data to the stream.

       //  SendNextChunk() determines whether to send the header, footer, or body of
       //  the POST request by comparing the number of bytes sent and the
       //  ContentLength set earlier (in the RequestState constructor).

       //  Note that it's important to handle reads and writes asynchronously to
       //  improve the performance of the background agent, especially if the image
       //  is large. If SendNextChunk() elects to send the header or footer to the
       //  destination stream then GetStreamWriteCallback() will post to the stream
       //  the existing header or footer byte array stored in the RequestState.

       //  If SendNextChunk() elects to send the body to the destination stream, then
       //  GetStreamReadCallback() will asynchronously read the image, 4 kB at a
       //  time (see postData[] in RequestState), and write those 4 kB to the
       //  destination stream by calling GetStreamWriteCallback().

       //  Once everything has been sent to the destination stream, SendNextChunk()
       //  calls RespCallback() to asynchronously get the response from the service.

       //  RespCallback() ends the operation, closes the stream objects (optionally
       //  saving the response if necessary), and calls EndSend() to clean up the
       //  flow.

       //  EndSend() returns to the calling method, indicating that the next picture
       //  can be sent.

    internal void BeginSend(OnHttpPostCompleteDelegate callbackDelegate)
    {
      try
      {
        httpPostCompleteCallbackDelegate = callbackDelegate;
        request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), requestState);
      }
      catch (Exception ex)
      {
	// TODO: Handle Exception
      }
    }

    internal void EndSend(Picture p)
    {
      httpPostCompleteCallbackDelegate(p); // End the upload flow for this picture.
    }

    public AsyncHttpPostHelper(Picture p, Uri u)
    {
      requestState = new RequestState(p, u);
      request = requestState.request;
    }

    // The comparisons here must be processed in the listed order to ensure that the
    // file is uploaded correctly.
    private void SendNextChunk(RequestState rs)
    {
      if (rs.bytesWritten == 0) // send header
      {
        rs.buffer = rs.headerBytes;
        rs.destination.BeginWrite(rs.buffer, 0, rs.buffer.Length, new AsyncCallback(GetStreamWriteCallback), rs);
        rs.bytesWritten += rs.buffer.Length;
      }
      else if (rs.bytesWritten == rs.request.ContentLength - rs.footerBytes.Length) // send footer
      {
        rs.buffer = rs.footerBytes;
        rs.destination.BeginWrite(rs.buffer, 0, rs.buffer.Length, new AsyncCallback(GetStreamWriteCallback), rs);
        rs.bytesWritten += rs.buffer.Length;
      }
      else if (rs.bytesWritten < rs.request.ContentLength) // send body
      {
        rs.buffer = rs.postData;
        rs.source.BeginRead(rs.buffer, 0, rs.buffer.Length, new AsyncCallback(GetStreamReadCallback), rs);
      }
      else if (rs.bytesWritten == rs.request.ContentLength) // stop sending
      {
        rs.destination.Close();
        rs.bytesWritten = 0; // reset the bytes written counter
        rs.request.BeginGetResponse(new AsyncCallback(RespCallback), rs);
      }
      else 
      {
        // TODO: Handle error.
      }
    }

    // Reads the source stream and starts writing to the destination stream.
    private void GetStreamReadCallback(IAsyncResult ar)
    {
      RequestState requestState = (RequestState)ar.AsyncState;
      int bytesRead = requestState.source.EndRead(ar);
      requestState.bytesWritten += bytesRead;
      try
      {
        requestState.destination.BeginWrite(requestState.buffer, 0, bytesRead, new AsyncCallback(GetStreamWriteCallback), requestState);
      }
      catch (Exception ex)
      {
        // TODO: Handle exception.
      }
    }

    // Writes to the destination stream.
    private void GetStreamWriteCallback(IAsyncResult ar)
    {
      try
      {
        RequestState requestState = (RequestState)ar.AsyncState;
        requestState.destination.EndWrite(ar);
        SendNextChunk(requestState);
      }
      catch (Exception ex)
      {
        // TODO: Handle exception.
      }
    }

    // Ends the operation, saves the request stream, and prepares the next chunk
    // to be sent.
    private void GetRequestStreamCallback(IAsyncResult ar)
    {
      try
      {
        RequestState requestState = (RequestState)ar.AsyncState;
        requestState.source = requestState.picture.GetImage();
        requestState.destination = requestState.request.EndGetRequestStream(ar);
        SendNextChunk(requestState);
      }
      catch (Exception ex)
      {
        // TODO: Handle exception.
      }
    }

    // Reads the response from the server.
    private void RespCallback(IAsyncResult ar)
    {
      try
      {
        RequestState requestState = (RequestState)ar.AsyncState;

        HttpWebResponse objHttpWebResponse = (HttpWebResponse)requestState.request.EndGetResponse(ar);
        Stream objStreamResponse = objHttpWebResponse.GetResponseStream();
        StreamReader objStreamReader = new StreamReader(objStreamResponse);

        // Response string may be used by developer.
        string responseString = objStreamReader.ReadToEnd();

        // Close the stream object
        objStreamResponse.Close();
        objStreamReader.Close();
        objHttpWebResponse.Close();

	// Notify the calling method that the upload has completed for this
	// picture.
        EndSend(requestState.picture);
      }
      catch (Exception ex)
      {
        // TODO: Handle exception.
      }
    }
  }
}
