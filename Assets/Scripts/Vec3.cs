using System;

using UnityEngine;

namespace CustomMath
{
    public struct Vec3 : IEquatable<Vec3>
    {
        public float x;

        public float y;

        public float z;

        public const float epsilon = 1E-05f;

        public const float sqrEpsilon = epsilon * epsilon;

        public const float RAD2DEG = 57.29578f;
        public readonly float sqrMagnitude => x * x + y * y + z * z;

        public readonly Vec3 normalized
        {
            get
            {
                float vecMangitude = magnitude;
                if (vecMangitude < epsilon)
                {
                    return Zero;
                }
                return new Vec3(x / vecMangitude, y / vecMangitude, z / vecMangitude);
            }
        }

        public readonly float magnitude => MathF.Sqrt(x * x + y * y + z * z);

        public static Vec3 Zero => new Vec3(0f, 0f, 0f);

        public static Vec3 One => new Vec3(1f, 1f, 1f);

        public static Vec3 Forward => new Vec3(0f, 0f, 1f);

        public static Vec3 Back => new Vec3(0f, 0f, -1f);

        public static Vec3 Right => new Vec3(1f, 0f, 0f);

        public static Vec3 Left => new Vec3(-1f, 0f, 0f);

        public static Vec3 Up => new Vec3(0f, 1f, 0f);

        public static Vec3 Down => new Vec3(0f, -1f, 0f);

        public static Vec3 PositiveInfinity => new Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        public static Vec3 NegativeInfinity => new Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public Vec3(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
        }

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3(Vec3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        public Vec3(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        public Vec3(Vector2 v2)
        {
            x = v2.x;
            y = v2.y;
            z = 0f;
        }

        public static bool operator ==(Vec3 left, Vec3 right)
        {
            float num = left.x - right.x;
            float num2 = left.y - right.y;
            float num3 = left.z - right.z;
            return num * num + num2 * num2 + num3 * num3 < sqrEpsilon;
        }

        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return !(left == right);
        }

        public static implicit operator Vector3(Vec3 myVec)
        {
            return new Vector3(myVec.x, myVec.y, myVec.z);
        }

        public static implicit operator Vec3(Vector3 unityVec)
        {
            return new Vec3(unityVec);
        }

        public static Vec3 operator +(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x + rightV3.x, leftV3.y + rightV3.y, leftV3.z + rightV3.z);
        }

        public static Vec3 operator -(Vec3 leftV3, Vec3 rightV3)
        {
            return new Vec3(leftV3.x - rightV3.x, leftV3.y - rightV3.y, leftV3.z - rightV3.z);
        }

        public static Vec3 operator -(Vec3 v3)
        {
            return new Vec3(-v3.x, -v3.y, -v3.z);
        }

        public static Vec3 operator *(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x * scalar, v3.y * scalar, v3.z * scalar);
        }

        public static Vec3 operator *(float scalar, Vec3 v3)
        {
            return new Vec3(v3 * scalar);
        }

        public static Vec3 operator /(Vec3 v3, float scalar)
        {
            return new Vec3(v3.x / scalar, v3.y / scalar, v3.z / scalar);
        }

        public static implicit operator Vector2(Vec3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public override string ToString()
        {
            return "(" + x.ToString("F2") + ", " + y.ToString("F2") + ", " + z.ToString("F2") + ")";
        }

        public static float Angle(Vec3 from, Vec3 to)
        {
            return RAD2DEG * MathF.Acos(Dot(from.normalized, to.normalized));
        }

        public static Vec3 ClampMagnitude(Vec3 vector, float maxLength)
        {
            float sqrMagnitude = vector.sqrMagnitude;

            if (maxLength * maxLength > sqrMagnitude)
            {
                return vector;
            }

            float magnitude = MathF.Sqrt(sqrMagnitude);
            float scalar = maxLength / magnitude;

            return vector * scalar;
        }

        public static float Magnitude(Vec3 vector)
        {
            return MathF.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            return new Vec3(a.y * b.z - a.z * b.y, (a.x * b.z - a.z * b.x), a.x * b.y - a.y * b.x);
        }

        public static float Distance(Vec3 a, Vec3 b)
        {
            return (b - a).magnitude;
        }

        public static float Dot(Vec3 a, Vec3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            float clampedT = MathF.Max(0f, MathF.Min(1f, t));
            return a + (b - a) * clampedT;
        }

        public static Vec3 LerpUnclamped(Vec3 a, Vec3 b, float t)
        {
            return a + (b - a) * t;
        }

        public static Vec3 Max(Vec3 a, Vec3 b)
        {
            return new Vec3(MathF.Max(a.x, b.x), MathF.Max(a.y, b.y), MathF.Max(a.z, b.z));
        }

        public static Vec3 Min(Vec3 a, Vec3 b)
        {
            return new Vec3(MathF.Min(a.x, b.x), MathF.Min(a.y, b.y), MathF.Min(a.z, b.z));
        }

        public static float SqrMagnitude(Vec3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

        public static Vec3 Project(Vec3 vector, Vec3 onNormal)
        {
            if (vector == onNormal)
            {
                return vector;
            }

            Vec3 normalizedOnNormal = onNormal.normalized;
            return normalizedOnNormal * Dot(vector, normalizedOnNormal);
        }

        public static Vec3 Reflect(Vec3 inDirection, Vec3 inNormal)
        {
            return inDirection + Dot(inDirection, inNormal) * -2f * inNormal;
        }

        public void Set(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        public void Scale(Vec3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        public void Normalize()
        {
            float num = magnitude;
            if (!(num < epsilon))
            {
                x /= num;
                y /= num;
                z /= num;
            }
        }

        public override bool Equals(object other)
        {
            if (!(other is Vec3))
            {
                return false;
            }
            return Equals((Vec3)other);
        }

        public bool Equals(Vec3 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
    }
}