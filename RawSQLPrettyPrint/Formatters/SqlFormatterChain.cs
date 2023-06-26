using RawSQLPrettyPrint.AST;
using RawSQLPrettyPrint.ConfigurationOptions;
using RawSQLPrettyPrint.Tokenization;

namespace RawSQLPrettyPrint.Formatters
{
    public class SqlFormatterChain
    {
        private readonly List<ISqlFormatter> formatters;

        public SqlFormatterChain(List<ISqlFormatter> formatters)
        {
            this.formatters = formatters.OrderBy(f => f.Order).ToList();
        }

        public string Format(string sql, AstNode astRoot, List<Token> tokens, WordCaseOption caseOption)
        {
            foreach (var formatter in formatters)
            {
                sql = formatter.Format(sql, tokens, astRoot, caseOption);
            }

            return sql;
        }
    }
}