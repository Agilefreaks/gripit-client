using System;

namespace gripit_client
{
    public class ForceProjection : EventArgs
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string Id { get; set; }

        public ForceProjection(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}