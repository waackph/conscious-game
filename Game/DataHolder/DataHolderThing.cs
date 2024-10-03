namespace conscious
{
    public class DataHolderThing : DataHolderEntity
    {
        public int Id { get; set; }
        public DataHolderThoughtNode Thought { get; set; }
        public bool IsInInventory { get; set; }
        public DataHolderThoughtNode EventThought { get; set; }
        public string LightMaskFilePath { get; set; }
        public string DepressedTexture { get; set; }
        public string ManicTexture { get; set; }
    }
}
