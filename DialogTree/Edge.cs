using Newtonsoft.Json;

namespace conscious
{
    public class Edge
    {
        [JsonProperty]
        private int _nextNodeId;
        [JsonProperty]
        private string _dialogLine;

        public Edge(int nextNodeId, string dialogLine){
            _nextNodeId = nextNodeId;
            _dialogLine = dialogLine;
        }

        public string GetLine(){
            return _dialogLine;
        }

        public int getNextNodeId(){
            return _nextNodeId;
        }
    }
}