namespace RawSQLPrettyPrint.AST
{
    public class AstWrapper
    {
        public string Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<AstNode> Nodes { get; set; }
    }
}