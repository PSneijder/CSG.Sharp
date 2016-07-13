using System;

namespace CSG.Sharp
{
    // Represents a 3D vector.
    // 
    // Example usage:
    // 
    //     new Vector(1, 2, 3);
    //     new Vector([1, 2, 3]);
    //     new Vector({ x: 1, y: 2, z: 3 });
    public struct Vector
    {
        public static Vector Backward { get { return new Vector(0, 0, 1); } }
        public static Vector Down { get { return new Vector(0, -1, 0); } }
        public static Vector Forward { get { return new Vector(0, 0, -1); } }
        public static Vector Left { get { return new Vector(-1, 0, 0); } }
        public static Vector One { get { return new Vector(1, 1, 1); } }
        public static Vector Right { get { return new Vector(1, 0, 0); } }
        public static Vector UnitX { get { return new Vector(1, 0, 0); } }
        public static Vector UnitY { get { return new Vector(0, 1, 0); } }
        public static Vector UnitZ { get { return new Vector(0, 0, 1); } }
        public static Vector Up { get { return new Vector(0, 1, 0); } }
        public static Vector Zero { get { return new Vector(0, 0, 0); } }

        public double x { get; private set; }
        public double y { get; private set; }
        public double z { get; private set; }

        public Vector(Vector v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector Clone()
        {
            return new Vector(x, y, z);
        }

        public Vector Negated()
        {
            return new Vector(-x, -y, -z);
        }
        public Vector Plus(Vector a)
        {
            return new Vector(x + a.x, y + a.y, z + a.z);
        }

        public Vector Minus(Vector a)
        {
            return new Vector(x - a.x, y - a.y, z - a.z);
        }

        public Vector Times(double a)
        {
            return new Vector(x * a, y * a, z * a);
        }

        public Vector DividedBy(double a)
        {
            return new Vector(x / a, y / a, z / a);
        }

        public double Dot(Vector a)
        {
            return x * a.x + y * a.y + z * a.z;
        }

        public Vector Lerp(Vector a, double t)
        {
            return Plus(a.Minus(this).Times(t));
        }

        public double Length()
        {
            return Math.Sqrt(Dot(this));
        }

        public Vector Unit()
        {
            return DividedBy(Length());
        }

        public Vector Cross(Vector a)
        {
            return new Vector(
              y * a.z - z * a.y,
              z * a.x - x * a.z,
              x * a.y - y * a.x
            );
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", x, y, z);
        }

        public static bool operator !=(Vector v1, Vector v2)
        {
            return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        }
        public static bool operator ==(Vector v1, Vector v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Vector other = (Vector)obj;
            return other.x == x && other.y == y && other.z == z;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + x.GetHashCode();
            hash = (hash * 8) + y.GetHashCode();
            hash = (hash * 9) + z.GetHashCode();
            return hash;
        }
    }
}