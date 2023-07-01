using RawSQLPrettyPrint.AST;
using RawSQLPrettyPrint.ConfigurationOptions;
using RawSQLPrettyPrint.Tokenization;

namespace RawSQLPrettyPrint.Formatters.General
{
    public class ReservedWordCapitalizationFormatter : ISqlFormatter
    {
        public int Order => 1;

        public string Format(string sql, List<Token> tokens, AstNode astRoot, WordCaseOption caseOption, FirstWordOfClauseAlignmentOption alignOption)
        {
            var formattedSql = sql;

            if (tokens != null)
            {
                var reservedKeywords = GetReservedKeywords(tokens);

                if (caseOption == WordCaseOption.Uppercase)
                {
                    formattedSql = UppercaseKeywords(reservedKeywords, tokens, caseOption);
                }
                else if (caseOption == WordCaseOption.Uppercase)
                {
                    formattedSql = LowercaseKeywords(reservedKeywords, tokens, caseOption);
                }
            }

            return formattedSql;
        }

        private HashSet<string> GetReservedKeywords(List<Token> tokens)
        {
            var result = tokens
                .Where(token => token.Type == "ReservedKeyword")
                .Select(token => token.Content.ToLowerInvariant())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            result.Add("sum"); // TODO: this should be configurable/optional
            result.Add("count"); // TODO: this should be configurable/optional
            result.Add("asc"); // TODO: this should be configurable/optional
            result.Add("desc"); // TODO: this should be configurable/optional
            result.Add("partition"); // TODO: this should be configurable/optional
            result.Add("concat"); // TODO: this should be configurable/optional
            result.Add("upper"); // TODO: this should be configurable/optional
            result.Add("lower"); // TODO: this should be configurable/optional
            result.Add("substring"); // TODO: this should be configurable/optional
            result.Add("sum"); // TODO: this should be configurable/optional
            result.Add("avg"); // TODO: this should be configurable/optional

            return result;
        }

        private string UppercaseKeywords(HashSet<string> reservedKeywords, List<Token> tokens, WordCaseOption caseOption)
        {
            foreach (Token token in tokens)
            {
                if (reservedKeywords.Contains(token.Content.ToLower()))
                {
                    token.Content = token.Content.ToUpper();
                }
            }

            return string.Join("", tokens.ConvertAll(token => token.Content));
        }

        private string LowercaseKeywords(HashSet<string> reservedKeywords, List<Token> tokens, WordCaseOption caseOption)
        {
            foreach (Token token in tokens)
            {
                if (reservedKeywords.Contains(token.Content.ToLower()))
                {
                    token.Content = token.Content.ToLower();
                }
            }

            return string.Join("", tokens.ConvertAll(token => token.Content));
        }
    }
}