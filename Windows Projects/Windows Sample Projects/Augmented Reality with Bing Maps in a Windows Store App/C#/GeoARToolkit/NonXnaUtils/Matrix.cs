
using System;
namespace NonXnaUtils
{
    public struct Matrix
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;

        public float M21;
        public float M22;
        public float M23;
        public float M24;

        public float M31;
        public float M32;
        public float M33;
        public float M34;
        
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            
            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }



        public static Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix result;

            // TODO: sanity checks here

            float fovRatio = 1.0f / ((float)Math.Tan((double)(fieldOfView * 0.5f)));

            result.M11 = fovRatio / aspectRatio;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = fovRatio;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M34 = -1.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
            result.M44 = 0.0f;

            return result;
        }

        public static Matrix CreateLookAt(Vector3 camPosition, Vector3 camTarget, Vector3 camUp)
        {
            Matrix result;

            Vector3 vector = Vector3.Normalize(camPosition - camTarget);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(camUp, vector));
            Vector3 vector3 = Vector3.Cross(vector, vector2);

            result.M11 = vector2.X;
            result.M12 = vector3.X;
            result.M13 = vector.X;
            result.M14 = 0.0f;

            result.M21 = vector2.Y;
            result.M22 = vector3.Y;
            result.M23 = vector.Y;
            result.M24 = 0.0f;

            result.M31 = vector2.Z;
            result.M32 = vector3.Z;
            result.M33 = vector.Z;
            result.M34 = 0.0f;

            result.M41 = -Vector3.Dot(vector2, camPosition);
            result.M42 = -Vector3.Dot(vector3, camPosition);
            result.M43 = -Vector3.Dot(vector, camPosition);
            result.M44 = 1.0f;

            return result;
        }


        public static Matrix CreateRotationX(float angleInRadians)
        {
            Matrix result;

            float cosine = (float)Math.Cos((double) angleInRadians);
            float sine = (float)Math.Sin((double) angleInRadians);

            result.M11 = 1.0f;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;
            
            result.M21 = 0.0f;
            result.M22 = cosine;
            result.M23 = sine;
            result.M24 = 0.0f;
            
            result.M31 = 0.0f;
            result.M32 = -sine;
            result.M33 = cosine;
            result.M34 = 0.0f;
            
            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;

            return result;
        }

        public static Matrix CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
        {
            Matrix result;
            Vector3 vector = Vector3.Normalize(-forward);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector));
            Vector3 vector3 = Vector3.Cross(vector, vector2);

            result.M11 = vector2.X;
            result.M12 = vector2.Y;
            result.M13 = vector2.Z;
            result.M14 = 0.0f;
            
            result.M21 = vector3.X;
            result.M22 = vector3.Y;
            result.M23 = vector3.Z;
            result.M24 = 0.0f;
            
            result.M31 = vector.X;
            result.M32 = vector.Y;
            result.M33 = vector.Z;
            result.M34 = 0.0f;
            
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
            result.M44 = 1.0f;

            return result;
        }


        public static Matrix Multiply(Matrix matrixA, Matrix matrixB)
        {
            Matrix result;

            result.M11 = (((matrixA.M11 * matrixB.M11) + (matrixA.M12 * matrixB.M21)) + (matrixA.M13 * matrixB.M31)) + (matrixA.M14 * matrixB.M41);
            result.M12 = (((matrixA.M11 * matrixB.M12) + (matrixA.M12 * matrixB.M22)) + (matrixA.M13 * matrixB.M32)) + (matrixA.M14 * matrixB.M42);
            result.M13 = (((matrixA.M11 * matrixB.M13) + (matrixA.M12 * matrixB.M23)) + (matrixA.M13 * matrixB.M33)) + (matrixA.M14 * matrixB.M43);
            result.M14 = (((matrixA.M11 * matrixB.M14) + (matrixA.M12 * matrixB.M24)) + (matrixA.M13 * matrixB.M34)) + (matrixA.M14 * matrixB.M44);

            result.M21 = (((matrixA.M21 * matrixB.M11) + (matrixA.M22 * matrixB.M21)) + (matrixA.M23 * matrixB.M31)) + (matrixA.M24 * matrixB.M41);
            result.M22 = (((matrixA.M21 * matrixB.M12) + (matrixA.M22 * matrixB.M22)) + (matrixA.M23 * matrixB.M32)) + (matrixA.M24 * matrixB.M42);
            result.M23 = (((matrixA.M21 * matrixB.M13) + (matrixA.M22 * matrixB.M23)) + (matrixA.M23 * matrixB.M33)) + (matrixA.M24 * matrixB.M43);
            result.M24 = (((matrixA.M21 * matrixB.M14) + (matrixA.M22 * matrixB.M24)) + (matrixA.M23 * matrixB.M34)) + (matrixA.M24 * matrixB.M44);

            result.M31 = (((matrixA.M31 * matrixB.M11) + (matrixA.M32 * matrixB.M21)) + (matrixA.M33 * matrixB.M31)) + (matrixA.M34 * matrixB.M41);
            result.M32 = (((matrixA.M31 * matrixB.M12) + (matrixA.M32 * matrixB.M22)) + (matrixA.M33 * matrixB.M32)) + (matrixA.M34 * matrixB.M42);
            result.M33 = (((matrixA.M31 * matrixB.M13) + (matrixA.M32 * matrixB.M23)) + (matrixA.M33 * matrixB.M33)) + (matrixA.M34 * matrixB.M43);
            result.M34 = (((matrixA.M31 * matrixB.M14) + (matrixA.M32 * matrixB.M24)) + (matrixA.M33 * matrixB.M34)) + (matrixA.M34 * matrixB.M44);

            result.M41 = (((matrixA.M41 * matrixB.M11) + (matrixA.M42 * matrixB.M21)) + (matrixA.M43 * matrixB.M31)) + (matrixA.M44 * matrixB.M41);
            result.M42 = (((matrixA.M41 * matrixB.M12) + (matrixA.M42 * matrixB.M22)) + (matrixA.M43 * matrixB.M32)) + (matrixA.M44 * matrixB.M42);
            result.M43 = (((matrixA.M41 * matrixB.M13) + (matrixA.M42 * matrixB.M23)) + (matrixA.M43 * matrixB.M33)) + (matrixA.M44 * matrixB.M43);
            result.M44 = (((matrixA.M41 * matrixB.M14) + (matrixA.M42 * matrixB.M24)) + (matrixA.M43 * matrixB.M34)) + (matrixA.M44 * matrixB.M44);

            return result;
        }

        public static Matrix Invert(Matrix source)
        {
            Matrix result;

            float num5 = source.M11;
            float num4 = source.M12;
            float num3 = source.M13;
            float num2 = source.M14;
            float num9 = source.M21;
            float num8 = source.M22;
            float num7 = source.M23;
            float num6 = source.M24;
            float num17 = source.M31;
            float num16 = source.M32;
            float num15 = source.M33;
            float num14 = source.M34;
            float num13 = source.M41;
            float num12 = source.M42;
            float num11 = source.M43;
            float num10 = source.M44;

            float num23 = (num15 * num10) - (num14 * num11);
            float num22 = (num16 * num10) - (num14 * num12);
            float num21 = (num16 * num11) - (num15 * num12);
            float num20 = (num17 * num10) - (num14 * num13);
            float num19 = (num17 * num11) - (num15 * num13);
            float num18 = (num17 * num12) - (num16 * num13);
            float num39 = ((num8 * num23) - (num7 * num22)) + (num6 * num21);
            float num38 = -(((num9 * num23) - (num7 * num20)) + (num6 * num19));
            float num37 = ((num9 * num22) - (num8 * num20)) + (num6 * num18);
            float num36 = -(((num9 * num21) - (num8 * num19)) + (num7 * num18));
            float num = 1.0f / ((((num5 * num39) + (num4 * num38)) + (num3 * num37)) + (num2 * num36));

            result.M11 = num39 * num;
            result.M21 = num38 * num;
            result.M31 = num37 * num;
            result.M41 = num36 * num;

            result.M12 = -(((num4 * num23) - (num3 * num22)) + (num2 * num21)) * num;
            result.M22 = (((num5 * num23) - (num3 * num20)) + (num2 * num19)) * num;
            result.M32 = -(((num5 * num22) - (num4 * num20)) + (num2 * num18)) * num;
            result.M42 = (((num5 * num21) - (num4 * num19)) + (num3 * num18)) * num;

            float num35 = (num7 * num10) - (num6 * num11);
            float num34 = (num8 * num10) - (num6 * num12);
            float num33 = (num8 * num11) - (num7 * num12);
            float num32 = (num9 * num10) - (num6 * num13);
            float num31 = (num9 * num11) - (num7 * num13);
            float num30 = (num9 * num12) - (num8 * num13);

            result.M13 = (((num4 * num35) - (num3 * num34)) + (num2 * num33)) * num;
            result.M23 = -(((num5 * num35) - (num3 * num32)) + (num2 * num31)) * num;
            result.M33 = (((num5 * num34) - (num4 * num32)) + (num2 * num30)) * num;
            result.M43 = -(((num5 * num33) - (num4 * num31)) + (num3 * num30)) * num;

            float num29 = (num7 * num14) - (num6 * num15);
            float num28 = (num8 * num14) - (num6 * num16);
            float num27 = (num8 * num15) - (num7 * num16);
            float num26 = (num9 * num14) - (num6 * num17);
            float num25 = (num9 * num15) - (num7 * num17);
            float num24 = (num9 * num16) - (num8 * num17);

            result.M14 = -(((num4 * num29) - (num3 * num28)) + (num2 * num27)) * num;
            result.M24 = (((num5 * num29) - (num3 * num26)) + (num2 * num25)) * num;
            result.M34 = -(((num5 * num28) - (num4 * num26)) + (num2 * num24)) * num;
            result.M44 = (((num5 * num27) - (num4 * num25)) + (num3 * num24)) * num;

            return result;
        }

        public static Matrix operator *(Matrix matrixA, Matrix matrixB)
        {
            Matrix result;

            result.M11 = (((matrixA.M11 * matrixB.M11) + (matrixA.M12 * matrixB.M21)) + (matrixA.M13 * matrixB.M31)) + (matrixA.M14 * matrixB.M41);
            result.M12 = (((matrixA.M11 * matrixB.M12) + (matrixA.M12 * matrixB.M22)) + (matrixA.M13 * matrixB.M32)) + (matrixA.M14 * matrixB.M42);
            result.M13 = (((matrixA.M11 * matrixB.M13) + (matrixA.M12 * matrixB.M23)) + (matrixA.M13 * matrixB.M33)) + (matrixA.M14 * matrixB.M43);
            result.M14 = (((matrixA.M11 * matrixB.M14) + (matrixA.M12 * matrixB.M24)) + (matrixA.M13 * matrixB.M34)) + (matrixA.M14 * matrixB.M44);
            
            result.M21 = (((matrixA.M21 * matrixB.M11) + (matrixA.M22 * matrixB.M21)) + (matrixA.M23 * matrixB.M31)) + (matrixA.M24 * matrixB.M41);
            result.M22 = (((matrixA.M21 * matrixB.M12) + (matrixA.M22 * matrixB.M22)) + (matrixA.M23 * matrixB.M32)) + (matrixA.M24 * matrixB.M42);
            result.M23 = (((matrixA.M21 * matrixB.M13) + (matrixA.M22 * matrixB.M23)) + (matrixA.M23 * matrixB.M33)) + (matrixA.M24 * matrixB.M43);
            result.M24 = (((matrixA.M21 * matrixB.M14) + (matrixA.M22 * matrixB.M24)) + (matrixA.M23 * matrixB.M34)) + (matrixA.M24 * matrixB.M44);
            
            result.M31 = (((matrixA.M31 * matrixB.M11) + (matrixA.M32 * matrixB.M21)) + (matrixA.M33 * matrixB.M31)) + (matrixA.M34 * matrixB.M41);
            result.M32 = (((matrixA.M31 * matrixB.M12) + (matrixA.M32 * matrixB.M22)) + (matrixA.M33 * matrixB.M32)) + (matrixA.M34 * matrixB.M42);
            result.M33 = (((matrixA.M31 * matrixB.M13) + (matrixA.M32 * matrixB.M23)) + (matrixA.M33 * matrixB.M33)) + (matrixA.M34 * matrixB.M43);
            result.M34 = (((matrixA.M31 * matrixB.M14) + (matrixA.M32 * matrixB.M24)) + (matrixA.M33 * matrixB.M34)) + (matrixA.M34 * matrixB.M44);
            
            result.M41 = (((matrixA.M41 * matrixB.M11) + (matrixA.M42 * matrixB.M21)) + (matrixA.M43 * matrixB.M31)) + (matrixA.M44 * matrixB.M41);
            result.M42 = (((matrixA.M41 * matrixB.M12) + (matrixA.M42 * matrixB.M22)) + (matrixA.M43 * matrixB.M32)) + (matrixA.M44 * matrixB.M42);
            result.M43 = (((matrixA.M41 * matrixB.M13) + (matrixA.M42 * matrixB.M23)) + (matrixA.M43 * matrixB.M33)) + (matrixA.M44 * matrixB.M43);
            result.M44 = (((matrixA.M41 * matrixB.M14) + (matrixA.M42 * matrixB.M24)) + (matrixA.M43 * matrixB.M34)) + (matrixA.M44 * matrixB.M44);
            
            return result;
        }


        public static Matrix CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Matrix matrix;
            Quaternion quaternion;
            Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
            CreateFromQuaternion(ref quaternion, out matrix);
            return matrix;
        }

        public static Matrix CreateFromQuaternion(Quaternion quaternion)
        {
            Matrix matrix;
            float num9 = quaternion.X * quaternion.X;
            float num8 = quaternion.Y * quaternion.Y;
            float num7 = quaternion.Z * quaternion.Z;
            float num6 = quaternion.X * quaternion.Y;
            float num5 = quaternion.Z * quaternion.W;
            float num4 = quaternion.Z * quaternion.X;
            float num3 = quaternion.Y * quaternion.W;
            float num2 = quaternion.Y * quaternion.Z;
            float num = quaternion.X * quaternion.W;
            matrix.M11 = 1f - (2f * (num8 + num7));
            matrix.M12 = 2f * (num6 + num5);
            matrix.M13 = 2f * (num4 - num3);
            matrix.M14 = 0f;
            matrix.M21 = 2f * (num6 - num5);
            matrix.M22 = 1f - (2f * (num7 + num9));
            matrix.M23 = 2f * (num2 + num);
            matrix.M24 = 0f;
            matrix.M31 = 2f * (num4 + num3);
            matrix.M32 = 2f * (num2 - num);
            matrix.M33 = 1f - (2f * (num8 + num9));
            matrix.M34 = 0f;
            matrix.M41 = 0f;
            matrix.M42 = 0f;
            matrix.M43 = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix result)
        {
            float num9 = quaternion.X * quaternion.X;
            float num8 = quaternion.Y * quaternion.Y;
            float num7 = quaternion.Z * quaternion.Z;
            float num6 = quaternion.X * quaternion.Y;
            float num5 = quaternion.Z * quaternion.W;
            float num4 = quaternion.Z * quaternion.X;
            float num3 = quaternion.Y * quaternion.W;
            float num2 = quaternion.Y * quaternion.Z;
            float num = quaternion.X * quaternion.W;
            result.M11 = 1f - (2f * (num8 + num7));
            result.M12 = 2f * (num6 + num5);
            result.M13 = 2f * (num4 - num3);
            result.M14 = 0f;
            result.M21 = 2f * (num6 - num5);
            result.M22 = 1f - (2f * (num7 + num9));
            result.M23 = 2f * (num2 + num);
            result.M24 = 0f;
            result.M31 = 2f * (num4 + num3);
            result.M32 = 2f * (num2 - num);
            result.M33 = 1f - (2f * (num8 + num9));
            result.M34 = 0f;
            result.M41 = 0f;
            result.M42 = 0f;
            result.M43 = 0f;
            result.M44 = 1f;
        }

    }
}
