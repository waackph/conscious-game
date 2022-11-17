using System;
using System.Collections.Generic;

namespace conscious
{
    /// <summary>Class <c>PriorityQueue</c> implements a priority queue data structure
    /// that is used for the A* path finding algorithm.
    /// </summary>
    ///
    public class PriorityQueue<T>
    {
        private List<Tuple<T, float>> elements = new List<Tuple<T, float>>();

        public int Count
        {
            get { return elements.Count; }
        }
        
        public void Enqueue(T item, float priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++) {
                if (elements[i].Item2 < elements[bestIndex].Item2) {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}