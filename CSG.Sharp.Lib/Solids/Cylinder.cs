using System;
using System.Collections.Generic;

namespace CSG.Sharp
{
    // Construct a solid cylinder. Optional parameters are `start`, `end`,
    // `radius`, and `slices`, which default to `[0, -1, 0]`, `[0, 1, 0]`, `1`, and
    // `16`. The `slices` parameter controls the tessellation.
    // 
    // Example usage:
    // 
    //     var cylinder = CSG.cylinder({
    //       start: [0, -1, 0],
    //       end: [0, 1, 0],
    //       radius: 1,
    //       slices: 16
    //     });
    public class Cylinder
    {
        public static CSG Create(Vector startV = default(Vector), Vector endV = default(Vector), double radius = 1, double slices = 16)
        {
            var s = new Vector(startV == Vector.Zero ? Vector.Down : startV);
            var e = new Vector(endV == Vector.Zero ? Vector.Up : endV);
            var ray = e.Minus(s);
            var r = radius;
            var axisZ = ray.Unit();
            var isY = (Math.Abs(axisZ.y) > 0.5);

            var axisX = new Vector((isY ? 1 : 0), (!isY ? 0 : 1), 0).Cross(axisZ).Unit();
            var axisY = axisX.Cross(axisZ).Unit();
            var start = new Vertex(s, axisZ.Negated());
            var end = new Vertex(e, axisZ.Unit());

            var polygons = new List<Polygon>();
            for (var i = 0; i < slices; i++)
            {
                var t0 = i / slices;
                var t1 = (i + 1) / slices;
                polygons.Add(new Polygon(new Vertex[] { start, point(axisX, axisY, axisZ, s, ray, r, 0, t0, -1), point(axisX, axisY, axisZ, s, ray, r, 0, t1, -1) }));
                polygons.Add(new Polygon(new Vertex[] { point(axisX, axisY, axisZ, s, ray, r, 0, t1, 0), point(axisX, axisY, axisZ, s, ray, r, 0, t0, 0), point(axisX, axisY, axisZ, s, ray, r, 1, t0, 0), point(axisX, axisY, axisZ, s, ray, r, 1, t1, 0) }));
                polygons.Add(new Polygon(new Vertex[] { end, point(axisX, axisY, axisZ, s, ray, r, 1, t1, 1), point(axisX, axisY, axisZ, s, ray, r, 1, t0, 1) }));
            }
            return CSG.FromPolygons(polygons.ToArray());
        }

        internal static Vertex point(Vector axisX, Vector axisY, Vector axisZ, Vector s, Vector ray, double r, double stack, double slice, double normalBlend)
        {
            var angle = slice * Math.PI * 2;
            var @out = axisX.Times(Math.Cos(angle)).Plus(axisY.Times(Math.Sin(angle)));
            var pos = s.Plus(ray.Times(stack)).Plus(@out.Times(r));
            var normal = @out.Times(1 - Math.Abs(normalBlend)).Plus(axisZ.Times(normalBlend));
            return new Vertex(pos, normal);
        }
    }
}
