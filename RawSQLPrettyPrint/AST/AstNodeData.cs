namespace RawSQLPrettyPrint.AST
{
    public class AstNodeData
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public List<AstNodeData> Children { get; set; }
    }
}
