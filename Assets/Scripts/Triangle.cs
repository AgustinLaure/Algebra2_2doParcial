using CustomMath;

public class Triangle : IBoundeable
{
    public Vec3[] vertices;

    public Triangle(Vec3[] vertices)
    {
        this.vertices = vertices;
    }

    public Vec3 GetExtents()
    {
        Vec3 min = Vec3.Zero;
        Vec3 max = Vec3.Zero;

        min = Vec3.Min(vertices[0], Vec3.Min(vertices[1], vertices[2]));
        max = Vec3.Max(vertices[0], Vec3.Max(vertices[1], vertices[2]));

        return (max - min) * 0.5f;
    }

    public Vec3 GetPos()
    {
        Vec3 min = Vec3.Zero;
        Vec3 max = Vec3.Zero;

        min = Vec3.Min(vertices[0], Vec3.Min(vertices[1], vertices[2]));
        max = Vec3.Max(vertices[0], Vec3.Max(vertices[1], vertices[2]));

        return (min + max) * 0.5f;
    }
}
