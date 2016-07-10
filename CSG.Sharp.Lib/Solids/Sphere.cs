using System;
using System.Collections.Generic;

namespace CSG.Sharp
{
    // Construct a solid sphere. Optional parameters are `center`, `radius`,
    // `slices`, and `stacks`, which default to `[0, 0, 0]`, `1`, `16`, and `8`.
    // The `slices` and `stacks` parameters control the tessellation along the
    // longitude and latitude directions.
    // 
    // Example usage:
    // 
    //     var sphere = CSG.sphere({
    //       center: [0, 0, 0],
    //       radius: 1,
    //       slices: 16,
    //       stacks: 8
    //     });
    public class Sphere
    {
        public static CSG Create(Vector center = default(Vector), double radius = 1, double slices = 16, double stacks = 8)
        {
            var c = new Vector(center);
            var r = radius;
            var polygons = new List<Polygon>();
            var vertices = new List<Vertex>();

            for (var i = 0; i < slices; i++)
            {
                for (var j = 0; j < stacks; j++)
                {
                    vertices.Clear();
                    Vertex(vertices, c, r, i / slices, j / stacks);
                    if (j > 0) Vertex(vertices, c, r, (i + 1) / slices, j / stacks);
                    if (j < stacks - 1) Vertex(vertices, c, r, (i + 1) / slices, (j + 1) / stacks);
                    Vertex(vertices, c, r, i / slices, (j + 1) / stacks);
                    polygons.Add(new Polygon(vertices.ToArray()));
                }
            }
            return CSG.FromPolygons(polygons.ToArray());
        }

        internal static void Vertex(IList<Vertex> vertices, Vector c, double r, double theta, double phi)
        {
            theta *= Math.PI * 2;
            phi *= Math.PI;

            var dir = new Vector(
              Math.Cos(theta) * Math.Sin(phi),
              Math.Cos(phi),
              Math.Sin(theta) * Math.Sin(phi)
            );

            vertices.Add(new Vertex(c.Plus(dir.Times(r)), dir));
        }
    }
}