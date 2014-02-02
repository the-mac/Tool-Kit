using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Devices;

namespace PhotoFun.Effects
{
    /// <summary>
    /// This interface is for image processing effect 
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// When implemented, takes the source array of pixels and returns
        /// the array after processing
        /// </summary>
        /// <param name="source">Source image as array of pixels</param>
        /// <returns>>Processed image as array of pixels</returns>
        int[] ProcessImage(int[] source);
    }

  
}
