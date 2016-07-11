using System;
using System.Collections.Generic;
using System.Linq;

namespace CSG.Sharp
{

    // Construct an axis-aligned solid cuboid. Optional parameters are `center` and
    // `radius`, which default to `[0, 0, 0]` and `[1, 1, 1]`. The radius can be
    // specified using a single number or a list of three numbers, one for each axis.
    // 
    // Example code:
    // 
    //     var cube = CSG.cube({
    //       center: [0, 0, 0],
    //       radius: 1
    //     });
    public class Cube
    {
        public static CSG Create(Vector center = default(Vector), double radius = 1)
        {
            var c = center;
            var r = new double[] { radius, radius, radius };

            var polygons = new List<Polygon>()
            {
                { CreatePolygon(new List<short> { 0, 4, 6, 2 }, new Vector(-1, 0, 0), c, r) },
                { CreatePolygon(new List<short> { 1, 3, 7, 5 }, new Vector(+1, 0, 0), c, r) },
                { CreatePolygon(new List<short> { 0, 1, 5, 4 }, new Vector(0, -1, 0), c, r) },
                { CreatePolygon(new List<short> { 2, 6, 7, 3 }, new Vector(0, +1, 0), c, r) },
                { CreatePolygon(new List<short> { 0, 2, 3, 1 }, new Vector(0, 0, -1), c, r) },
                { CreatePolygon(new List<short> { 4, 5, 7, 6 }, new Vector(0, 0, +1), c, r) },
            };

            return CSG.FromPolygons(polygons.ToArray());
        }

        private static Polygon CreatePolygon(IEnumerable<short> info, Vector normal, Vector c, double[] r)
        {
            return new Polygon(info.Select(i => {

                var pos = new Vector(
                  c.x + r[0] * (2 * Convert.ToInt16(Convert.ToBoolean(i & 1)) - 1),
                  c.y + r[1] * (2 * Convert.ToInt16(Convert.ToBoolean(i & 2)) - 1),
                  c.z + r[2] * (2 * Convert.ToInt16(Convert.ToBoolean(i & 4)) - 1)
                );
                return new Vertex(pos, normal);

            }).ToArray());
        }
    }
}