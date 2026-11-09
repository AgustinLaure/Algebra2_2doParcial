using CustomMath;
using UnityEngine;
using System.Collections.Generic;

public class Tester : MonoBehaviour
{
    private List<Element> elements = new List<Element>();
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float depth;

    private Octree octree;

    private void Start()
    {
        Element[] elementsInScene = FindObjectsByType<Element>(FindObjectsSortMode.None);
        foreach (Element element in elementsInScene)
        {
            elements.Add(element);
        }

        octree = new Octree(transform.position, width, height, depth, elements, 7);
    }

    private void OnDrawGizmos()
    {
        octree?.Draw();
    }
}
