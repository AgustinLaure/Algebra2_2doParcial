using CustomMath;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    public Node topNode = null;

    public class Node
    {
        public Vec3 size = Vec3.Zero;
        public Vec3 position = Vec3.Zero;
        public List<IBoundeable> elements = new List<IBoundeable>();
        public List<Node> childNodes = new List<Node>();

        public void Calculate(int maxElements, int currentDepth, int maxDepth)
        {
            if (elements.Count >= maxElements && currentDepth <= maxDepth)
            {
                Vec3 newNodeSize = new Vec3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f);

                Vec3 direction = Vec3.One;

                direction.x *= newNodeSize.x * 0.5f;
                direction.y *= newNodeSize.y * 0.5f;
                direction.z *= newNodeSize.z * 0.5f;

                for (int i = 0; i < 6; i++)
                {
                    CalculateSon(newNodeSize, direction, maxElements, currentDepth, maxDepth);

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
                CalculateSon(newNodeSize, direction, maxElements, currentDepth, maxDepth);

                direction = direction * -1f;
                CalculateSon(newNodeSize, direction, maxElements, currentDepth, maxDepth);

                elements.Clear();
            }
        }

        private void CalculateSon(Vec3 newNodeSize, Vec3 direction, int maxElements, int currentDepth, int maxDepth)
        {
            Node currentNewNode = new Node();

            currentNewNode.size.x = newNodeSize.x;
            currentNewNode.size.y = newNodeSize.y;
            currentNewNode.size.z = newNodeSize.z;
            currentNewNode.position = position + direction;

            foreach (IBoundeable element in elements)
            {
                if (currentNewNode.IsElementInside(element))
                {
                    currentNewNode.elements.Add(element);
                }
            }

            currentNewNode.Calculate(maxElements, currentDepth + 1, maxDepth);
            childNodes.Add(currentNewNode);
        }

        public void DrawNode()
        {
            Gizmos.DrawWireCube(position, size);
        }

        public void DoSomethingRecursive(Action<Node> action)
        {
            if (childNodes.Count <= 0)
            {
                action(this);
            }
            else
            {
                foreach (Node node in childNodes)
                {
                    node.DoSomethingRecursive(action);
                }
            }
        }

        public bool IsElementInside(IBoundeable element)
        {
            Vec3 elementCenter = element.GetPos();
            Vec3 elementBounds = element.GetExtents();

            Vec3 halfExtents = size * 0.5f;

            return position.x + halfExtents.x >= elementCenter.x - elementBounds.x
                && position.x - halfExtents.x <= elementCenter.x + elementBounds.x
                && position.y + halfExtents.y >= elementCenter.y - elementBounds.y
                  && position.y - halfExtents.y <= elementCenter.y + elementBounds.y
                  && position.z + halfExtents.z >= elementCenter.z - elementBounds.z
                  && position.z - halfExtents.z <= elementCenter.z + elementBounds.z;
        }
    }

    public Octree(Vec3 pos, float width, float height, float depth, List<IBoundeable> elements, int maxElements, int maxDepth)
    {
        Node firstNode = new Node();

        topNode = firstNode;

        firstNode.size.x = width;
        firstNode.size.y = height;
        firstNode.size.z = depth;
        firstNode.position = pos;

        firstNode.elements = new List<IBoundeable>(elements);

        firstNode.Calculate(maxElements, 0, maxDepth);
    }

    public void Recalculate(List<IBoundeable> elements)
    {

    }

    private void DrawRecursive(Node topNode)
    {
        topNode.DrawNode();

        foreach (Node node in topNode.childNodes)
        {
            DrawRecursive(node);
        }
    }

    public void DoSomething(Action<Node> action)
    {
        topNode.DoSomethingRecursive(action);
    }

    public void Draw()
    {
        if (topNode != null)
        {
            DrawRecursive(topNode);
        }
    }
}
