using Newtonsoft.Json;

namespace conscious
{
    public class Edge
    {
        [JsonProperty]
        private int _nextNodeId;
        [JsonProperty]
        private string _dialogLine;
        public MoodState MoodDependence;

        public Edge(int nextNodeId, string dialogLine, MoodState moodDependence){
            _nextNodeId = nextNodeId;
            _dialogLine = dialogLine;
            MoodDependence = moodDependence;
        }

        public string GetLine(){
            return _dialogLine;
        }

        public int getNextNodeId(){
            return _nextNodeId;
        }
    }
}