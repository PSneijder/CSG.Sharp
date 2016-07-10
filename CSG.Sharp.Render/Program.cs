using System;

namespace CSG.Sharp
{
    static class Program
    {
        static void Main()
        {
            using (var viewport = new Viewport())
            {
                viewport.Run();
            }
        }
    }
}