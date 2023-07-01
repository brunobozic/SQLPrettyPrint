using RawSQLPrettyPrint.AST;
using RawSQLPrettyPrint.ConfigurationOptions;
using RawSQLPrettyPrint.Tokenization;

namespace RawSQLPrettyPrint.Formatters
{

    public interface ISqlFormatter
    {
        int Order { get; }
        string Format(string sql, List<Token> tokens, AstNode astRoot, WordCaseOption caseOption, FirstWordOfClauseAlignmentOption alignOption)  ;
    }
}
