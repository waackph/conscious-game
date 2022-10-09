using System.Collections.Generic;
using Newtonsoft.Json;

namespace conscious
{
    public class Node
    {
        public int Id { get; set; }
        [JsonProperty]
        public List<Edge> _edges;
        [JsonProperty]
        private string _dialogLine;

        public Node(int id, string dialogLine, List<Edge> edges){
            Id = id;
            _edges = edges;
            _dialogLine = dialogLine;
        }

        public int GetId(){
            return Id;
        }

        public void AddEdge(Edge edge){
            _edges.Add(edge);
        }

        public void RemoveEdge(Edge edge){
            if(_edges.Contains(edge))
                _edges.Remove(edge);
        }

        public List<Edge> GetEdges(){
            return _edges;
        }

        public bool HasEdges(){
            bool hasEdge = false;
            if(_edges.Count > 0)
            {
                hasEdge = true;
            }
            return hasEdge;
        }

        public string GetLine(){
            return _dialogLine;
        }
    }
}