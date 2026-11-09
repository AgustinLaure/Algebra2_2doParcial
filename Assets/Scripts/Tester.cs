using UnityEngine;
using System.Collections.Generic;
using CustomMath;

public class Tester : MonoBehaviour
{
    private List<IBoundeable> elements = new List<IBoundeable>();
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float depth;
    [SerializeField] private int maxElements;
    [SerializeField] private Color AABBColor;

    private Octree octree;

    private void Start()
    {
        Element[] elementsInScene = FindObjectsByType<Element>(FindObjectsSortMode.None);
        foreach (Element element in elementsInScene)
        {
            elements.Add(element);
        }

        octree = new Octree(transform.position, width, height, depth, elements, maxElements);
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
                }
            }
        }
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

    private void OnDrawGizmos()
    {
        octree?.Draw();
    }
}
