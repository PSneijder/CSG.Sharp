
namespace CSG.Sharp
{
    // Represents a vertex of a polygon. Use your own vertex class instead of this
    // one to provide additional features like texture coordinates and vertex
    // colors. Custom vertex classes need to provide a `pos` property and `clone()`,
    // `flip()`, and `interpolate()` methods that behave analogous to the ones
    // defined by `CSG.Vertex`. This class provides `normal` so convenience
    // functions like `CSG.sphere()` can return a smooth vertex normal, but `normal`
    // is not used anywhere else.
    public struct Vertex
    {
        public Vector Pos { get; private set; }
        public Vector Normal { get; private set; }

        public Vertex(Vector pos, Vector normal)
        {
            Pos = new Vector(pos);
            Normal = new Vector(normal);
        }

        public Vertex Clone()
        {
            return new Vertex(Pos.Clone(), Normal.Clone());
        }

        // Invert all orientation-specific data (e.g. vertex normal). Called when the
        // orientation of a polygon is flipped.
        public void Flip()
        {
            Normal = Normal.Negated();
        }

        // Create a new vertex between this vertex and `other` by linearly
        // interpolating all properties using a parameter of `t`. Subclasses should
        // override this to interpolate additional properties.
        public Vertex Interpolate(Vertex other, double t)
        {
            return new Vertex(
              Pos.Lerp(other.Pos, t),
              Normal.Lerp(other.Normal, t)
            );
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Normal, Pos);
        }
    };
}