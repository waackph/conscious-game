using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>Vertex</c> holds data for a position in the given Room 
    /// and is e.g. representing a vertex of a bounding box.
    /// </summary>
    ///
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