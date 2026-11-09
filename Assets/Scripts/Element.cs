using UnityEngine;

public class Element : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;

    public MeshRenderer GetMeshRenderer { get { return meshRenderer; } }
    public MeshFilter GetMeshFilter { get { return meshFilter; } }
}
