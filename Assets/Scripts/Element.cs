using UnityEngine;
using CustomMath;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Element : MonoBehaviour, IBoundeable
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private int modelOctreeMaxDepth;
    [SerializeField] private int modelOctreeMaxPerNode;
    private Color baseColor;
    public Octree modelOctree;
    public List<IBoundeable> triangles = new List<IBoundeable>();

    private bool shouldDrawModelOctree = false;

    public MeshRenderer GetMeshRenderer { get { return meshRenderer; } }
    public MeshFilter GetMeshFilter { get { return meshFilter; } }

    private void Awake()
    {
        baseColor = meshRenderer.material.color;
    }

    private void Start()
    {
        Mesh mesh = meshFilter.sharedMesh;

        SetTriangles();

        modelOctree = new Octree(mesh.bounds.center, mesh.bounds.size.x, mesh.bounds.size.y, mesh.bounds.size.z, triangles, modelOctreeMaxPerNode, modelOctreeMaxDepth);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            shouldDrawModelOctree = !shouldDrawModelOctree;
        }
    }

    private void SetTriangles()
    {
        Mesh mesh = meshFilter.sharedMesh;

        Vector3[] vertices = mesh.vertices;
        int[] indexes = mesh.triangles;

        for (int i = 0; i < indexes.Length / 3; i++)
        {
            Vec3[] triangleVertices = new Vec3[3];

            triangleVertices[0] = vertices[indexes[i * 3]];
            triangleVertices[1] = vertices[indexes[i * 3 + 1]];
            triangleVertices[2] = vertices[indexes[i * 3 + 2]];

            Triangle triangle = new Triangle(triangleVertices);

            triangles.Add(triangle);
        }
    }

    public void SetColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    public void ResetColor()
    {
        meshRenderer.material.color = baseColor;
    }

    public Vec3 GetExtents()
    {
        return meshRenderer.bounds.extents;
    }

    public Vec3 GetPos()
    {
        return meshRenderer.bounds.center;
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix = transform.localToWorldMatrix;

        if (shouldDrawModelOctree)
        {
            modelOctree?.Draw();
        }

        Gizmos.matrix = oldMatrix;

        if (shouldDrawModelOctree)
        {
            Gizmos.DrawWireCube(meshRenderer.bounds.center, meshRenderer.bounds.size);
        }
    }
}
