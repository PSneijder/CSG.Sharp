using System.Linq;
using CSG.Sharp.Extensions;

namespace CSG.Sharp
{
    public class CSG
    {
        // Constructive Solid Geometry (CSG) is a modeling technique that uses Boolean
        // operations like union and intersection to combine 3D solids. This library
        // implements CSG operations on meshes elegantly and concisely using BSP trees,
        // and is meant to serve as an easily understandable implementation of the
        // algorithm. All edge cases involving overlapping coplanar polygons in both
        // solids are correctly handled.
        // 
        // Example usage:
        // 
        //     var cube = CSG.cube();
        //     var sphere = CSG.sphere({ radius: 1.3 });
        //     var polygons = cube.subtract(sphere).toPolygons();
        // 
        // ## Implementation Details
        // 
        // All CSG operations are implemented in terms of two functions, `clipTo()` and
        // `invert()`, which remove parts of a BSP tree inside another BSP tree and swap
        // solid and empty space, respectively. To find the union of `a` and `b`, we
        // want to remove everything in `a` inside `b` and everything in `b` inside `a`,
        // then combine polygons from `a` and `b` into one solid:
        // 
        //     a.clipTo(b);
        //     b.clipTo(a);
        //     a.build(b.allPolygons());
        // 
        // The only tricky part is handling overlapping coplanar polygons in both trees.
        // The code above keeps both copies, but we need to keep them in one tree and
        // remove them in the other tree. To remove them from `b` we can clip the
        // inverse of `b` against `a`. The code for union now looks like this:
        // 
        //     a.clipTo(b);
        //     b.clipTo(a);
        //     b.invert();
        //     b.clipTo(a);
        //     b.invert();
        //     a.build(b.allPolygons());
        // 
        // Subtraction and intersection naturally follow from set operations. If
        // union is `A | B`, subtraction is `A - B = ~(~A | B)` and intersection is
        // `A & B = ~(~A | ~B)` where `~` is the complement operator.

        // # class CSG

        // Holds a binary space partition tree representing a 3D solid. Two solids can
        // be combined using the `union()`, `subtract()`, and `intersect()` methods.

        private Polygon[] polygons;

        public CSG()
        {

        }

        // Construct a CSG solid from a list of `CSG.Polygon` instances.
        public static CSG FromPolygons(Polygon[] polygons)
        {
            var csg = new CSG();
            csg.polygons = polygons;
            return csg;
        }

        public CSG Clone()
        {
            var csg = new CSG();
            csg.polygons = polygons.Select(p => p.Clone()).ToArray();
            return csg;
        }

        public Polygon[] ToPolygons()
        {
            return polygons;
        }

        // Return a new CSG solid representing space in either this solid or in the
        // solid `csg`. Neither this solid nor the solid `csg` are modified.
        // 
        //     A.union(B)
        // 
        //     +-------+            +-------+
        //     |       |            |       |
        //     |   A   |            |       |
        //     |    +--+----+   =   |       +----+
        //     +----+--+    |       +----+       |
        //          |   B   |            |       |
        //          |       |            |       |
        //          +-------+            +-------+
        // 
        public CSG Union(CSG csg)
        {
            var a = new Node(Clone().polygons);
            var b = new Node(csg.Clone().polygons);
            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons());
            return CSG.FromPolygons(a.AllPolygons());
        }

        // Return a new CSG solid representing space in this solid but not in the
        // solid `csg`. Neither this solid nor the solid `csg` are modified.
        // 
        //     A.subtract(B)
        // 
        //     +-------+            +-------+
        //     |       |            |       |
        //     |   A   |            |       |
        //     |    +--+----+   =   |    +--+
        //     +----+--+    |       +----+
        //          |   B   |
        //          |       |
        //          +-------+
        // 
        public CSG Subtract(CSG csg)
        {
            var a = new Node(Clone().polygons);
            var b = new Node(csg.Clone().polygons);
            a.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons());
            a.Invert();
            return CSG.FromPolygons(a.AllPolygons());
        }

        // Return a new CSG solid representing space both this solid and in the
        // solid `csg`. Neither this solid nor the solid `csg` are modified.
        // 
        //     A.intersect(B)
        // 
        //     +-------+
        //     |       |
        //     |   A   |
        //     |    +--+----+   =   +--+
        //     +----+--+    |       +--+
        //          |   B   |
        //          |       |
        //          +-------+
        // 
        public CSG Intersect(CSG csg)
        {
            var a = new Node(Clone().polygons);
            var b = new Node(csg.Clone().polygons);
            a.Invert();
            b.ClipTo(a);
            b.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            a.Build(b.AllPolygons());
            a.Invert();
            return CSG.FromPolygons(a.AllPolygons());
        }

        // Return a new CSG solid with solid and empty space switched. This solid is
        // not modified.
        public CSG Inverse()
        {
            var csg = Clone();
            csg.polygons.ForEach(p => p.Flip());
            return csg;
        }
    }
}