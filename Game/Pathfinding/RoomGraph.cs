using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace conscious
{
    /// <summary>Class <c>RoomGraph</c> holds data and logic
    /// to generate a graph from the current room the player is in with all bounding boxes of Things in the room as vertices.
    /// Given a starting point, the protagonist is at, the graph is evaluated for walkable paths 
    /// to vertices and the length of an edge between vertices.
    /// </summary>
    ///
    public class RoomGraph
    {
        private List<Rectangle> _boundingBoxes = new List<Rectangle>();

        List<Vertex> Graph = new List<Vertex>();
        public Vertex Start = null;
        public Vertex Goal = null;

        private float _minRoomLimitX;
        private float _maxRoomLimitX;
        private float _minRoomLimitY;
        private float _maxRoomLimitY;

        public RoomGraph(){ }

        // Create the Graph of the room, taking into account all bounding boxes of Things in the Room
        // as well as the Rooms dimensions.
        public void GenerateRoomGraph(List<Rectangle> boundingBoxes, float minX, float maxX, float minY, float maxY)
        {
            _boundingBoxes = boundingBoxes;
            Graph.Clear();
            _minRoomLimitX = minX;
            _minRoomLimitY = minY;
            _maxRoomLimitX = maxX;
            _maxRoomLimitY = maxY;
            
            foreach(Rectangle bb in boundingBoxes)
            {
                // add padding for bounding boxes
                int padding = 50;
                Graph.Add(new Vertex(bb.X - padding, bb.Y - padding));
                Graph.Add(new Vertex(bb.X + bb.Width + padding, bb.Y - padding));
                Graph.Add(new Vertex(bb.X - padding, bb.Y + bb.Height + padding));
                Graph.Add(new Vertex(bb.X + bb.Width + padding, bb.Y + bb.Height + padding));
            }

            List<Vertex> verticesChecked = new List<Vertex>();
            foreach(Vertex v1 in Graph)
            {
                foreach(Vertex v2 in Graph)
                {
                    if(v1 != v2 && !verticesChecked.Contains(v2))
                    {
                        evaluateLink(v1, v2);
                    }
                }
                verticesChecked.Add(v1);
            }
        }

        // Set start and goal vertex in the room and remove those from the Graph.
        public void SetStartGoal(Vector2 start, Vector2 goal)
        {
            if(Start != null && Goal != null)
            {
                removeVertex(Start);
                removeVertex(Goal);
            }

            Start = new Vertex(start.X, start.Y);
            Goal = new Vertex(goal.X, goal.Y);
            evaluateLink(Start, Goal);

            foreach(Vertex v in Graph)
            {
                evaluateLink(Start, v);
                evaluateLink(Goal, v);
            }
        }

        // Evaluate a link in terms of if it is walkable (or something is in the way) 
        // and a evenly weighted cost of 1.
        private void evaluateLink(Vertex v1, Vertex v2)
        {
            if(!vertexNotWalkable(v1) &&
               !vertexNotWalkable(v2) &&
               !isNotVisible(v1.RoomPosition, v2.RoomPosition, _boundingBoxes))
            {
                v1.AddNeighbor(v2, 1);
                v2.AddNeighbor(v1, 1);
            }
        }

        // Evaluate if a path to a vertex is outside of the Rooms dimensions.
        private bool vertexNotWalkable(Vertex v)
        {
            if(v.RoomPosition.X < _minRoomLimitX || 
               v.RoomPosition.X > _maxRoomLimitX ||
               v.RoomPosition.Y < _minRoomLimitY ||
               v.RoomPosition.Y > _maxRoomLimitY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Remove a vertex from the Room Graph.
        private void removeVertex(Vertex v)
        {
            if(Graph.Contains(v))
            {
                Graph.Remove(v);
            }
            foreach(Vertex gv in Graph)
            {
                if(gv.Neighbors.ContainsKey(v))
                {
                    gv.Neighbors.Remove(v);
                }
            }
        }

        // Calculate the visibility of bounding boxes
        private bool isNotVisible(Vector2 v1, Vector2 v2, List<Rectangle> boundingBoxes)
        {
            float slope = calculateSlope(v1.X, v1.Y, v2.X, v2.Y);
            float intercept = calculateIntercept(v1.X, v1.Y, slope);

            foreach(Rectangle r in boundingBoxes)
            {
                if(r.Left >= getXMax(v1, v2) || r.Right <= getXMin(v1, v2))
                {
                    continue;
                }
                if(r.Bottom <= getYMin(v1, v2) || r.Top >= getYMax(v1, v2))
                {
                    continue;
                }

                float yAtRectLeft = calculateYforX(r.Left, slope, intercept);
                float yAtRectRight = calculateYforX(r.Right, slope, intercept);
                if(r.Top >= yAtRectLeft && r.Top >= yAtRectRight)
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        private float calculateSlope(float x1, float y1, float x2, float y2)
        {
            float rise = 0f;
            float run = 0f;
            float slope = 0f;
            if(y1 >= y2)
            {
                rise = y1 - y2;
            }
            else
            {
                rise = y2 - y1;
            }
            if(x1 >= x2)
            {
                run = x1 - x2;
            }
            else
            {
                run = x2 - x1;
            }

            if(run != 0)
            {
                slope = rise/run;
            }
            return slope;
        }

        private float calculateIntercept(float x, float y, float slope)
        {
            return y - slope * x;
        }
 
        private float getXMin(Vector2 vertexA, Vector2 vertexB)
        {
            if(vertexA.X <= vertexB.X)
            {
                return vertexA.X;
            }
            else
            {
                return vertexB.X;
            }
        }

        private float getXMax(Vector2 vertexA, Vector2 vertexB)
        {
            if(vertexA.X >= vertexB.X)
            {
                return vertexA.X;
            }
            else
            {
                return vertexB.X;
            }            
        }

        private float getYMin(Vector2 vertexA, Vector2 vertexB)
        {
            if(vertexA.Y <= vertexB.Y)
            {
                return vertexA.Y;
            }
            else
            {
                return vertexB.Y;
            }
        }

        private float getYMax(Vector2 vertexA, Vector2 vertexB)
        {
            if(vertexA.Y >= vertexB.Y)
            {
                return vertexA.Y;
            }
            else
            {
                return vertexB.Y;
            }
        }

        private float calculateYforX(float x, float slope, float intercept)
        {
            return slope*x+intercept;
        }
    }
}