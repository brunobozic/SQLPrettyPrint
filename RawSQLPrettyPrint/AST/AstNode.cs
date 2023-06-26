namespace RawSQLPrettyPrint.AST
{
    public class AstNode
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public List<AstNode> Children { get; set; }
    }

}
