using Newtonsoft.Json;
using RawSQLPrettyPrint.AST;
using RawSQLPrettyPrint.ConfigurationOptions;
using RawSQLPrettyPrint.Formatters.General;
using RawSQLPrettyPrint.Tokenization;

namespace RawSQLPrettyPrint.Tests.Unit.AligningTheFirstWordOfClause
{
    public class AligningTheFirstWordOfClauseSelectTest
    {
        [Fact]
        public void Simple_Stuff_Right_Align()
        {
            // Arrange
            var formatter = new AlignFirstWordOfClauseFormatter(FirstWordOfClauseAlignmentOption.Left);
            var inputSql = "SELECT MAX(q4) FROM my_staff_size WHERE username = 'Ivan';";
            var expectedSql = "SELECT MAX(q4)\r\n  FROM my_staff_size\r\n WHERE username = 'Ivan';";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("AligningTheFirstWordOfClause/SimpleSelect/AligningTheFirstWordOfClauseSelectTest1.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("AligningTheFirstWordOfClause/SimpleSelect/ListOfTokensSimpleSelect.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Right);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }

        [Fact]
        public void AligningTheFirstWordOfClauseSelectFormatter_Format_Left_ReturnsFormattedSql()
        {
            // Arrange
            var formatter = new AlignFirstWordOfClauseFormatter(FirstWordOfClauseAlignmentOption.Left);
            var inputSql = "SELECT d.dep_id,p.per_id,COUNT(*)AS per_cnt,MAX(m.actual) AS has_actual_template FROM department d JOIN manager m ON d.dep_id = m.dep_id LEFT JOIN person p ON m.per_id = p.per_id, message m LEFT OUTER JOIN mark mk ON m.id = mk.msg_id LEFT JOIN template tp ON m.template_id = tp.id WHERE d.dep_id = m.dep_1 GROUP BY d.dep_id, p.per_i HAVING COUNT(*) >= 2 ORDER BY per_cnt DESC;";
            var expectedSql = @"SELECT d.dep_id,
       p.per_id,
       COUNT(*)      AS per_cnt,
       MAX(m.actual) AS has_actual_template
FROM department d
         JOIN manager m ON d.dep_id = m.dep_id
         LEFT JOIN person p ON m.per_id = p.per_id,
     message m
         LEFT OUTER JOIN mark mk ON m.id = mk.msg_id
         LEFT JOIN template tp ON m.template_id = tp.id
WHERE d.dep_id = m.dep_1
GROUP BY d.dep_id, p.per_i
HAVING COUNT(*) >= 2
ORDER BY per_cnt DESC;";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("AligningTheFirstWordOfClause/SimpleSelect/AligningTheFirstWordOfClauseSelectTest1.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("AligningTheFirstWordOfClause/SimpleSelect/ListOfTokensSimpleSelect.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Right);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }

        [Fact]
        public void AligningTheFirstWordOfClauseSelectFormatter_Format_Right_ReturnsFormattedSql()
        {
            // Arrange
            var formatter = new AlignFirstWordOfClauseFormatter(FirstWordOfClauseAlignmentOption.Right);
            var inputSql = "SELECT d.dep_id,p.per_id,COUNT(*)AS per_cnt,MAX(m.actual) AS has_actual_template FROM department d JOIN manager m ON d.dep_id = m.dep_id LEFT JOIN person p ON m.per_id = p.per_id, message m LEFT OUTER JOIN mark mk ON m.id = mk.msg_id LEFT JOIN template tp ON m.template_id = tp.id WHERE d.dep_id = m.dep_1 GROUP BY d.dep_id, p.per_i HAVING COUNT(*) >= 2 ORDER BY per_cnt DESC;";
            var expectedSql = @"  SELECT d.dep_id,
         p.per_id,
         COUNT(*)      AS per_cnt,
         MAX(m.actual) AS has_actual_templatew
FROM department d
           JOIN manager m ON d.dep_id = m.dep_id
           LEFT JOIN person p ON m.per_id = p.per_id,
       message m
           LEFT OUTER JOIN mark mk ON m.id = mk.msg_id
           LEFT JOIN template tp ON m.template_id = tp.id
 WHERE d.dep_id = m.dep_1
 GROUP BY d.dep_id, p.per_i
HAVING COUNT(*) >= 2
 ORDER BY per_cnt DESC;";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("AligningTheFirstWordOfClause/SimpleSelect/AligningTheFirstWordOfClauseSelectTest1.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("AligningTheFirstWordOfClause/SimpleSelect/ListOfTokensSimpleSelect.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Right);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }
    }

}