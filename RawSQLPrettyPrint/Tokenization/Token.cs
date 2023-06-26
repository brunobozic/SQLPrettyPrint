namespace RawSQLPrettyPrint.Tokenization
{
    public class Token
    {
        public string Content { get; set; }
        public string Type { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}
