using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhotoFun.Effects
{
    /// <summary>
    /// The blending mode.
    /// </summary>
    public enum BlendMode
    {
        /// <summary>
        /// Alpha blending uses the alpha channel to combine the source and destination. 
        /// </summary>
        Alpha,

        /// <summary>
        /// Additive blending adds the colors of the source and the destination.
        /// </summary>
        Additive,

        /// <summary>
        /// Subtractive blending subtracts the source color from the destination.
        /// </summary>
        Subtractive,

        /// <summary>
        /// Uses the source color as a mask.
        /// </summary>
        Mask,

        /// <summary>
        /// Multiplies the source color with the destination color.
        /// </summary>
        Multiply,

        /// <summary>
        /// Ignores the specified Color
        /// </summary>
        ColorKeying,

        /// <summary>
        /// No blending just copies the pixels from the source.
        /// </summary>
        None
    }
}
