using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace conscious
{
    public class Vertex
    {
        public Vector2 RoomPosition;
        public Dictionary<Vertex, int> Neighbors { get; }

        public Vertex(float x, float y)
        {
            RoomPosition = new Vector2(x, y);
            Neighbors = new Dictionary<Vertex, int>();
        }

        public void AddNeighbor(Vertex v, int cost)
        {
            Neighbors.Add(v, cost);
        }
    }
}