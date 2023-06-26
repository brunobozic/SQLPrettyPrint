using RawSQLPrettyPrint.Tokenization;

namespace RawSQLPrettyPrint.Formatters.General
{
    using System.Collections.Generic;
    using System.Text;
    using RawSQLPrettyPrint.AST;
    using RawSQLPrettyPrint.ConfigurationOptions;
    using RawSQLPrettyPrint.Formatters;

    public class AlignFirstWordOfClauseFormatter : ISqlFormatter
    {
        private const int IndentationSpaces = 2;

        public AlignFirstWordOfClauseFormatter(FirstWordOfClauseAlignmentOption option)
        {
        }

        public int Order => 2;

        public string Format(string sql, List<Token> tokens, AstNode astRoot, WordCaseOption caseOption)
        {
            // Call the FormatSql method to format the SQL
            return FormatSql(sql, astRoot, isRightAligned: true);
        }

        public string FormatSql(string sql, AstNode astRoot, bool isRightAligned)
        {
            StringBuilder sb = new StringBuilder();

            // Check if the AST root node is a SelectStatement
            if (astRoot.Name == "SelectStatement")
            {
                // Find the "SelectList" and "FromClause" nodes recursively
                AstNode selectListNode = FindClauseNode("SelectList", astRoot);
                AstNode fromClauseNode = FindClauseNode("FromClause", astRoot);
                AstNode whereClauseNode = FindClauseNode("WhereClause", astRoot);

                if (selectListNode != null && fromClauseNode != null)
                {
                    // Append "SELECT" without indentation and a space
                    sb.Append("SELECT ");
                    var ap1 = sql.Substring(selectListNode.Start, selectListNode.End - selectListNode.Start);
                    sb.Append(ap1);

                    // Determine the indentation based on alignment
                    string indentation = isRightAligned ? new string(' ', IndentationSpaces) : "";

                    // Start the "FROM" keyword on a new line with the appropriate indentation
                    sb.AppendLine();
                    sb.Append(indentation);

                    // Append the "FROM" keyword and table name
                    var fromToken = "FROM ";
                    sb.Append(fromToken);
                    var ap2 = sql.Substring(fromClauseNode.Start + fromToken.Length, (fromClauseNode.End + 1) - (fromClauseNode.Start + fromToken.Length));
                    sb.Append(ap2);

                    // Check if there is a "WhereClause" node
                    if (whereClauseNode != null)
                    {
                        // Start the "WHERE" keyword on a new line with the appropriate indentation
                        sb.AppendLine();
                        sb.Append(indentation);

                        // Append the "WHERE" keyword and condition
                        var whereToken = "WHERE ";
                        sb.Append(whereToken);
                        var ap3 = sql.Substring(whereClauseNode.Start + whereToken.Length, (whereClauseNode.End + 1) - (whereClauseNode.Start + whereToken.Length));
                        sb.Append(ap3);
                    }
                }
            }

            return sb.ToString();
        }

        private AstNode FindClauseNode(string clauseName, AstNode astNode)
        {
            if (astNode.Name.ToUpper() == clauseName.ToUpper())
            {
                return astNode;
            }

            foreach (var childNode in astNode.Children)
            {
                var foundNode = FindClauseNode(clauseName, childNode);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null;
        }
    }
}