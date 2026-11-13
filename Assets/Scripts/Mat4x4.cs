using CustomMath;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class Mat4x4
{
    public float m00; public float m01; public float m02; public float m03;
    public float m10; public float m11; public float m12; public float m13;
    public float m20; public float m21; public float m22; public float m23;
    public float m30; public float m31; public float m32; public float m33;

    private const float epsilon = 1E-05f;

    #region Constructors

    public Mat4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
    {
        m00 = column0.x;
        m01 = column1.x;
        m02 = column2.x;
        m03 = column3.x;

        m10 = column0.y;
        m11 = column1.y;
        m12 = column2.y;
        m13 = column3.y;

        m20 = column0.z;
        m21 = column1.z;
        m22 = column2.z;
        m23 = column3.z;

        m30 = column0.w;
        m31 = column1.w;
        m32 = column2.w;
        m33 = column3.w;
    }

    public Mat4x4(Mat4x4 matrix)
    {
        this.m00 = matrix.m00;
        this.m01 = matrix.m01;
        this.m02 = matrix.m02;
        this.m03 = matrix.m03;

        this.m10 = matrix.m10;
        this.m11 = matrix.m11;
        this.m12 = matrix.m12;
        this.m13 = matrix.m13;

        this.m20 = matrix.m20;
        this.m21 = matrix.m21;
        this.m22 = matrix.m22;
        this.m23 = matrix.m23;

        this.m30 = matrix.m30;
        this.m31 = matrix.m31;
        this.m32 = matrix.m32;
        this.m33 = matrix.m33;
    }

    public Mat4x4(Matrix4x4 unityMatrix)
    {
        this.m00 = unityMatrix.m00;
        this.m01 = unityMatrix.m01;
        this.m02 = unityMatrix.m02;
        this.m03 = unityMatrix.m03;

        this.m10 = unityMatrix.m10;
        this.m11 = unityMatrix.m11;
        this.m12 = unityMatrix.m12;
        this.m13 = unityMatrix.m13;

        this.m20 = unityMatrix.m20;
        this.m21 = unityMatrix.m21;
        this.m22 = unityMatrix.m22;
        this.m23 = unityMatrix.m23;

        this.m30 = unityMatrix.m30;
        this.m31 = unityMatrix.m31;
        this.m32 = unityMatrix.m32;
        this.m33 = unityMatrix.m33;
    }

    #endregion

    #region Properties

    public static Mat4x4 zero
    {
        get
        {
            return new Mat4x4(new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f));
        }
    }

    public static Mat4x4 identity
    {
        get
        {
            return new Mat4x4(
                new Vector4(1f, 0f, 0f, 0f),
                new Vector4(0f, 1f, 0f, 0f),
                new Vector4(0f, 0f, 1f, 0f),
                new Vector4(0f, 0f, 0f, 1f));
        }
    }


    //matriz adjunta
    public Mat4x4 inverse
    {
        get
        {
            float determinant = this.determinant;

            if (Mathf.Abs(determinant) < epsilon)
            {
                return this;
            }

            float det00 = Determinant3x3(new Vec3(m11, m21, m31), new Vec3(m12, m22, m32), new Vec3(m13, m23, m33));
            float det01 = Determinant3x3(new Vec3(m01, m21, m31), new Vec3(m02, m22, m32), new Vec3(m03, m23, m33)) * -1; //cofactores
            float det02 = Determinant3x3(new Vec3(m01, m11, m31), new Vec3(m02, m12, m32), new Vec3(m03, m13, m33));
            float det03 = Determinant3x3(new Vec3(m01, m11, m21), new Vec3(m02, m12, m22), new Vec3(m03, m13, m23)) * -1;

            float det10 = Determinant3x3(new Vec3(m10, m20, m30), new Vec3(m12, m22, m32), new Vec3(m13, m23, m33)) * -1;
            float det11 = Determinant3x3(new Vec3(m00, m20, m30), new Vec3(m02, m22, m32), new Vec3(m03, m23, m33));
            float det12 = Determinant3x3(new Vec3(m00, m10, m30), new Vec3(m02, m12, m32), new Vec3(m03, m13, m33)) * -1;
            float det13 = Determinant3x3(new Vec3(m00, m10, m20), new Vec3(m02, m12, m22), new Vec3(m03, m13, m23));

            float det20 = Determinant3x3(new Vec3(m10, m20, m30), new Vec3(m11, m21, m31), new Vec3(m13, m23, m33));
            float det21 = Determinant3x3(new Vec3(m00, m20, m30), new Vec3(m01, m21, m31), new Vec3(m03, m23, m33)) * -1;
            float det22 = Determinant3x3(new Vec3(m00, m10, m30), new Vec3(m01, m11, m31), new Vec3(m03, m13, m33));
            float det23 = Determinant3x3(new Vec3(m00, m10, m20), new Vec3(m01, m11, m21), new Vec3(m03, m13, m23)) * -1;

            float det30 = Determinant3x3(new Vec3(m10, m20, m30), new Vec3(m11, m21, m31), new Vec3(m12, m22, m32)) * -1;
            float det31 = Determinant3x3(new Vec3(m00, m20, m30), new Vec3(m01, m21, m31), new Vec3(m02, m22, m32));
            float det32 = Determinant3x3(new Vec3(m00, m10, m30), new Vec3(m01, m11, m31), new Vec3(m02, m12, m32)) * -1;
            float det33 = Determinant3x3(new Vec3(m00, m10, m20), new Vec3(m01, m11, m21), new Vec3(m02, m12, m22));
            //traspone

            return new Mat4x4(new Vector4(det00, det10, det20, det30) / determinant, new Vector4(det01, det11, det21, det31) / determinant, new Vector4(det02, det12, det22, det32) / determinant, new Vector4(det03, det13, det23, det33) / determinant);
        }
    }

    public float Determinant3x3(Vec3 col0, Vec3 col1, Vec3 col2)
    {
        float m00 = col0.x; float m01 = col1.x; float m02 = col2.x;
        float m10 = col0.y; float m11 = col1.y; float m12 = col2.y;
        float m20 = col0.z; float m21 = col1.z; float m22 = col2.z;

        float determinant = m00 * (m11 * m22 - m21 * m12) - m01 * (m10 * m22 - m12 * m20) + m02 * (m10 * m21 - m11 * m20);

        return determinant;
    }

    public float determinant
    {
        get
        {
            float det00 = Determinant3x3(new Vec3(m11, m21, m31), new Vec3(m12, m22, m32), new Vec3(m13, m23, m33));
            float det01 = Determinant3x3(new Vec3(m10, m20, m30), new Vec3(m12, m22, m32), new Vec3(m13, m23, m33));
            float det02 = Determinant3x3(new Vec3(m10, m20, m30), new Vec3(m11, m21, m31), new Vec3(m13, m23, m33));
            float det03 = Determinant3x3(new Vec3(m10, m20, m30), new Vec3(m11, m21, m31), new Vec3(m12, m22, m32));

            return m00 * det00 - m01 * det01 + m02 * det02 - m03 * det03;
        }
    }

    public bool isIdentity
    {
        get
        {
            return this == identity;
        }
    }

    public Quat rotation
    {
        get
        {
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
    }

    public Vec3 lossyScale
    {
        get
        {
            float scaleX = new Vec3(m00, m10, m20).magnitude;
            float scaleY = new Vec3(m01, m11, m21).magnitude;
            float scaleZ = new Vec3(m02, m12, m22).magnitude;

            return new Vec3(scaleX, scaleY, scaleZ);
        }
    }

    public Mat4x4 transpose
    {
        get
        {
            return new Mat4x4(
                new Vector4(m00, m01, m02, m03),
                new Vector4(m10, m11, m12, m13),
                new Vector4(m20, m21, m22, m23),
                new Vector4(m30, m31, m32, m33));
        }
    }

    #endregion

    #region Operators

    public static Vector4 operator *(Mat4x4 lhs, Vector4 vector)
    {
        return new Vector4(
            (lhs.m00 * vector.x) + (lhs.m01 * vector.y) + (lhs.m02 * vector.z) + (lhs.m03 * vector.w),
            (lhs.m10 * vector.x) + (lhs.m11 * vector.y) + (lhs.m12 * vector.z) + (lhs.m13 * vector.w),
            (lhs.m20 * vector.x) + (lhs.m21 * vector.y) + (lhs.m22 * vector.z) + (lhs.m23 * vector.w),
            (lhs.m30 * vector.x) + (lhs.m31 * vector.y) + (lhs.m32 * vector.z) + (lhs.m33 * vector.w));
    }

    public static Mat4x4 operator *(Mat4x4 lhs, Mat4x4 rhs)
    {
        float m00 = (lhs.m00 * rhs.m00) + (lhs.m01 * rhs.m10) + (lhs.m02 * rhs.m20) + (lhs.m03 * rhs.m30);
        float m01 = (lhs.m00 * rhs.m01) + (lhs.m01 * rhs.m11) + (lhs.m02 * rhs.m21) + (lhs.m03 * rhs.m31);
        float m02 = (lhs.m00 * rhs.m02) + (lhs.m01 * rhs.m12) + (lhs.m02 * rhs.m22) + (lhs.m03 * rhs.m32);
        float m03 = (lhs.m00 * rhs.m03) + (lhs.m01 * rhs.m13) + (lhs.m02 * rhs.m23) + (lhs.m03 * rhs.m33);

        float m10 = (lhs.m10 * rhs.m00) + (lhs.m11 * rhs.m10) + (lhs.m12 * rhs.m20) + (lhs.m13 * rhs.m30);
        float m11 = (lhs.m10 * rhs.m01) + (lhs.m11 * rhs.m11) + (lhs.m12 * rhs.m21) + (lhs.m13 * rhs.m31);
        float m12 = (lhs.m10 * rhs.m02) + (lhs.m11 * rhs.m12) + (lhs.m12 * rhs.m22) + (lhs.m13 * rhs.m32);
        float m13 = (lhs.m10 * rhs.m03) + (lhs.m11 * rhs.m13) + (lhs.m12 * rhs.m23) + (lhs.m13 * rhs.m33);

        float m20 = (lhs.m20 * rhs.m00) + (lhs.m21 * rhs.m10) + (lhs.m22 * rhs.m20) + (lhs.m23 * rhs.m30);
        float m21 = (lhs.m20 * rhs.m01) + (lhs.m21 * rhs.m11) + (lhs.m22 * rhs.m21) + (lhs.m23 * rhs.m31);
        float m22 = (lhs.m20 * rhs.m02) + (lhs.m21 * rhs.m12) + (lhs.m22 * rhs.m22) + (lhs.m23 * rhs.m32);
        float m23 = (lhs.m20 * rhs.m03) + (lhs.m21 * rhs.m13) + (lhs.m22 * rhs.m23) + (lhs.m23 * rhs.m33);

        float m30 = (lhs.m30 * rhs.m00) + (lhs.m31 * rhs.m10) + (lhs.m32 * rhs.m20) + (lhs.m33 * rhs.m30);
        float m31 = (lhs.m30 * rhs.m01) + (lhs.m31 * rhs.m11) + (lhs.m32 * rhs.m21) + (lhs.m33 * rhs.m31);
        float m32 = (lhs.m30 * rhs.m02) + (lhs.m31 * rhs.m12) + (lhs.m32 * rhs.m22) + (lhs.m33 * rhs.m32);
        float m33 = (lhs.m30 * rhs.m03) + (lhs.m31 * rhs.m13) + (lhs.m32 * rhs.m23) + (lhs.m33 * rhs.m33);

        return new Mat4x4(
            new Vector4(m00, m10, m20, m30),
            new Vector4(m01, m11, m21, m31),
            new Vector4(m02, m12, m22, m32),
            new Vector4(m03, m13, m23, m33));
    }

    public static bool operator ==(Mat4x4 lhs, Mat4x4 rhs)
    {
        float diffm00 = lhs.m00 - rhs.m00;
        float diffm01 = lhs.m01 - rhs.m01;
        float diffm02 = lhs.m02 - rhs.m02;
        float diffm03 = lhs.m03 - rhs.m03;

        float diffCol00 = diffm00 * diffm00 + diffm01 * diffm01 + diffm02 * diffm02 + diffm03 * diffm03;

        if (diffCol00 > epsilon * epsilon)
        {
            return false;
        }

        float diffm10 = lhs.m10 - rhs.m10;
        float diffm11 = lhs.m11 - rhs.m11;
        float diffm12 = lhs.m12 - rhs.m12;
        float diffm13 = lhs.m13 - rhs.m13;
        float diffCol01 = diffm10 * diffm10 + diffm11 * diffm11 + diffm12 * diffm12 + diffm13 * diffm13;

        if (diffCol01 > epsilon * epsilon)
        {
            return false;
        }

        float diffm20 = lhs.m20 - rhs.m20;
        float diffm21 = lhs.m21 - rhs.m21;
        float diffm22 = lhs.m22 - rhs.m22;
        float diffm23 = lhs.m23 - rhs.m23;
        float diffCol02 = diffm20 * diffm20 + diffm21 * diffm21 + diffm22 * diffm22 + diffm23 * diffm23;

        if (diffCol02 > epsilon * epsilon)
        {
            return false;
        }

        float diffm30 = lhs.m30 - rhs.m30;
        float diffm31 = lhs.m31 - rhs.m31;
        float diffm32 = lhs.m32 - rhs.m32;
        float diffm33 = lhs.m33 - rhs.m33;
        float diffCol03 = diffm30 * diffm30 + diffm31 * diffm31 + diffm32 * diffm32 + diffm33 * diffm33;

        return diffCol03 < epsilon * epsilon;
    }

    public static bool operator !=(Mat4x4 lhs, Mat4x4 rhs)
    {
        return !(lhs == rhs);
    }

    public static implicit operator Matrix4x4(Mat4x4 matrix)
    {
        return new Matrix4x4
            (
            new Vector4
            (
                matrix.m00,
                matrix.m10,
                matrix.m20,
                matrix.m30
            ),
            new Vector4
            (
                matrix.m01,
                matrix.m11,
                matrix.m21,
                matrix.m31
            ),
            new Vector4
            (
                matrix.m02,
                matrix.m12,
                matrix.m22,
                matrix.m32
            ),
            new Vector4
            (
                matrix.m03,
                matrix.m13,
                matrix.m23,
                matrix.m33
            )
            );
    }

    public static implicit operator Mat4x4(Matrix4x4 unityMatrix)
    {
        return new Mat4x4(unityMatrix);
    }

    #endregion

    #region Statics

    public static float Determinant(Mat4x4 m)
    {
        return m.determinant;
    }

    public static Mat4x4 Inverse(Mat4x4 m)
    {
        return m.inverse;
    }

    public static Mat4x4 LookAt(Vec3 from, Vec3 to, Vec3 up)
    {
        Vec3 forward = (to - from).normalized;

        return Mat4x4.TRS(from, Quat.LookRotation((to - from).normalized, up), Vec3.One);
    }

    public static Mat4x4 Rotate(Quat q)
    {
        Vec3 right = new Vec3(1f, 0f, 0f);
        Vec3 up = new Vec3(0f, 1f, 0f);
        Vec3 forward = new Vec3(0f, 0f, 1f);

        right = q * right;
        up = q * up;
        forward = q * forward;

        Mat4x4 mat = Mat4x4.identity;
        mat.SetColumn(0, new Vector4(right.x, right.y, right.z, 0f));
        mat.SetColumn(1, new Vector4(up.x, up.y, up.z, 0f));
        mat.SetColumn(2, new Vector4(forward.x, forward.y, forward.z, 0f));

        return mat;
    }

    public static Mat4x4 Scale(Vec3 vector)
    {
        Mat4x4 mat = Mat4x4.identity;
        mat.SetColumn(0, new Vector4(vector.x, 0f, 0f, 0f));
        mat.SetColumn(1, new Vector4(0f, vector.y, 0f, 0f));
        mat.SetColumn(2, new Vector4(0f, 0f, vector.z, 0f));

        return mat;
    }

    public static Mat4x4 Translate(Vec3 vector)
    {
        Mat4x4 mat = Mat4x4.identity;
        mat.SetColumn(3, new Vector4(vector.x, vector.y, vector.z, 1f));

        return mat;
    }

    public static Mat4x4 Transpose(Mat4x4 m)
    {
        return m.transpose;
    }

    public static Mat4x4 TRS(Vec3 pos, Quat q, Vec3 s)
    {
        Mat4x4 translation = Mat4x4.Translate(pos);
        Mat4x4 rotation = Mat4x4.Rotate(q);
        Mat4x4 scale = Mat4x4.Scale(s);

       return translation * rotation * scale;
    }

    #endregion

    #region Instance

    public Vec3 GetPosition()
    {
        Vector4 translation = this.GetColumn(3);

        return new Vec3(translation.x, translation.y, translation.z);
    }

    public Vector4 GetRow(int index)
    {
        Vector4 row = Vector4.zero;

        switch (index)
        {
            case 0:
                row = new Vector4(m00, m01, m02, m03);
                break;

            case 1:
                row = new Vector4(m10, m11, m12, m13);
                break;

            case 2:
                row = new Vector4(m20, m21, m22, m23);
                break;

            case 3:
                row = new Vector4(m30, m31, m32, m33);
                break;

            default:
                break;
        }

        return row;
    }

    public Vector4 GetColumn(int index)
    {
        Vector4 column = Vector4.zero;

        switch (index)
        {
            case 0:
                column = new Vector4(m00, m10, m20, m30);
                break;

            case 1:
                column = new Vector4(m01, m11, m21, m31);
                break;

            case 2:
                column = new Vector4(m02, m12, m22, m32);
                break;

            case 3:
                column = new Vector4(m03, m13, m23, m33);
                break;

            default:
                break;
        }

        return column;
    }

    public Vec3 MultiplyPoint(Vec3 point)
    {
        Vector4 transformedPoint = this * new Vector4(point.x, point.y, point.z, 1f);

        return new Vec3(transformedPoint.x, transformedPoint.y, transformedPoint.z);
    }

    public Vec3 MultiplyPoint3x4(Vec3 point)
    {
        float resX = point.x * m00 + point.y * m01 + point.z * m02 + m03;
        float resY = point.x * m10 + point.y * m11 + point.z * m12 + m13;
        float resZ = point.x * m20 + point.y * m21 + point.z * m22 + m23;

        return new Vec3(resX, resY, resZ);
    }

    public Vec3 MultiplyVector(Vec3 vector)
    {
        Vector4 vec = new Vector4(vector.x, vector.y, vector.z, 0f);
        vec = this * vec;

        return new Vec3(vec.x, vec.y, vec.z);
    }

    public void SetColumn(int index, Vector4 column)
    {
        switch (index)
        {
            case 0:
                m00 = column.x;
                m10 = column.y;
                m20 = column.z;
                m30 = column.w;
                break;

            case 1:
                m01 = column.x;
                m11 = column.y;
                m21 = column.z;
                m31 = column.w;
                break;

            case 2:
                m02 = column.x;
                m12 = column.y;
                m22 = column.z;
                m32 = column.w;
                break;

            case 3:
                m03 = column.x;
                m13 = column.y;
                m23 = column.z;
                m33 = column.w;
                break;

            default:
                break;
        }
    }

    public void SetRow(int index, Vector4 row)
    {
        switch (index)
        {
            case 0:
                m00 = row.x;
                m01 = row.y;
                m02 = row.z;
                m03 = row.w;
                break;

            case 1:
                m10 = row.x;
                m11 = row.y;
                m12 = row.z;
                m13 = row.w;
                break;

            case 2:
                m20 = row.x;
                m21 = row.y;
                m22 = row.z;
                m23 = row.w;
                break;

            case 3:
                m30 = row.x;
                m31 = row.y;
                m32 = row.z;
                m33 = row.w;
                break;

            default:
                break;
        }
    }

    public void SetTRS(Vec3 pos, Quat q, Vec3 s)
    {
        Mat4x4 trs = Mat4x4.TRS(pos, q, s);

        this.m00 = trs.m00;
        this.m01 = trs.m01;
        this.m02 = trs.m02;
        this.m03 = trs.m03;
        this.m10 = trs.m10;
        this.m11 = trs.m11;
        this.m12 = trs.m12;
        this.m13 = trs.m13;
        this.m20 = trs.m20;
        this.m21 = trs.m21;
        this.m22 = trs.m22;
        this.m23 = trs.m23;
        this.m30 = trs.m30;
        this.m31 = trs.m31;
        this.m32 = trs.m32;
        this.m33 = trs.m33;
    }

    public bool ValidTRS()
    {
        if (m30 != 0f || m31 != 0f || m32 != 0f || m33 != 1f)
        {
            return false;
        }

        Vec3 right = new Vec3(m00, m10, m20);
        Vec3 up = new Vec3(m01, m11, m21);
        Vec3 forward = new Vec3(m02, m12, m22);

        if (Mathf.Abs(Vec3.Dot(right, up)) > epsilon || Mathf.Abs(Vec3.Dot(right, forward)) > epsilon || Mathf.Abs(Vec3.Dot(up, forward)) > epsilon)
        {
            return false;
        }

        return true;
    }

    public string ToString()
    {
        return
            m00 + " " + m01 + " " + m02 + " " + m03 + "\n" +
            m10 + " " + m11 + " " + m12 + " " + m13 + "\n" +
            m20 + " " + m21 + " " + m22 + " " + m23 + "\n" +
            m30 + " " + m31 + " " + m32 + " " + m33;
    }

    #endregion
}
