using CustomMath;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    private List<IBoundeable> elements = new List<IBoundeable>();
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float depth;
    [SerializeField] private UnityEngine.Color AABBColor;
    [SerializeField] private UnityEngine.Color realCollisionColor;

    [SerializeField] private int maxElements;
    [SerializeField] private int octreeMaxDepth;

    [SerializeField] private float epsilon = 0.000001f;

    private Octree octree;

    private void Start()
    {
        Element[] elementsInScene = FindObjectsByType<Element>(FindObjectsSortMode.None);
        foreach (Element element in elementsInScene)
        {
            elements.Add(element);
        }

        octree = new Octree(transform.position, width, height, depth, elements, maxElements, octreeMaxDepth);
    }

    private void Update()
    {
        foreach (Element element in elements)
        {
            element.ResetColor();
        }

        octree.DoSomething(HandleCollision);
    }

    private void HandleCollision(Octree.Node currentNode)
    {
        for (int i = 0; i < currentNode.elements.Count - 1; i++)
        {
            for (int j = i + 1; j < currentNode.elements.Count; j++)
            {
                if (AABB(currentNode.elements[i], currentNode.elements[j]))
                {
                    Element element1 = currentNode.elements[i] as Element;

                    if (element1 != null)
                    {
                        element1.SetColor(AABBColor);
                    }

                    Element element2 = currentNode.elements[j] as Element;

                    if (element2 != null)
                    {
                        element2.SetColor(AABBColor);
                    }

                    if (CheckMeshCollision(element1, element2))
                    {
                        element1.SetColor(realCollisionColor);
                        element2.SetColor(realCollisionColor);
                    }
                }
            }
        }
    }

    private bool CheckMeshCollision(Element a, Element b)
    {
        bool isCollision = false;

        CheckMeshCollisionRecursive(ref isCollision, a.modelOctree.topNode, b.modelOctree.topNode, a.transform, b.transform);

        //Triangle randomTriA = GetSmallestNode(a.modelOctree.topNode).elements[0] as Triangle;
        //Triangle randomTriB = GetSmallestNode(b.modelOctree.topNode).elements[0] as Triangle;
        //Mat4x4 bToLocalA = a.transform.worldToLocalMatrix * b.transform.localToWorldMatrix;
        //Mat4x4 aToLocalB = b.transform.worldToLocalMatrix * a.transform.localToWorldMatrix;
        //
        //bool isInside = IsVertexInside(bToLocalA, b, randomTriA) || IsVertexInside(aToLocalB, a, randomTriB);
        //
        //if (isInside)
        //{
        //    return true;
        //}

        return isCollision;
    }

    private bool IsVertexInside(Mat4x4 bToLocalA, Element b, Triangle randomTri)
    {
        foreach (Triangle triangle in b.triangles)
        {
            MyPlane bPlane = new MyPlane(bToLocalA.MultiplyPoint3x4(triangle.vertices[0]), bToLocalA.MultiplyPoint3x4(triangle.vertices[1]), bToLocalA.MultiplyPoint3x4(triangle.vertices[2]));

            if (bPlane.GetSide(randomTri.vertices[0]))
            {
                return false;
            }
        }

        return true;
    }
    private Octree.Node GetSmallestNode(Octree.Node node)
    {
        if (node.childNodes.Count <= 0)
        {
            return node;
        }
        else
        {
            return GetSmallestNode(node.childNodes[0]);
        }
    }

    private void CheckMeshCollisionRecursive(ref bool isCollision, Octree.Node currentANode, Octree.Node currentBNode, Transform aTransform, Transform bTransform)
    {
        if (isCollision)
        {
            return;
        }

        Mat4x4 bToLocalAMat = aTransform.worldToLocalMatrix * bTransform.localToWorldMatrix;

        if (NodeCollision(bToLocalAMat, currentANode, currentBNode, aTransform, bTransform))
        {
            bool aHasChildren = currentANode.childNodes.Count > 0;
            bool bHasChildren = currentBNode.childNodes.Count > 0;

            if (!aHasChildren && !bHasChildren)
            {
                for (int i = 0; i < currentANode.elements.Count; i++)
                {
                    for (int j = 0; j < currentBNode.elements.Count; j++)
                    {
                        if (TriangleCollision(bToLocalAMat, currentANode.elements[i] as Triangle, currentBNode.elements[j] as Triangle))
                        {
                            isCollision = true;
                            return;
                        }
                    }
                }
            }
            else if (aHasChildren && !bHasChildren)
            {
                foreach (Octree.Node aNodeChild in currentANode.childNodes)
                {
                    CheckMeshCollisionRecursive(ref isCollision, aNodeChild, currentBNode, aTransform, bTransform);
                }
            }
            else if (!aHasChildren && bHasChildren)
            {
                foreach (Octree.Node bNodeChild in currentBNode.childNodes)
                {
                    CheckMeshCollisionRecursive(ref isCollision, currentANode, bNodeChild, aTransform, bTransform);
                }
            }
            else
            {
                foreach (Octree.Node aNodeChild in currentANode.childNodes)
                {
                    foreach (Octree.Node bNodeChild in currentBNode.childNodes)
                    {
                        CheckMeshCollisionRecursive(ref isCollision, aNodeChild, bNodeChild, aTransform, bTransform);
                        if (isCollision)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }

    private bool NodeCollision(Mat4x4 bToLocalAMat, Octree.Node aNode, Octree.Node bNode, Transform aTransform, Transform bTransform)
    {
        float absM00 = Mathf.Abs(bToLocalAMat.m00);
        float absM01 = Mathf.Abs(bToLocalAMat.m01);
        float absM02 = Mathf.Abs(bToLocalAMat.m02);

        float absM10 = Mathf.Abs(bToLocalAMat.m10);
        float absM11 = Mathf.Abs(bToLocalAMat.m11);
        float absM12 = Mathf.Abs(bToLocalAMat.m12);

        float absM20 = Mathf.Abs(bToLocalAMat.m20);
        float absM21 = Mathf.Abs(bToLocalAMat.m21);
        float absM22 = Mathf.Abs(bToLocalAMat.m22);

        Vec3 bNewSize = new Vec3
            (
                absM00 * bNode.size.x + absM01 * bNode.size.y + absM02 * bNode.size.z,
                absM10 * bNode.size.x + absM11 * bNode.size.y + absM12 * bNode.size.z,
                absM20 * bNode.size.x + absM21 * bNode.size.y + absM22 * bNode.size.z
            );

        return AABB(aNode.position, aNode.size, bToLocalAMat.MultiplyPoint3x4(bNode.position), bNewSize);
    }

    private bool TriangleCollision(Mat4x4 bToLocalAMat, Triangle a, Triangle b)
    {
        Vec3[] transformedB = new Vec3[]
            {
                bToLocalAMat.MultiplyPoint3x4(b.vertices[0]),
                bToLocalAMat.MultiplyPoint3x4(b.vertices[1]),
                bToLocalAMat.MultiplyPoint3x4(b.vertices[2])
            };

        MyPlane aPlane = new MyPlane(a.vertices[0], a.vertices[1], a.vertices[2]);
        MyPlane bPlane = new MyPlane(transformedB[0], transformedB[1], transformedB[2]);

        if (aPlane.Normal == bPlane.Normal)
        {
            for (int i = 0; i < a.vertices.Length; i++)
            {
                if (IsInsideTriangle(transformedB[0], transformedB[1], transformedB[2], bPlane, a.vertices[i]))
                {
                    return true;
                }
            }
        }
        else
        {
            if (CheckTriangleOnTriangle(a.vertices, bPlane, transformedB))
            {
                return true;
            }

            if (CheckTriangleOnTriangle(transformedB, aPlane, a.vertices))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckTriangleOnTriangle(Vec3[] a, MyPlane bPlane, Vec3[] transformedB)
    {
        for (int i = 0; i < a.Length; i++)
        {
            int endIndex = i + 1;
    
            if (endIndex >= a.Length)
            {
                endIndex = 0;
            }
    
            Vec3 start = a[i];
            Vec3 end = a[endIndex];
    
            float startDist = -bPlane.GetDistanceToPoint(start);
            float endDist = -bPlane.GetDistanceToPoint(end);
    
            if ((startDist > epsilon && endDist < -epsilon) || (startDist < -epsilon && endDist > epsilon)
                || Mathf.Abs(startDist) <= epsilon || Mathf.Abs(endDist) <= epsilon)
            {
                Vec3 segmentDir = end - start;
                float orthoDist = Vec3.Dot(bPlane.Normal, segmentDir);
    
                if (Mathf.Abs(orthoDist) > epsilon)
                {
                    float t = startDist / orthoDist;

                    Vec3 intersection = start + (segmentDir * Mathf.Clamp01(t));
    
                    if (IsInsideTriangle(transformedB[0], transformedB[1], transformedB[2], bPlane, intersection))
                    {
                        return true;
                    }
                }
            }
        }
    
        return false;
    }

    private bool IsInsideTriangle(Vec3 a, Vec3 b, Vec3 c, MyPlane plane, Vec3 point)
    {
        float triangle1Area = TriangleArea(a, b, point);
        float triangle2Area = TriangleArea(b, c, point);
        float triangle3Area = TriangleArea(c, a, point);

        return triangle1Area + triangle2Area + triangle3Area - TriangleArea(a, b, c) < epsilon;
    }

    private float TriangleArea(Vec3 a, Vec3 b, Vec3 c)
    {
        return Vec3.Cross(b - a, c - a).magnitude * 0.5f;
    }

    public bool AABB(IBoundeable element1, IBoundeable element2)
    {
        Vec3 element1Center = element1.GetPos();
        Vec3 element1Bounds = element1.GetExtents();

        Vec3 element2Center = element2.GetPos();
        Vec3 element2Bounds = element2.GetExtents();

        return element1Center.x + element1Bounds.x >= element2Center.x - element2Bounds.x
            && element1Center.x - element1Bounds.x <= element2Center.x + element2Bounds.x
            && element1Center.y + element1Bounds.y >= element2Center.y - element2Bounds.y
              && element1Center.y - element1Bounds.y <= element2Center.y + element2Bounds.y
              && element1Center.z + element1Bounds.z >= element2Center.z - element2Bounds.z
              && element1Center.z - element1Bounds.z <= element2Center.z + element2Bounds.z;
    }

    private bool AABB(Vec3 aPos, Vec3 aSize, Vec3 bPos, Vec3 bSize)
    {
        Vec3 aHalfExtents = aSize * 0.5f;
        Vec3 bHalfExtents = bSize * 0.5f;

        return aPos.x + aHalfExtents.x >= bPos.x - bHalfExtents.x &&
            aPos.x - aHalfExtents.x <= bPos.x + bHalfExtents.x &&
            aPos.y + aHalfExtents.y >= bPos.y - bHalfExtents.y &&
            aPos.y - aHalfExtents.y <= bPos.y + bHalfExtents.y &&
            aPos.z + aHalfExtents.z >= bPos.z - bHalfExtents.z &&
            aPos.z - aHalfExtents.z <= bPos.z + bHalfExtents.z;
    }

    private void OnDrawGizmos()
    {
        octree?.Draw();
    }
}
