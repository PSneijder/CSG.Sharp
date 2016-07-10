using CSG.Sharp.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CSG.Sharp
{
    // Holds a node in a BSP tree. A BSP tree is built from a collection of polygons
    // by picking a polygon to split along. That polygon (and all other coplanar
    // polygons) are added directly to that node and the other polygons are added to
    // the front and/or back subtrees. This is not a leafy BSP tree since there is
    // no distinction between internal and leaf nodes.
    public class Node
    {
        private Plane _plane;
        private Node _front;
        private Node _back;
        private IList<Polygon> _polygons;

        public Node()
        {
            _plane = null;
            _front = null;
            _back = null;
            _polygons = new List<Polygon>();
        }

        public Node(Polygon[] polygons)
            : this()
        {
            if (polygons.Any()) Build(polygons);
        }

        public Node Clone()
        {
            var node = new Node();
            node._plane = _plane.Clone();
            node._front = _front.Clone();
            node._back = _back.Clone();
            node._polygons = _polygons.Select(p => p.Clone()).ToList();
            return node;
        }

        // Convert solid space to empty space and empty space to solid space.
        public void Invert()
        {
            for (var i = 0; i < _polygons.Count; i++)
            {
                _polygons[i].Flip();
            }
            _plane.Flip();
            if (_front != null) _front.Invert();
            if (_back != null) _back.Invert();
            var temp = _front;
            _front = _back;
            _back = temp;
        }

        // Recursively remove all polygons in `polygons` that are inside this BSP
        // tree.
        public IList<Polygon> ClipPolygons(IList<Polygon> polygons)
        {
            if (_plane == null) return polygons.Slice();
            var front = new List<Polygon>();
            var back = new List<Polygon>();
            for (var i = 0; i < polygons.Count; i++)
            {
                _plane.SplitPolygon(polygons[i], front, back, front, back);
            }
            if (_front != null) front = _front.ClipPolygons(front).ToList();
            if (_back != null) back = _back.ClipPolygons(back).ToList();
            else back = Enumerable.Empty<Polygon>().ToList();

            return front.Concat(back).ToArray();
        }

        // Remove all polygons in this BSP tree that are inside the other BSP tree
        // `bsp`.
        public void ClipTo(Node bsp)
        {
            _polygons = bsp.ClipPolygons(_polygons.ToArray());
            if (_front != null) _front.ClipTo(bsp);
            if (_back != null) _back.ClipTo(bsp);
        }

        // Return a list of all polygons in this BSP tree.
        public Polygon[] AllPolygons()
        {
            var polygons = _polygons.Slice();
            if (_front != null) polygons = polygons.Concat(_front.AllPolygons()).ToArray();
            if (_back != null) polygons = polygons.Concat(_back.AllPolygons()).ToArray();
            return polygons;
        }

        // Build a BSP tree out of `polygons`. When called on an existing tree, the
        // new polygons are filtered down to the bottom of the tree and become new
        // nodes there. Each set of polygons is partitioned using the first polygon
        // (no heuristic is used to pick a good split).
        public void Build(IList<Polygon> polygons)
        {
            if (polygons.Count == 0) return;
            if (_plane == null) _plane = polygons[0].Plane.Clone();
            var front = new List<Polygon>();
            var back = new List<Polygon>();
            for (var i = 0; i < polygons.Count; i++)
            {
                _plane.SplitPolygon(polygons[i], _polygons, _polygons, front, back);
            }
            if (front.Count > 0)
            {
                if (_front == null) _front = new Node();
                _front.Build(front);
            }
            if (back.Count > 0)
            {
                if (_back == null) _back = new Node();
                _back.Build(back);
            }
        }
    }
}