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

namespace NonXnaUtils
{
    public struct Viewport
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float MinDepth { get; set; }
        public float MaxDepth { get; set; }

        public float AspectRatio
        {
            get
            {
                if ((Width == 0) || (Height == 0))
                {
                    return 0.0f;
                }
                else
                {
                    return ((float)Width / (float)Height);
                }
            }
        }

        public Viewport(int x, int y, int width, int height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            MinDepth = 0.0f;
            MaxDepth = 1.0f;
        }

        private static bool IsWithinEpsilon(float a, float b)
        {
            float num = a - b;
            return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
        }

        public Vector3 Project(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
            Matrix matrix = Matrix.Multiply(Matrix.Multiply(world, view), projection);
            Vector3 result = Vector3.Transform(source, matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
            
            if (!IsWithinEpsilon(a, 1.0f))
            {
                result = (Vector3)(result / a);
            }

            result.X = (((result.X + 1.0f) * 0.5f) * this.Width) + this.X;
            result.Y = (((-result.Y + 1.0f) * 0.5f) * this.Height) + this.Y;
            result.Z = (result.Z * (this.MaxDepth - this.MinDepth)) + this.MinDepth;

            return result;
        }

        public Vector3 Unproject(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
            Matrix matrix = Matrix.Invert(Matrix.Multiply(Matrix.Multiply(world, view), projection));

            source.X = (((source.X - this.X) / ((float)this.Width)) * 2f) - 1f;
            source.Y = -((((source.Y - this.Y) / ((float)this.Height)) * 2f) - 1f);
            source.Z = (source.Z - this.MinDepth) / (this.MaxDepth - this.MinDepth);
            Vector3 result = Vector3.Transform(source, matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
            if (!IsWithinEpsilon(a, 1f))
            {
                result = (Vector3)(result / a);
            }
            return result;
        }



    }
}
