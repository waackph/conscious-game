using System.Collections.Generic;

namespace conscious
{
    public class DataHolderCharacter : DataHolderEntity
    {
        public int Id { get; set; }
        public List<Node> TreeStructure { get; set; }
        public string Pronoun { get; set; }
        public string CatchPhrase { get; set; }
        public bool GiveAble { get; set; }
    }
}