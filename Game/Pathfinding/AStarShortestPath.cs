using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

namespace conscious
{
    public class AStarShortestPath
    {
        private RoomGraph _graph;

        public Dictionary<Vertex, Vertex> cameFrom = new Dictionary<Vertex, Vertex>();
        public Dictionary<Vertex, int> costSoFar = new Dictionary<Vertex, int>();
        public AStarShortestPath(RoomGraph graph)
        {
            _graph = graph;
        }

        private float heuristic(Vector2 position1, Vector2 position2)
        {
            return Math.Abs(position1.X - position2.X) + Math.Abs(position1.Y - position2.Y);
        }

        public List<Vector2> AStarSearch(Vector2 start, Vector2 goal)
        {
            _graph.SetStartGoal(start, goal);

            cameFrom.Clear();
            costSoFar.Clear();
            PriorityQueue<Vertex> frontier = new PriorityQueue<Vertex>();

            frontier.Enqueue(_graph.Start, 0);
            cameFrom[_graph.Start] = _graph.Start;
            costSoFar[_graph.Start] = 0;

            while(frontier.Count > 0)
            {
                Vertex current = frontier.Dequeue();

                if(current.Equals(_graph.Goal))
                {
                    break;
                }

                foreach(KeyValuePair<Vertex, int> next in current.Neighbors)
                {
                    if(next.Key.Equals(_graph.Goal))
                    {
                        
                    }
                    int newCost = costSoFar[current] + next.Value;
                    if(!costSoFar.ContainsKey(next.Key) || newCost < costSoFar[next.Key])
                    {
                        costSoFar[next.Key] = newCost;
                        float priority = newCost + heuristic(next.Key.RoomPosition, _graph.Goal.RoomPosition);
                        frontier.Enqueue(next.Key, priority);
                        cameFrom[next.Key] = current;
                    }
                }
            }

            // Get the path to walk from the result
            List<Vector2> path = new List<Vector2>();
            Vertex currentV = _graph.Goal;
            while(!currentV.Equals(_graph.Start))
            {
                path.Add(currentV.RoomPosition);
                currentV = cameFrom[currentV];
            }
            // I think we do not need the start in the path
            // path.Add(_graph.Start);
            path.Reverse();
            return path;
        }
    }
}