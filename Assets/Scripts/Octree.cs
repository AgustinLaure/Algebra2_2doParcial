using CustomMath;
using System.Collections.Generic;

public class Octree<T>
{
    private List<Node> nodes = new List<Node>();

    private class Node
    {
        public float width = 0f;
        public float height = 0f;
        public float depth = 0f;
        public Vec3 position = Vec3.Zero;
        public List<T> elements = new List<T>();
        public List<Node> childNodes = new List<Node>();

        public void Calculate(int maxElements)
        {
            if (elements.Count >= maxElements)
            {
                Vec3 newNodeSize = new Vec3(width * 0.5f, height * 0.5f, depth * 0.5f);

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

        public bool IsElementInside(T element)
        {
            return true;
        }

        private void CalculateSon(Vec3 newNodeSize, Vec3 direction, int maxElements)
        {
            Node currentNewNode = new Node();

            currentNewNode.width = newNodeSize.x;
            currentNewNode.height = newNodeSize.y;
            currentNewNode.depth = newNodeSize.z;
            currentNewNode.position = position + direction;

            foreach (T element in elements)
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

    public Octree(Vec3 pos, float width, float height, float depth, List<T> elements, int maxElements)
    {
        Node firstNode = new Node();

        firstNode.width = width;
        firstNode.height = height;
        firstNode.depth = depth;
        firstNode.position = pos;

        firstNode.elements = elements;
        nodes.Add(firstNode);

        firstNode.Calculate(maxElements);
    }
}
