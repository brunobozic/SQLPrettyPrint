using System.Text;
using RawSQLPrettyPrint.AST;
using RawSQLPrettyPrint.ConfigurationOptions;

namespace RawSQLPrettyPrint.Formatters.General
{
    public class ClauseElementPlacementFormatter 
    {
        public int Order => 3;

        public ClauseElementPlacementOption PlacementOption { get; set; }

        public ClauseElementPlacementFormatter(ClauseElementPlacementOption placementOption)
        {
            PlacementOption = placementOption;
        }

        public string Format(string sql, AstNode astRoot)
        {
            if (PlacementOption == ClauseElementPlacementOption.NoChange)
            {
                return sql; // Return the original SQL if no formatting change is required
            }

            var formattedSql = sql;

            // Apply the formatting based on the PlacementOption
            switch (PlacementOption)
            {
                case ClauseElementPlacementOption.SameLine:
                    formattedSql = FormatAstTree(astRoot, Environment.NewLine, " ");
                    break;
                case ClauseElementPlacementOption.NewLine:
                    formattedSql = FormatAstTree(astRoot, Environment.NewLine, "");
                    break;
            }

            return formattedSql;
        }

        private string FormatAstTree(AstNode node, string lineBreak, string separator)
        {
            var formattedSql = new StringBuilder();
            FormatAstNode(node, formattedSql, lineBreak, separator);
            return formattedSql.ToString();
        }

        private void FormatAstNode(AstNode node, StringBuilder formattedSql, string lineBreak, string separator)
        {
            if (node == null)
                return;

            if (node.Children.Count > 0)
            {
                foreach (var child in node.Children)
                {
                    FormatAstNode(child, formattedSql, lineBreak, separator);
                }
            }
            else
            {
                formattedSql.Append(node.Name);

                if (node.Type == NodeType.Name.ToString())
                {
                    formattedSql.Append(separator);
                }
                else if (node.Type == NodeType.Clause.ToString())
                {
                    formattedSql.Append(lineBreak);
                }
            }
        }
    }


}
