using CustomMath;
using System;
using UnityEngine;
public struct Quat
{
    public float x;
    public float y;
    public float z;
    public float w;

    private const float epsilon = 1E-05f;

    #region Constructors

    public Quat(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Quat(Quat quat)
    {
        this.x = quat.x;
        this.y = quat.y;
        this.z = quat.z;
        this.w = quat.w;
    }

    #endregion

    public Quat(Quaternion unityQuat)
    {
        this.x = unityQuat.x;
        this.y = unityQuat.y;
        this.z = unityQuat.z;
        this.w = unityQuat.w;
    }

    #region Operators

    public static bool operator ==(Quat lhs, Quat rhs)
    {
        float xDiff = lhs.x - rhs.x;
        float yDiff = lhs.y - rhs.y;
        float zDiff = lhs.z - rhs.z;
        float wDiff = lhs.w - rhs.w;

        return xDiff * xDiff + yDiff * yDiff + zDiff * zDiff + wDiff * wDiff < epsilon * epsilon;
    }

    public static bool operator !=(Quat lhs, Quat rhs)
    {
        return !(lhs == rhs);
    }

    public static Vec3 operator *(Quat rotation, Vec3 point)
    {
        Quat normalizedRotation = rotation.normalized;

        Quat pureRotationQuat = new Quat(point.x, point.y, point.z, 0f);
        Quat invertedRotation = new Quat(-normalizedRotation.x, -normalizedRotation.y, -normalizedRotation.z, normalizedRotation.w);
        Quat resultCuat = normalizedRotation * pureRotationQuat * invertedRotation;

        return new Vec3(resultCuat.x, resultCuat.y, resultCuat.z);
    }

    public static Quat operator *(Quat q1, Quat q2)
    {
        float newX = (q1.w * q2.x) + (q1.x * q2.w) + (q1.y * q2.z) - (q1.z * q2.y);
        float newY = (q1.w * q2.y) - (q1.x * q2.z) + (q1.y * q2.w) + (q1.z * q2.x);
        float newZ = (q1.w * q2.z) + (q1.x * q2.y) - (q1.y * q2.x) + (q1.z * q2.w);
        float newW = (q1.w * q2.w) - (q1.x * q2.x) - (q1.y * q2.y) - (q1.z * q2.z);

        return new Quat(newX, newY, newZ, newW);
    }

    public static implicit operator Quaternion(Quat myQuat)
    {
        return new Quaternion(myQuat.x, myQuat.y, myQuat.z, myQuat.w);
    }

    public static implicit operator Quat(Quaternion unityQuat)
    {
        return new Quat(unityQuat);
    }

    #endregion

    #region Properties


    public static Quat identity
    {
        get
        {
            return new Quat(0f, 0f, 0f, 1f);
        }
    }

    public Vec3 eulerAngles
    {
        get
        {
            Vec3 rotationZ = this * new Vec3(0f, 0f, 1f);
            Vec3 rotationX = this * new Vec3(1f, 0f, 0f);
            Vec3 rotationY = this * new Vec3(0f, 1f, 0f);

            float m00 = rotationX.x; float m01 = rotationY.x; float m02 = rotationZ.x;
            float m10 = rotationX.y; float m11 = rotationY.y; float m12 = rotationZ.y;
            float m20 = rotationX.z; float m21 = rotationY.z; float m22 = rotationZ.z;

            Vec3 angles = new Vec3(-Mathf.Asin(m12), Mathf.Atan2(m02, m22), Mathf.Atan2(m10, m11));

            angles.x *= Mathf.Rad2Deg;
            angles.y *= Mathf.Rad2Deg;
            angles.z *= Mathf.Rad2Deg;

            return angles;
        }
    }


    public Quat normalized
    {
        get
        {
            if (this.sqrMagnitude < epsilon * epsilon)
            {
                return Quat.identity;
            }

            Quat normalizedQuat = this;

            float magnitude = this.magnitude;

            normalizedQuat.x /= magnitude;
            normalizedQuat.y /= magnitude;
            normalizedQuat.z /= magnitude;
            normalizedQuat.w /= magnitude;

            return normalizedQuat;
        }
    }

    public float magnitude
    {
        get
        {
            return Mathf.Sqrt(this.sqrMagnitude);
        }
    }

    public float sqrMagnitude
    {
        get
        {
            return this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
        }
    }

    #endregion

    #region Static

    public static float Angle(Quat a, Quat b)
    {
        return Mathf.Acos(Mathf.Abs(Dot(a.normalized, b.normalized))) * 2f;
    }

    public static Quat AngleAxis(float angle, Vec3 axis)
    {
        float halfAngleRadians = angle * 0.5f * Mathf.Deg2Rad;

        Vec3 normAxis = axis.normalized;

        return new Quat(normAxis.x * Mathf.Sin(halfAngleRadians), normAxis.y * Mathf.Sin(halfAngleRadians), normAxis.z * Mathf.Sin(halfAngleRadians), Mathf.Cos(halfAngleRadians));
    }

    public static float Dot(Quat a, Quat b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    }

    public static Quat Euler(Vec3 euler)
    {
        return EulerRotation(euler);
    }
    public static Quat EulerAngles(Vec3 euler)
    {
        return EulerRotation(euler);
    }
    public static Quat EulerRotation(Vec3 euler)
    {
        Vec3 halfEulerRad = new Vec3(euler.x * 0.5f * Mathf.Deg2Rad, euler.y * 0.5f * Mathf.Deg2Rad, euler.z * 0.5f * Mathf.Deg2Rad);

        Quat rotationX = new Quat(Mathf.Sin(halfEulerRad.x), 0f, 0f, Mathf.Cos(halfEulerRad.x));
        Quat rotationY = new Quat(0f, Mathf.Sin(halfEulerRad.y), 0f, Mathf.Cos(halfEulerRad.y));
        Quat rotationZ = new Quat(0f, 0f, Mathf.Sin(halfEulerRad.z), Mathf.Cos(halfEulerRad.z));

        return rotationZ * rotationX * rotationY;
    }

    public static Quat FromToRotation(Vec3 fromDirection, Vec3 toDirection)
    {
        Vec3 normalizedFrom = fromDirection.normalized;
        Vec3 normalizedTo = toDirection.normalized;
        Vec3 axis;

        float dot = Vec3.Dot(normalizedFrom, normalizedTo);

        if (dot >= 1f - epsilon)
        {
            return Quat.identity;
        }

        if (dot <= -1 + epsilon)
        {
            axis = Vec3.Cross(normalizedFrom, Vec3.Left);
            return Quat.AngleAxis(180f, axis);
        }

        axis = Vec3.Cross(normalizedFrom, normalizedTo);
        axis.Normalize();

        float angle = Vec3.Angle(normalizedFrom, normalizedTo);

        return Quat.AngleAxis(angle, axis);
    }

    public static Quat Inverse(Quat rotation)
    {
        return new Quat(-rotation.x, -rotation.y, -rotation.z, rotation.w);
    }

    public static Quat Lerp(Quat a, Quat b, float t)
    {
        t = Mathf.Clamp01(t);

        return Quat.LerpUnclamped(a, b, t);
    }

    public static Quat LerpUnclamped(Quat a, Quat b, float t)
    {
        Vector4 aVec = new Vector4(a.x, a.y, a.z, a.w);
        Vector4 bVec = new Vector4(b.x, b.y, b.z, b.w);
        Vector4 result = aVec + (bVec - aVec) * t;

        return new Quat(result.x, result.y, result.z, result.w).normalized;
    }

    public static Quat LookRotation(Vec3 forward, Vec3 upwards)
    {
        forward.Normalize();

        Vec3 right = Vec3.Cross(forward, upwards);
        right.Normalize();

        upwards = Vec3.Cross(forward, right);
        upwards.Normalize();

        float m00 = right.x; float m01 = upwards.x; float m02 = forward.x;
        float m10 = right.y; float m11 = upwards.y; float m12 = forward.y;
        float m20 = right.z; float m21 = upwards.z; float m22 = forward.z;

        float trace = m00 + m11 + m22;


        Quat q = new Quat();

        if (trace > 0f)
        {
            float scale = Mathf.Sqrt(trace + 1f) * 0.5f;
            q.w = scale;

            //devuelven un vector que apunta en la dirección del eje sobre el cual está rotando el objeto
            q.x = (m21 - m12) / (scale * 4);
            q.y = (m02 - m20) / (scale * 4);
            q.z = (m10 - m01) / (scale * 4);

            return q.normalized;
        }
        else if (m00 >= m11 && m00 >= m22)
        {
            float scale = Mathf.Sqrt(1f + m00 - m11 - m22);
            q.w = (m21 - m12) / (scale * 2f);
            q.x = scale / 2f;
            q.y = (m01 + m10) / (scale * 2f);
            q.z = (m02 + m20) / (scale * 2f);
        }
        else if (m11 >= m22)
        {
            float scale = Mathf.Sqrt(1f + m11 - m00 - m22);
            q.w = (m02 - m20) / (scale * 2f);
            q.x = (m01 + m10) / (scale * 2f);
            q.y = scale / 2f;
            q.z = (m12 + m21) / (scale * 2f);
        }
        else
        {
            float scale = Mathf.Sqrt(1f + m22 - m00 - m11);
            q.w = (m10 - m01) / (scale * 2f);
            q.x = (m02 + m20) / (scale * 2f);
            q.y = (m12 + m21) / (scale * 2f);
            q.z = scale / 2f;
        }

        return q.normalized;
    }

    public static Quat Normalize(Quat q)
    {
        return q.normalized;
    }

    public static Quat RotateTowards(Quat from, Quat to, float maxDegreesDelta)
    {
        float angle = Quat.Angle(from, to);

        if (angle <= epsilon || maxDegreesDelta > angle)
        {
            return to;
        }

        return Quat.Slerp(from, to, maxDegreesDelta / angle);
    }

    public static Quat Slerp(Quat a, Quat b, float t)
    {
        t = Mathf.Clamp01(t);

        return SlerpUnclamped(a, b, t);
    }

    public static Quat SlerpUnclamped(Quat a, Quat b, float t)
    {
        a.Normalize();
        b.Normalize();

        float dot = Quat.Dot(a, b);

        if (dot < 0)
        {
            dot = -dot;
            b = new Quat(-b.x, -b.y, -b.z, -b.w);
        }

        if (dot > 0.995f)
        {
            return Quat.Lerp(a, b, t);
        }

        float angle = MathF.Acos(dot);
        float angleSin = MathF.Sin(angle);

        float weightA = Mathf.Sin((1f - t) * angle) / angleSin;
        float weightB = Mathf.Sin(t * angle) / angleSin;

        Quat q = new Quat();
        q.x = weightA * a.x + weightB * b.x;
        q.y = weightA * a.y + weightB * b.y;
        q.z = weightA * a.z + weightB * b.z;
        q.w = weightA * a.w + weightB * b.w;

        return q;
    }

    public static Vec3 ToEulerAngles(Quat rotation)
    {
        return rotation.eulerAngles;
    }

    #endregion

    #region Methods

    public void Normalize()
    {
        this = this.normalized;
    }

    public void Set(float newX, float newY, float newZ, float newW)
    {
        x = newX;
        y = newY;
        z = newZ;
        w = newW;
    }

    public void SetAxisAngle(Vec3 axis, float angle)
    {
        this = Quat.AngleAxis(angle, axis);
    }

    public void SetEulerAngles(Vec3 euler)
    {
        this = Quat.Euler(euler);
    }

    public void SetEulerRotation(Vec3 euler)
    {
        this = Quat.Euler(euler);
    }

    public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
    {
        this = Quat.FromToRotation(fromDirection, toDirection);
    }

    public void SetLookRotation(Vec3 view, Vec3 up)
    {
        this = Quat.LookRotation(view, up);
    }

    public void ToAngleAxis(out float angle, out Vec3 axis)
    {
        Quat q = this;

        if (q.w < 0f)
        {
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }

        angle = 2f * MathF.Acos(q.w) * Mathf.Rad2Deg;

        // calcula el divisor necesario para sacar la escala del eje.
        // sin(theta/2) equivale a la raíz cuadrada de (1 - w^2)
        float den = MathF.Sqrt(1f - q.w * q.w);

        if (den < epsilon)
        {
            axis = Vec3.Up;
        }
        else
        {
            axis = new Vec3(q.x / den, q.y / den, q.z / den);
        }
    }

    public Vec3 ToEuler()
    {
        return this.eulerAngles;
    }


    #endregion
}
