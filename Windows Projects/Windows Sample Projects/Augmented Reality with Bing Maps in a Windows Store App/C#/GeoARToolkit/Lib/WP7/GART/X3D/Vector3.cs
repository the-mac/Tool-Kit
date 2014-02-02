using System;

namespace GART.X3D
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public static readonly Vector3 Zero = new Vector3();

        public static readonly Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
        public static readonly Vector3 Down = new Vector3(0.0f, -1.0f, 0.0f);

        public static readonly Vector3 Left = new Vector3(-1.0f, 0.0f, 0.0f);
        public static readonly Vector3 Right = new Vector3(1.0f, 0.0f, 0.0f);

        public static readonly Vector3 Forward = new Vector3(0.0f, 0.0f, -1.0f);
        public static readonly Vector3 Backward = new Vector3(0.0f, 0.0f, 1.0f);

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float LengthSquared()
        {
            return (X * X + Y * Y + Z * Z);
        }

        public float Length()
        {
            return (float)Math.Sqrt((double)LengthSquared());
        }

        /// <summary>
        /// Performs vector normalization process, so at the end
        /// vector's len = 1.0f; Note, does not support zero vectors
        /// </summary>
        public void Normalize()
        {
            float factor = 1.0f / Length();
            X = X * factor;
            Y = Y * factor;
            Z = Z * factor;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj)) return true;
            if (obj is Vector3)
            {
                return this == (Vector3)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // HACK: Is this the right way to compute a haschode for three floats?
            // I don't like casting to int, but the ^ operator does not work on float.
            return ((int)X) ^ ((int)Y) ^ ((int)Z);
        }

        public static Vector3 Normalize(Vector3 value)
        {
            Vector3 result;
            float factor  = 1.0f / value.Length();
            result.X = value.X * factor;
            result.Y = value.Y * factor;
            result.Z = value.Z * factor;
            return result;
        }

        public static Vector3 operator -(Vector3 vector)
        {
            Vector3 result;

            result.X = -vector.X;
            result.Y = -vector.Y;
            result.Z = -vector.Z;

            return result;
        }

        /// <summary>
        /// Subtracts vectorB from vectorA
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 vectorA, Vector3 vectorB)
        {
            Vector3 result;

            result.X = vectorA.X - vectorB.X;
            result.Y = vectorA.Y - vectorB.Y;
            result.Z = vectorA.Z - vectorB.Z;

            return result;
        }

        public static Vector3 operator +(Vector3 vectorA, Vector3 vectorB)
        {
            Vector3 result;

            result.X = vectorA.X + vectorB.X;
            result.Y = vectorA.Y + vectorB.Y;
            result.Z = vectorA.Z + vectorB.Z;

            return result;
        }

        public static Vector3 operator *(Vector3 vector, float scaleFactor)
        {
            Vector3 result;

            result.X = vector.X * scaleFactor;
            result.Y = vector.Y * scaleFactor;
            result.Z = vector.Z * scaleFactor;

            return result;
        }

        public static Vector3 operator *(float scaleFactor, Vector3 vector)
        {
            return (vector * scaleFactor);
        }

        public static Vector3 operator /(Vector3 vector, float divider)
        {
            Vector3 result;
            float invertedScale = 1.0f / divider;

            result = vector * invertedScale;

            return result;
        }

        public static Vector3 operator /(Vector3 vectorA, Vector3 vectorB)
        {
            Vector3 result;

            result.X = vectorA.X / vectorB.X;
            result.Y = vectorA.Y / vectorB.Y;
            result.Z = vectorA.Z / vectorB.Z;

            return result;
        }

        public static bool operator ==(Vector3 vectorA, Vector3 vectorB)
        {
            return ((vectorA.X == vectorB.X) && (vectorA.Y == vectorB.Y) && (vectorA.Z == vectorB.Z));
        }

        public static bool operator !=(Vector3 vectorA, Vector3 vectorB)
        {
            return !((vectorA.X == vectorB.X) && (vectorA.Y == vectorB.Y) && (vectorA.Z == vectorB.Z));
        }

        public static Vector3 Cross(Vector3 vectorA, Vector3 vectorB)
        {
            Vector3 result;

            result.X = (vectorA.Y * vectorB.Z) - (vectorA.Z * vectorB.Y);
            result.Y = (vectorA.Z * vectorB.X) - (vectorA.X * vectorB.Z);
            result.Z = (vectorA.X * vectorB.Y) - (vectorA.Y * vectorB.X);

            return result;
        }

        public static float Dot(Vector3 vectorA, Vector3 vectorB)
        {
            float result;

            result = ((vectorA.X * vectorB.X) + (vectorA.Y * vectorB.Y)) + (vectorA.Z * vectorB.Z);

            return result;
        }

        public static Vector3 Transform(Vector3 position, Matrix matrix)
        {
            Vector3 result;

            result.X = (((position.X * matrix.M11) + (position.Y * matrix.M21)) + (position.Z * matrix.M31)) + matrix.M41;
            result.Y = (((position.X * matrix.M12) + (position.Y * matrix.M22)) + (position.Z * matrix.M32)) + matrix.M42;
            result.Z = (((position.X * matrix.M13) + (position.Y * matrix.M23)) + (position.Z * matrix.M33)) + matrix.M43;

            return result;
        }


    }

}
