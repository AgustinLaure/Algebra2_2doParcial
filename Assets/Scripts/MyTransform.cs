
using CustomMath;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTransform : IEnumerable
{
    private class Enumerator : IEnumerator
    {
        private MyTransform outer;

        private int currentIndex = -1;

        public object Current => outer.GetChild(currentIndex);

        internal Enumerator(MyTransform outer)
        {
            this.outer = outer;
        }

        public bool MoveNext()
        {
            int childCount = outer.childCount;
            return ++currentIndex < childCount;
        }

        public void Reset()
        {
            currentIndex = -1;
        }
    }

    private const float epsilon = 1e-05f;

    private Mat4x4 _worldTRS;
    private bool _isDirty = true;
    private bool _hasChanged = false;

    private Vec3 _localPosition;
    private Quat _localRotation;
    private Vec3 _localScale;
    private MyTransform _parent;
    public List<MyTransform> _children = new List<MyTransform>();

    //
    // Resumen:
    //     The world space position of the Transform.

    public MyTransform(Transform unityTransform)
    {
        this.localPosition = unityTransform.localPosition;
        this.localRotation = unityTransform.localRotation;
        this.localScale = unityTransform.localScale;

        _isDirty = true;
    }

    public Vec3 position
    {
        get
        {
            return localToWorldMatrix.GetPosition();
        }
        set
        {
            if (parent != null)
            {
                Mat4x4 parentWorldToLocal = parent.worldToLocalMatrix;

                Vector4 localPos = parentWorldToLocal * new Vector4(value.x, value.y, value.z, 1f);

                localPosition = new Vec3(localPos.x, localPos.y, localPos.z);
            }
            else
            {
                localPosition = value;
            }
        }
    }

    //
    // Resumen:
    //     Position of the transform relative to the parent transform.
    public Vec3 localPosition
    {
        get
        {
            return _localPosition;
        }
        set
        {
            _localPosition = value;
            SetDirty();
        }
    }

    //
    // Resumen:
    //     The rotation as Euler angles in degrees.
    public Vec3 eulerAngles
    {
        get
        {
            return rotation.eulerAngles;
        }
        set
        {
            if (parent != null)
            {
                Quat parentWorldToLocalRot = Quat.Inverse(parent.rotation);

                Quat localRot = parentWorldToLocalRot * Quat.Euler(value);

                localRotation = localRot;
            }
            else
            {
                localRotation = Quat.Euler(value);
            }
        }
    }

    //
    // Resumen:
    //     The rotation as Euler angles in degrees relative to the parent transform's rotation.
    public Vec3 localEulerAngles
    {
        get
        {
            return _localRotation.eulerAngles;
        }
        set
        {
            _localRotation = Quat.Euler(value);
            SetDirty();
        }
    }

    //
    // Resumen:
    //     The red axis of the transform in world space.
    public Vec3 right
    {
        get
        {
            return rotation * Vec3.Right;
        }
        set
        {
            Quat rotationOffset = Quat.FromToRotation(right, value);

            rotation = rotationOffset * rotation;
        }
    }

    //
    // Resumen:
    //     The green axis of the transform in world space.
    public Vec3 up
    {
        get
        {
            return rotation * Vec3.Up;
        }
        set
        {
            Quat rotationOffset = Quat.FromToRotation(up, value);

            rotation = rotationOffset * rotation;
        }
    }

    //
    // Resumen:
    //     Returns a normalized vector representing the blue axis of the transform in world
    //     space.
    public Vec3 forward
    {
        get
        {
            return rotation * Vec3.Forward;
        }
        set
        {
            Quat rotationOffset = Quat.FromToRotation(forward, value);

            rotation = rotationOffset * rotation;
        }
    }

    //
    // Resumen:
    //     A Quaternion that stores the rotation of the Transform in world space.
    public Quat rotation
    {
        get
        {
            if (parent != null)
            {
                return parent.rotation * localRotation;
            }

            return localRotation;
        }
        set
        {
            if (parent != null)
            {
                Quat parentWorldToLocalRot = Quat.Inverse(parent.rotation);

                Quat localRot = parentWorldToLocalRot * value;

                localRotation = localRot;
            }
            else
            {
                localRotation = value;
            }
        }
    }

    //
    // Resumen:
    //     The rotation of the transform relative to the transform rotation of the parent.
    public Quat localRotation
    {
        get
        {
            return _localRotation;
        }
        set
        {
            _localRotation = value;
            SetDirty();
        }
    }

    //
    // Resumen:
    //     The scale of the transform relative to the GameObjects parent.
    public Vec3 localScale
    {
        get
        {
            return _localScale;
        }
        set
        {
            _localScale = value;
            SetDirty();
        }
    }

    //
    // Resumen:
    //     The parent of the transform.
    public MyTransform parent
    {
        get
        {
            return _parent;
        }
        set
        {
            SetParent(value);
        }
    }

    //
    // Resumen:
    //     Matrix that transforms a point from world space into local space (Read Only).
    public Mat4x4 worldToLocalMatrix
    {
        get
        {
            return Mat4x4.Inverse(localToWorldMatrix);
        }
    }

    //
    // Resumen:
    //     Matrix that transforms a point from local space into world space (Read Only).
    public Mat4x4 localToWorldMatrix
    {
        get
        {
            if (_isDirty)
            {
                Mat4x4 cleanLocal = Mat4x4.TRS(localPosition, localRotation, localScale);

                if (parent != null)
                {
                    _worldTRS = parent.localToWorldMatrix * cleanLocal;
                }
                else
                {
                    _worldTRS = cleanLocal;
                }

                _isDirty = false;
                hasChanged = true;
            }

            return _worldTRS;
        }
    }

    //
    // Resumen:
    //     Returns the topmost transform in the hierarchy.
    public MyTransform root => GetRoot();

    //
    // Resumen:
    //     The number of children the parent Transform has.
    public int childCount
    {

        get
        {
            return _children.Count;
        }
    }

    //
    // Resumen:
    //     The global scale of the object (Read Only).
    public Vec3 lossyScale
    {
        get
        {
            return localToWorldMatrix.lossyScale;
        }
    }

    //
    // Resumen:
    //     Has the transform changed since the last time the flag was set to 'false'?

    public bool hasChanged
    {
        get
        {
            return _hasChanged;
        }
        set
        {
            _hasChanged = value;
        }
    }

    //
    // Resumen:
    //     The transform capacity of the transform's hierarchy data structure.
    public int hierarchyCapacity
    {
        get
        {
            return _children.Capacity;
        }
        set
        {
            _children.Capacity = value;
        }
    }

    //
    // Resumen:
    //     The number of transforms in the transform's hierarchy data structure.
    public int hierarchyCount
    {
        get
        {
            if (_children.Count <= 0)
            {
                return 0;
            }

            int count = _children.Count;

            foreach (MyTransform child in _children)
            {
                count += child.hierarchyCount;
            }

            return count;
        }
    }


    private MyTransform GetParent()
    {
        return parent;
    }

    //
    // Resumen:
    //     Set the parent of the transform.
    //
    // Parámetros:
    //   parent:
    //     The parent Transform to use.
    //
    //   worldPositionStays:
    //     If true, the parent-relative position, scale and rotation are modified such that
    //     the object keeps the same world space position, rotation and scale as before.
    //
    //
    //   p:
    public void SetParent(MyTransform p)
    {
        SetParent(p, true);
    }

    //
    // Resumen:
    //     Set the parent of the transform.
    //
    // Parámetros:
    //   parent:
    //     The parent Transform to use.
    //
    //   worldPositionStays:
    //     If true, the parent-relative position, scale and rotation are modified such that
    //     the object keeps the same world space position, rotation and scale as before.
    //
    //
    //   p:

    public void SetParent(MyTransform parent, bool worldPositionStays)
    {
        if (_parent != null)
        {
            _parent._children.Remove(this);
        }

        if (worldPositionStays)
        {
            Vec3 globalPos = position;
            Quat globalRot = rotation;
            Vec3 globalScale = lossyScale;

            _parent = parent;

            if (_parent != null)
            {
                _parent._children.Add(this);

                position = globalPos;
                rotation = globalRot;

                Vec3 parentLossyScale = _parent.lossyScale;

                //localScale = new Vec3(globalScale.x / parentLossyScale.x, globalScale.y / parentLossyScale.y, globalScale.z / parentLossyScale.z);
            }
            else
            {
                position = globalPos;
                rotation = globalRot;
                //localScale = globalScale;
            }
        }
        else
        {
            _parent = parent;

            if (_parent != null)
            {
                _parent._children.Add(this);
            }
        }

        SetDirty();
    }

    //
    // Resumen:
    //     Sets the world space position and rotation of the Transform component.
    //
    // Parámetros:
    //   position:
    //     The world space position to apply to the transform.
    //
    //   rotation:
    //     The world space rotation to apply to the transform.
    public void SetPositionAndRotation(Vec3 position, Quat rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    //
    // Resumen:
    //     Sets the position and rotation of the Transform component in local space (i.e.
    //     relative to its parent transform).
    //
    // Parámetros:
    //   localPosition:
    //     The local space position to apply to the transform.
    //
    //   localRotation:
    //     The local space rotation to apply to the transform.

    public void SetLocalPositionAndRotation(Vec3 localPosition, Quat localRotation)
    {
        this.localPosition = localPosition;
        this.localRotation = localRotation;
    }

    public void GetPositionAndRotation(out Vec3 position, out Quat rotation)
    {
        position = this.position;
        rotation = this.rotation;
    }

    public void GetLocalPositionAndRotation(out Vec3 localPosition, out Quat localRotation)
    {
        localPosition = this.localPosition;
        localRotation = this.localRotation;
    }

    //
    // Resumen:
    //     Moves the transform along its x, y, and z axes by the values of the translation
    //     parameter's x, y, and z components respectively.
    //
    // Parámetros:
    //   translation:
    //     The amount by which to move the Transform.
    //
    //   relativeTo:
    //     The coordinate system in which to apply the translation.
    public void Translate(Vec3 translation, Space relativeTo)
    {
        if (relativeTo == Space.World)
        {
            position += translation;
        }
        else if (relativeTo == Space.Self)
        {
            position += rotation * translation;
        }
    }

    //
    // Resumen:
    //     Moves the transform along its x, y, and z axes by the values of the translation
    //     parameter's x, y, and z components respectively.
    //
    // Parámetros:
    //   translation:
    //     The amount by which to move the Transform.
    //
    //   relativeTo:
    //     The coordinate system in which to apply the translation.
    public void Translate(Vec3 translation)
    {
        Translate(translation, Space.Self);
    }

    //
    // Resumen:
    //     Moves the transform by x along the x axis, y along the y axis, and z along the
    //     z axis.
    //
    // Parámetros:
    //   x:
    //     The amount by which to move the Transform on the x-axis.
    //
    //   y:
    //     The amount by which to move the Transform on the y-axis.
    //
    //   z:
    //     The amount by which to move the Transform on the z-axis.
    //
    //   relativeTo:
    //     The coordinate system in which the translation is applied.
    public void Translate(float x, float y, float z, Space relativeTo)
    {
        Translate(new Vec3(x, y, z), relativeTo);
    }

    //
    // Resumen:
    //     Moves the transform by x along the x axis, y along the y axis, and z along the
    //     z axis.
    //
    // Parámetros:
    //   x:
    //     The amount by which to move the Transform on the x-axis.
    //
    //   y:
    //     The amount by which to move the Transform on the y-axis.
    //
    //   z:
    //     The amount by which to move the Transform on the z-axis.
    //
    //   relativeTo:
    //     The coordinate system in which the translation is applied.
    public void Translate(float x, float y, float z)
    {
        Translate(new Vec3(x, y, z));
    }

    //
    // Resumen:
    //     Moves the transform along its x, y, and z axes by the values of the translation
    //     parameter's x, y, and z components respectively.
    //
    // Parámetros:
    //   translation:
    //     The amount by which to move the Transform.
    //
    //   relativeTo:
    //     Defines the coordinate system used for the translation.
    public void Translate(Vec3 translation, MyTransform relativeTo)
    {
        position += relativeTo.rotation * translation;
    }

    //
    // Resumen:
    //     Moves the transform by x along the x axis, y along the y axis, and z along the
    //     z axis.
    //
    // Parámetros:
    //   x:
    //     The amount by which to move the Transform on the x-axis.
    //
    //   y:
    //     The amount by which to move the Transform on the y-axis.
    //
    //   z:
    //     The amount by which to move the Transform on the z-axis.
    //
    //   relativeTo:
    //     Defines the coordinate system used for the translation.
    public void Translate(float x, float y, float z, MyTransform relativeTo)
    {
        position += relativeTo.rotation * new Vec3(x, y, z);
    }

    //
    // Resumen:
    //     Applies a rotation of eulerAngles.z degrees around the z-axis, eulerAngles.x
    //     degrees around the x-axis, and eulerAngles.y degrees around the y-axis (in that
    //     order).
    //
    // Parámetros:
    //   eulers:
    //     The rotation to apply in euler angles.
    //
    //   relativeTo:
    //     Determines whether to rotate the GameObject either locally to the GameObject
    //     or relative to the Scene in world space.
    public void Rotate(Vec3 eulers, Space relativeTo) //revisar
    {
        if (relativeTo == Space.World)
        {
            rotation = Quat.Euler(eulers) * rotation;
        }
        else if (relativeTo == Space.Self)
        {
            rotation = rotation * Quat.Euler(eulers);
        }
    }

    //
    // Resumen:
    //     Applies a rotation of eulerAngles.z degrees around the z-axis, eulerAngles.x
    //     degrees around the x-axis, and eulerAngles.y degrees around the y-axis (in that
    //     order).
    //
    // Parámetros:
    //   eulers:
    //     The rotation to apply in euler angles.
    public void Rotate(Vec3 eulers)
    {
        Rotate(eulers, Space.Self);
    }

    //
    // Resumen:
    //     The implementation of this method applies a rotation of zAngle degrees around
    //     the z axis, xAngle degrees around the x axis, and yAngle degrees around the y
    //     axis (in that order).
    //
    // Parámetros:
    //   xAngle:
    //     Degrees to rotate the GameObject around the X axis.
    //
    //   yAngle:
    //     Degrees to rotate the GameObject around the Y axis.
    //
    //   zAngle:
    //     Degrees to rotate the GameObject around the Z axis.
    //
    //   relativeTo:
    //     Determines whether to rotate the GameObject either locally to the GameObject
    //     or relative to the Scene in world space.
    public void Rotate(float xAngle, float yAngle, float zAngle, Space relativeTo)
    {
        Rotate(new Vec3(xAngle, yAngle, zAngle), relativeTo);
    }

    //
    // Resumen:
    //     The implementation of this method applies a rotation of zAngle degrees around
    //     the z axis, xAngle degrees around the x axis, and yAngle degrees around the y
    //     axis (in that order).
    //
    // Parámetros:
    //   xAngle:
    //     Degrees to rotate the GameObject around the X axis.
    //
    //   yAngle:
    //     Degrees to rotate the GameObject around the Y axis.
    //
    //   zAngle:
    //     Degrees to rotate the GameObject around the Z axis.
    public void Rotate(float xAngle, float yAngle, float zAngle)
    {
        Rotate(new Vec3(xAngle, yAngle, zAngle));
    }


    //
    // Resumen:
    //     Rotates the object around the given axis by the number of degrees defined by
    //     the given angle.
    //
    // Parámetros:
    //   axis:
    //     The axis to apply rotation to.
    //
    //   angle:
    //     The degrees of rotation to apply.
    //
    //   relativeTo:
    //     Determines whether to rotate the GameObject either locally to the GameObject
    //     or relative to the Scene in world space.
    public void Rotate(Vec3 axis, float angle, Space relativeTo)
    {
        if (relativeTo == Space.World)
        {
            rotation = Quat.AngleAxis(angle, axis) * rotation;
        }
        else if (relativeTo == Space.Self)
        {
            rotation = rotation * Quat.AngleAxis(angle, axis);
        }
    }

    //
    // Resumen:
    //     Rotates the object around the given axis by the number of degrees defined by
    //     the given angle.
    //
    // Parámetros:
    //   axis:
    //     The axis to apply rotation to.
    //
    //   angle:
    //     The degrees of rotation to apply.
    public void Rotate(Vec3 axis, float angle)
    {
        Rotate(axis, angle, Space.Self);
    }

    //
    // Resumen:
    //     Rotates the transform about axis passing through point in world coordinates by
    //     angle degrees.
    //
    // Parámetros:
    //   point:
    //     The world-space point to rotate the target object around.
    //
    //   axis:
    //     The world-space axis to rotate the target object around. This vector does not
    //     need to be unit length.
    //
    //   angle:
    //     The angle to rotate, provided in degrees.
    public void RotateAround(Vec3 point, Vec3 axis, float angle)
    {
        Quat toApplyRotation = Quat.AngleAxis(angle, axis);

        Vec3 offset = position - point;

        offset = toApplyRotation * offset;

        position = point + offset;

        rotation = toApplyRotation * rotation;
    }

    //
    // Resumen:
    //     Rotates the transform so the forward vector points at target's current position.
    //
    //
    // Parámetros:
    //   target:
    //     Object to point towards.
    //
    //   worldUp:
    //     Vector specifying the upward direction.
    public void LookAt(MyTransform target, Vec3 worldUp)
    {
        LookAt(target.position, worldUp);
    }

    //
    // Resumen:
    //     Rotates the transform so the forward vector points at target's current position.
    //
    //
    // Parámetros:
    //   target:
    //     Object to point towards.
    //
    //   worldUp:
    //     Vector specifying the upward direction.
    public void LookAt(MyTransform target)
    {
        LookAt(target, Vec3.Up);
    }

    //
    // Resumen:
    //     Rotates the transform so the forward vector points at worldPosition.
    //
    // Parámetros:
    //   worldPosition:
    //     Point to look at.
    //
    //   worldUp:
    //     Vector specifying the upward direction.
    public void LookAt(Vec3 worldPosition, Vec3 worldUp)
    {
        Vec3 forward = (worldPosition - position).normalized;

        if (forward != Vec3.Zero)
        {
            rotation = Quat.LookRotation(forward, worldUp);
        }
    }

    //
    // Resumen:
    //     Rotates the transform so the forward vector points at worldPosition.
    //
    // Parámetros:
    //   worldPosition:
    //     Point to look at.
    //
    //   worldUp:
    //     Vector specifying the upward direction.
    public void LookAt(Vec3 worldPosition)
    {
        LookAt(worldPosition, Vec3.Up);
    }

    //
    // Resumen:
    //     Transforms direction from local space to world space.
    //
    // Parámetros:
    //   direction:
    public Vec3 TransformDirection(Vec3 direction)
    {
        return rotation * direction;
    }

    //
    // Resumen:
    //     Transforms direction x, y, z from local space to world space.
    //
    // Parámetros:
    //   x:
    //
    //   y:
    //
    //   z:
    public Vec3 TransformDirection(float x, float y, float z)
    {
        return TransformDirection(new Vec3(x, y, z));
    }

    public void TransformDirections(ReadOnlySpan<Vec3> directions, Span<Vec3> transformedDirections)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            transformedDirections[i] = TransformDirection(directions[i]);
        }
    }

    public void TransformDirections(Span<Vec3> directions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            directions[i] = TransformDirection(directions[i]);
        }
    }

    //
    // Resumen:
    //     Transforms a direction from world space to local space. The opposite of Transform.TransformDirection.
    //
    //
    // Parámetros:
    //   direction:
    public Vec3 InverseTransformDirection(Vec3 direction)
    {
        return Quat.Inverse(rotation) * direction;
    }

    //
    // Resumen:
    //     Transforms the direction x, y, z from world space to local space. The opposite
    //     of Transform.TransformDirection.
    //
    // Parámetros:
    //   x:
    //
    //   y:
    //
    //   z:
    public Vec3 InverseTransformDirection(float x, float y, float z)
    {
        return InverseTransformDirection(new Vec3(x, y, z));
    }


    public void InverseTransformDirections(ReadOnlySpan<Vec3> directions, Span<Vec3> transformedDirections)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            transformedDirections[i] = InverseTransformDirection(directions[i]);
        }
    }

    public void InverseTransformDirections(Span<Vec3> directions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            directions[i] = InverseTransformDirection(directions[i]);
        }
    }

    //
    // Resumen:
    //     Transforms vector from local space to world space.
    //
    // Parámetros:
    //   vector:
    //     The vector to transform, in local space.
    //
    // Devuelve:
    //     The transformed vector, in world space.
    public Vec3 TransformVector(Vec3 vector)
    {
        return TransformVector(vector.x, vector.y, vector.z);
    }

    //
    // Resumen:
    //     Transforms vector x, y, z from local space to world space.
    //
    // Parámetros:
    //   x:
    //     The x component of the vector to transform, in local space.
    //
    //   y:
    //     The y component of the vector to transform, in local space.
    //
    //   z:
    //     The z component of the vector to transform, in local space.
    //
    // Devuelve:
    //     The transformed vector, in world space.
    public Vec3 TransformVector(float x, float y, float z)
    {
        Vector4 transformedDirection = localToWorldMatrix * new Vector4(x, y, z, 0f);

        return new Vec3(transformedDirection.x, transformedDirection.y, transformedDirection.z);
    }


    public void TransformVectors(ReadOnlySpan<Vec3> vectors, Span<Vec3> transformedVectors)
    {
        for (int i = 0; i < vectors.Length; i++)
        {
            transformedVectors[i] = TransformVector(vectors[i]);
        }
    }

    public void TransformVectors(Span<Vec3> vectors)
    {
        for (int i = 0; i < vectors.Length; i++)
        {
            vectors[i] = TransformVector(vectors[i]);
        }
    }

    //
    // Resumen:
    //     Transforms a vector from world space to local space. The opposite of Transform.TransformVector.
    //
    //
    // Parámetros:
    //   vector:
    //     The vector to transform, in world space.
    //
    // Devuelve:
    //     The transformed vector, in local space.
    public Vec3 InverseTransformVector(Vec3 vector)
    {
        return InverseTransformVector(vector.x, vector.y, vector.z);
    }

    //
    // Resumen:
    //     Transforms the vector x, y, z from world space to local space. The opposite of
    //     Transform.TransformVector.
    //
    // Parámetros:
    //   x:
    //     The x component of the vector to transform, in world space.
    //
    //   y:
    //     The y component of the vector to transform, in world space.
    //
    //   z:
    //     The z component of the vector to transform, in world space.
    //
    // Devuelve:
    //     The transformed vector, in local space.
    public Vec3 InverseTransformVector(float x, float y, float z)
    {
        Vector4 transformedDirection = worldToLocalMatrix * new Vector4(x, y, z, 0f);

        return new Vec3(transformedDirection.x, transformedDirection.y, transformedDirection.z);
    }

    public void InverseTransformVectors(ReadOnlySpan<Vec3> vectors, Span<Vec3> transformedVectors)
    {
        for (int i = 0; i < vectors.Length; i++)
        {
            transformedVectors[i] = InverseTransformVector(vectors[i]);
        }
    }

    public void InverseTransformVectors(Span<Vec3> vectors)
    {
        for (int i = 0; i < vectors.Length; i++)
        {
            vectors[i] = InverseTransformVector(vectors[i]);
        }
    }

    //
    // Resumen:
    //     Transforms position from local space to world space.
    //
    // Parámetros:
    //   position:
    public Vec3 TransformPoint(Vec3 position)
    {
        return TransformPoint(position.x, position.y, position.z);
    }

    //
    // Resumen:
    //     Transforms the position x, y, z from local space to world space.
    //
    // Parámetros:
    //   x:
    //
    //   y:
    //
    //   z:
    public Vec3 TransformPoint(float x, float y, float z)
    {
        Vector4 transformedDirection = localToWorldMatrix * new Vector4(x, y, z, 1f);

        return new Vec3(transformedDirection.x, transformedDirection.y, transformedDirection.z);
    }


    public void TransformPoints(ReadOnlySpan<Vec3> positions, Span<Vec3> transformedPositions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            transformedPositions[i] = TransformPoint(positions[i]);
        }
    }

    public void TransformPoints(Span<Vec3> positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = TransformPoint(positions[i]);
        }
    }

    //
    // Resumen:
    //     Transforms position from world space to local space.
    //
    // Parámetros:
    //   position:
    public Vec3 InverseTransformPoint(Vec3 position)
    {
        return InverseTransformPoint(position.x, position.y, position.z);
    }

    //
    // Resumen:
    //     Transforms the position x, y, z from world space to local space.
    //
    // Parámetros:
    //   x:
    //
    //   y:
    //
    //   z:
    public Vec3 InverseTransformPoint(float x, float y, float z)
    {
        Vector4 transformedDirection = worldToLocalMatrix * new Vector4(x, y, z, 1f);

        return new Vec3(transformedDirection.x, transformedDirection.y, transformedDirection.z);
    }

    public void InverseTransformPoints(ReadOnlySpan<Vec3> positions, Span<Vec3> transformedPositions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            transformedPositions[i] = InverseTransformPoint(positions[i]);
        }
    }

    public void InverseTransformPoints(Span<Vec3> positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = InverseTransformPoint(positions[i]);
        }
    }

    private MyTransform GetRoot()
    {
        if (parent != null)
        {
            return parent.GetRoot();
        }
        else
        {
            return this;
        }
    }

    //
    // Resumen:
    //     Unparents all of the target object's children.

    public void DetachChildren()
    {
        for (int i = _children.Count - 1; i >= 0; i--)
        {
            _children[i].SetParent(null);
        }
    }

    //
    // Resumen:
    //     Move the transform to the start of the local transform list.
    public void SetAsFirstSibling()
    {
        if (parent != null)
        {
            parent._children.Remove(this);
            parent._children.Insert(0, this);
        }
    }

    //
    // Resumen:
    //     Move the transform to the end of the local transform list.
    public void SetAsLastSibling()
    {
        if (parent != null)
        {
            parent._children.Remove(this);
            parent._children.Add(this);
        }
    }

    //
    // Resumen:
    //     Sets the sibling index.
    //
    // Parámetros:
    //   index:
    //     Index to set.
    public void SetSiblingIndex(int index)
    {
        if (parent != null)
        {
            parent._children.Remove(this);
            parent._children.Insert(index, this);
        }
    }


    //
    // Resumen:
    //     Gets the index of this Transform, relative to its siblings.
    //
    // Devuelve:
    //     The index of this Transform, relative to its siblings.
    public int GetSiblingIndex()
    {
        int result = 0;

        if (parent != null)
        {
            result = parent._children.IndexOf(this);
        }

        return result;
    }

    //
    // Resumen:
    //     Finds a child by name n and returns it.
    //
    // Parámetros:
    //   n:
    //     The search string, either the name of an immediate child or a hierarchy path
    //     for finding a descendent.
    //
    // Devuelve:
    //     The found child transform. Null if child with matching name isn't found.


    private void SetDirty()
    {
        _isDirty = true;
        _hasChanged = true;

        foreach (MyTransform child in _children)
        {
            child.SetDirty();
        }
    }

    //
    // Resumen:
    //     Is this transform a child of parent?
    //
    // Parámetros:
    //   parent:

    public bool IsChildOf(MyTransform parent)
    {
        MyTransform current = this;

        while (current != null)
        {
            if (current._parent == parent)
            {
                return true;
            }
            else
            {
                current = current._parent;
            }
        }

        return false;
    }

    public IEnumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    //
    // Resumen:
    //     Returns a transform child by index.
    //
    // Parámetros:
    //   index:
    //     Index of the child transform to return. Must be smaller than Transform.childCount.
    //
    //
    // Devuelve:
    //     Transform child by index.

    public MyTransform GetChild(int index)
    {
        if (_children.Count <= 0 || index >= _children.Count)
        {
            return null;
        }

        return _children[index];
    }
}


