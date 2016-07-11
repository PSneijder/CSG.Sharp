using CSG.Sharp.Extensions;
using System.Linq;

namespace CSG.Sharp
{
    // Represents a convex polygon. The vertices used to initialize a polygon must
    // be coplanar and form a convex loop. They do not have to be `CSG.Vertex`
    // instances but they must behave similarly (duck typing can be used for
    // customization).
    // 
    // Each convex polygon has a `shared` property, which is shared between all
    // polygons that are clones of each other or were split from the same polygon.
    // This can be used to define per-polygon properties (such as surface color).
    public struct Polygon
    {
        public Vertex[] Vertices { get; private set; }
        public object Shared { get; private set; }
        public Plane Plane { get; private set; }

        public Polygon(Vertex[] vertices, object shared = null)
        {
              Vertices = vertices;
              Shared = shared;
              Plane = Plane.FromPoints(vertices[0].Pos, vertices[1].Pos, vertices[2].Pos);
        }

        public Polygon Clone()
        {
            var vertices = Vertices.Select(v => v.Clone()).ToArray();
            return new Polygon(vertices, Shared);
        }


        public void Flip()
        {
            var vertices = Vertices.Reverse();
            vertices.ForEach(v => v.Flip());

            Vertices = vertices.ToArray();
            Plane.Flip();
        }
    }
}