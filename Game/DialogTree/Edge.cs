using Newtonsoft.Json;

namespace conscious
{
    /// <summary>Class <c>Edge</c> holds data of an option in a dialog with another character
    /// that the player can choose as an answer. An option refers to the next node
    /// (i.e. something the NPC character says) and indicates if the option can be chosen
    /// given a needed mood the protagonist must be in.
    /// </summary>
    ///
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