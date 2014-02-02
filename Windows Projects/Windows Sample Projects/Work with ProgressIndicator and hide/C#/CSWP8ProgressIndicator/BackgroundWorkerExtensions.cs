/****************************** Module Header ******************************\
* Module Name:    BackgroundWorkerExtensions.cs
* Project:        CSWP8ProgressIndicator
* Copyright (c) Microsoft Corporation
*
* This class is the custom extension for BackgroundWorker.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWP8ProgressIndicator
{
    public static class BackgroundWorkerExtensions
    {
        public static Task<object> RunWorkerTaskAsync(this BackgroundWorker backgroundWorker)
        {
            var tcs = new TaskCompletionSource<object>();
            RunWorkerCompletedEventHandler handler = null;
            handler = (sender, args) =>
            {
                if (args.Cancelled)
                    tcs.TrySetCanceled();
                else if (args.Error != null)
                    tcs.TrySetException(args.Error);
                else
                    tcs.TrySetResult(args.Result);
            };
            backgroundWorker.RunWorkerCompleted += handler;
            try
            {
                backgroundWorker.RunWorkerAsync();
            }
            catch
            {
                backgroundWorker.RunWorkerCompleted -= handler;
                throw;
            }
            return tcs.Task;
        }
    }

}
