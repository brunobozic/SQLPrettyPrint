using RawSQLPrettyPrint.Tokenization;

namespace RawSQLPrettyPrint.Formatters.General
{
    using RawSQLPrettyPrint.AST;
    using RawSQLPrettyPrint.ConfigurationOptions;
    using RawSQLPrettyPrint.Formatters;
    using System.Collections.Generic;

    public class AlignFirstWordOfClauseFormatter : ISqlFormatter
    {
        private const int IndentationSpaces = 2;

        private readonly FirstWordOfClauseAlignmentOption alignmentOption;

        public AlignFirstWordOfClauseFormatter(FirstWordOfClauseAlignmentOption option)
        {
            alignmentOption = option;
        }

        public int Order => 2;

        public string Format(string sql, List<Token> tokens, AstNode astRoot, WordCaseOption caseOption, FirstWordOfClauseAlignmentOption alignOption)
        {
            return FormatSql(sql, astRoot, alignOption, caseOption);
        }

        public string FormatSql(string sql, AstNode astRoot, FirstWordOfClauseAlignmentOption alignOption, WordCaseOption caseOption)
        {
            bool isRightAligned = alignOption == FirstWordOfClauseAlignmentOption.Right;

            // Check if the AST root node is a SelectStatement
            if (astRoot.Name == "SelectStatement")
            {
                // Find the "SelectList" and "FromClause" nodes recursively
                AstNode selectListNode = FindClauseNode("SelectStatement", astRoot);
                AstNode fromClauseNode = FindClauseNode("FromClause", astRoot);
                AstNode whereClauseNode = FindClauseNode("WhereClause", astRoot);

                var addedIndentations = 0;

                if (selectListNode != null && fromClauseNode != null)
                {
                    // Determine the indentation based on alignment
                    string fromIndentation = isRightAligned ? new string(' ', IndentationSpaces) : "";
                    string whereIndentation = isRightAligned ? new string(' ', IndentationSpaces - 1) : "";

                    // Append "SELECT" without indentation
                    string selectWord = sql.Substring(selectListNode.Start, selectListNode.Start + 6);
                    sql = sql.Remove(selectListNode.Start, selectWord.Length).Insert(selectListNode.Start, selectWord);

                    // Cut out the "FROM" keyword from the original SQL, as is
                    string fromWord = sql.Substring(fromClauseNode.Start, fromClauseNode.Start + 4);
                    string fromWordTrimmed = fromWord.Trim();
                    int lostCharacters = fromWord.Length - fromWordTrimmed.Length;
                    string fromIndented = isRightAligned ? $"{Environment.NewLine}{fromIndentation}{fromWordTrimmed}" : $"{Environment.NewLine}{fromWordTrimmed}";

                    if (alignOption == FirstWordOfClauseAlignmentOption.Left)
                    {
                        addedIndentations += fromIndentation.Length;
                        addedIndentations += Environment.NewLine.Length;
                        addedIndentations -= lostCharacters;
                    }
                    else
                    {
                        addedIndentations += fromIndentation.Length;
                        addedIndentations += Environment.NewLine.Length;
                        addedIndentations -= lostCharacters;
                    }

                    sql = sql.Remove(fromClauseNode.Start - lostCharacters, fromWord.Length).Insert(fromClauseNode.Start - lostCharacters, fromIndented);

                    // Check if there is a "WhereClause" node
                    if (whereClauseNode != null)
                    {
                        // Cut out the "WHERE" keyword from the original SQL, as is
                        int whereStart = whereClauseNode.Start + addedIndentations;
                        int whereLength = ((whereClauseNode.End + addedIndentations) - whereStart);
                        string whereWord = sql.Substring(whereStart, whereLength);
                        string whereTrimmed = whereWord.Trim();
                        var lostCharactersInWhere = (whereWord.Length - whereTrimmed.Length);

                        if (alignOption == FirstWordOfClauseAlignmentOption.Left)
                        {
                            whereIndentation = "";
                        }
                        else
                        {
                            whereIndentation = " ";
                        }

                        string whereIndented = isRightAligned ? $"{Environment.NewLine}{whereIndentation}{whereTrimmed}" : $"{Environment.NewLine}{whereTrimmed}";

                        sql = sql.Remove(whereStart, whereLength).Insert(whereStart- lostCharactersInWhere, whereIndented);
                    }
                }
            }

            return sql;
        }

        public string FormatSqlAlt(string sql, AstNode astRoot, bool isRightAligned, WordCaseOption caseOption)
        {
            int offset = 0; // Initialize the offset
            string fromIndentation = isRightAligned ? new string(' ', IndentationSpaces) : "";
            string whereIndentation = isRightAligned ? new string(' ', IndentationSpaces - 1) : "";

            // Check if the AST root node is a SelectStatement
            if (astRoot.Name == "SelectStatement")
            {
                // Find the "SelectList" and "FromClause" nodes recursively
                AstNode selectListNode = FindClauseNode("SelectStatement", astRoot);
                AstNode fromClauseNode = FindClauseNode("FromClause", astRoot);
                AstNode whereClauseNode = FindClauseNode("WhereClause", astRoot);

                if (selectListNode != null && fromClauseNode != null)
                {
                    // Append "SELECT" without indentation
                    string selectWord = sql.Substring(selectListNode.Start, selectListNode.End - selectListNode.Start);
                    string selectTrimmed = selectWord.Trim();
                    sql = sql.Remove(selectListNode.Start, selectWord.Length).Insert(selectListNode.Start, selectTrimmed);
                    offset += selectWord.Length - selectTrimmed.Length; // Update the offset

                    // Cut out the "FROM" keyword from the original SQL
                    string fromWord = sql.Substring(fromClauseNode.Start, fromClauseNode.End - fromClauseNode.Start);
                    string fromTrimmed = fromWord.Trim();
                    sql = sql.Remove(fromClauseNode.Start, fromWord.Length).Insert(fromClauseNode.Start, fromTrimmed);
                    offset += fromWord.Length - fromTrimmed.Length; // Update the offset

                    // Check if there is a "WhereClause" node
                    if (whereClauseNode != null)
                    {
                        // Cut out the "WHERE" keyword from the original SQL
                        int whereStart = whereClauseNode.Start - offset; // Adjust the start position
                        int whereLength = whereClauseNode.End - whereClauseNode.Start + 1;
                        string whereWord = sql.Substring(whereStart, whereLength);
                        string whereTrimmed = whereWord.Trim();
                        sql = sql.Remove(whereStart, whereLength).Insert(whereStart, whereTrimmed);
                        offset += whereWord.Length - whereTrimmed.Length; // Update the offset
                    }
                }
            }

            // Apply the appropriate indentation to the "FROM" and "WHERE" clauses
            sql = sql.Replace("FROM", $"{Environment.NewLine}{fromIndentation}FROM");
            sql = sql.Replace("WHERE", $"{Environment.NewLine}{whereIndentation}WHERE");

            return sql;
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