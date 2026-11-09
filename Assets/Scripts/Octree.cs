using CustomMath;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    private Node topNode = null;
    public class Node
    {
        public Vec3 size = Vec3.Zero;
        public Vec3 position = Vec3.Zero;
        public List<Element> elements = new List<Element>();
        public List<Node> childNodes = new List<Node>();

        public void Calculate(int maxElements)
        {
            if (elements.Count >= maxElements)
            {
                Vec3 newNodeSize = new Vec3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f);

                Vec3 direction = Vec3.One;

                direction.x *= newNodeSize.x * 0.5f;
                direction.y *= newNodeSize.y * 0.5f;
                direction.z *= newNodeSize.z * 0.5f;

                for (int i = 0; i < 6; i++)
                {
                    CalculateSon(newNodeSize, direction, maxElements);

                    if (i % 3 == 0)
                    {
                        direction.x *= -1f;
                    }
                    else if (i % 3 == 1)
                    {
                        direction.y *= -1f;
                    }
                    else if (i % 3 == 2)
                    {
                        direction.z *= -1f;
                    }
                }

                direction.x *= -1f;
                direction.z *= -1f;
                CalculateSon(newNodeSize, direction, maxElements);

                direction = direction * -1f;
                CalculateSon(newNodeSize, direction, maxElements);

                elements.Clear();
            }
        }

        public void DrawNode()
        {
            Gizmos.DrawWireCube(position, size);
        }

        public bool IsElementInside(Element element)
        {
            MeshRenderer elementMeshRenderer = element.GetMeshRenderer;

            Vec3 elementCenter = elementMeshRenderer.bounds.center;
            Vec3 elementBounds = elementMeshRenderer.bounds.extents;

            Vec3 halfExtents = size * 0.5f;

            return position.x + halfExtents.x >= elementCenter.x - elementBounds.x
                && position.x - halfExtents.x <= elementCenter.x + elementBounds.x
                && position.y + halfExtents.y >= elementCenter.y - elementBounds.y
                  && position.y - halfExtents.y <= elementCenter.y + elementBounds.y
                  && position.z + halfExtents.z >= elementCenter.z - elementBounds.z
                  && position.z - halfExtents.z <= elementCenter.z + elementBounds.z;
        }

        private void CalculateSon(Vec3 newNodeSize, Vec3 direction, int maxElements)
        {
            Node currentNewNode = new Node();

            currentNewNode.size.x = newNodeSize.x;
            currentNewNode.size.y = newNodeSize.y;
            currentNewNode.size.z = newNodeSize.z;
            currentNewNode.position = position + direction;

            foreach (Element element in elements)
            {
                if (currentNewNode.IsElementInside(element))
                {
                    currentNewNode.elements.Add(element);
                }
            }

            currentNewNode.Calculate(maxElements);
            childNodes.Add(currentNewNode);
        }
    }

    public Octree(Vec3 pos, float width, float height, float depth, List<Element> elements, int maxElements)
    {
        Node firstNode = new Node();

        topNode = firstNode;

        firstNode.size.x = width;
        firstNode.size.y = height;
        firstNode.size.z = depth;
        firstNode.position = pos;

        firstNode.elements = elements;

        firstNode.Calculate(maxElements);
    }

    private void DrawRecursive(Node topNode)
    {
        topNode.DrawNode();

        foreach (Node node in topNode.childNodes)
        {
            DrawRecursive(node);
        }
    }

    public void Draw()
    {
        if (topNode != null)
        {
            DrawRecursive(topNode);
        }
    }
}
