using System.Collections.Generic;

namespace CSG.Sharp
{

    // Represents a plane in 3D space.
    public class Plane
    {
        private Vector _normal;
        private double _w;

        // `CSG.Plane.EPSILON` is the tolerance used by `splitPolygon()` to decide if a
        // point is on the plane.
        private static double EPSILON = 1e-5;

        public Plane(Vector normal, double w)
        {
            _normal = normal;
            _w = w;
        }

        public static Plane FromPoints(Vector a, Vector b,Vector c)
        {
            var n = b.Minus(a).Cross(c.Minus(a)).Unit();
            return new Plane(n, n.Dot(a));
        }

        public Plane Clone()
        {
            return new Plane(_normal.Clone(), _w);
        }
        public void Flip()
        {
            _normal = _normal.Negated();
            _w = -_w;
        }

        // Split `polygon` by this plane if needed, then put the polygon or polygon
        // fragments in the appropriate lists. Coplanar polygons go into either
        // `coplanarFront` or `coplanarBack` depending on their orientation with
        // respect to this plane. Polygons in front or in back of this plane go into
        // either `front` or `back`.
        public void SplitPolygon(Polygon polygon, ICollection<Polygon> coplanarFront, ICollection<Polygon> coplanarBack, ICollection<Polygon> front, ICollection<Polygon> back)
        {
            const int COPLANAR = 0;
            const int FRONT = 1;
            const int BACK = 2;
            const int SPANNING = 3;

            // Classify each point as well as the entire polygon into one of the above
            // four classes.
            var polygonType = 0;
            var types = new List<int>();
            for (var i = 0; i < polygon.Vertices.Length; i++)
            {
                var t = _normal.Dot(polygon.Vertices[i].Pos) - _w;
                var type = (t < -EPSILON) ? BACK : (t > EPSILON) ? FRONT : COPLANAR;
                polygonType |= type;
                types.Add(type);
            }

            // Put the polygon in the correct list, splitting it when necessary.
            switch (polygonType)
            {
                case COPLANAR:
                    (_normal.Dot(polygon.Plane._normal) > 0 ? coplanarFront : coplanarBack).Add(polygon);
                    break;
                case FRONT:
                    front.Add(polygon);
                    break;
                case BACK:
                    back.Add(polygon);
                    break;
                case SPANNING:
                    var f = new List<Vertex>();
                    var b = new List<Vertex>();
                    for (var i = 0; i < polygon.Vertices.Length; i++)
                    {
                        var j = (i + 1) % polygon.Vertices.Length;
                        var ti = types[i];
                        var tj = types[j];
                        var vi = polygon.Vertices[i];
                        var vj = polygon.Vertices[j];
                        if (ti != BACK) f.Add(vi);
                        if (ti != FRONT) b.Add(ti != BACK ? vi.Clone() : vi);
                        if ((ti | tj) == SPANNING)
                        {
                            var t = (_w - _normal.Dot(vi.Pos)) / _normal.Dot(vj.Pos.Minus(vi.Pos));
                            var v = vi.Interpolate(vj, t);
                            f.Add(v);
                            b.Add(v.Clone());
                        }
                    }
                    if (f.Count >= 3) front.Add(new Polygon(f.ToArray(), polygon.Shared));
                    if (b.Count >= 3) back.Add(new Polygon(b.ToArray(), polygon.Shared));
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", _normal, _w);
        }
    }
}