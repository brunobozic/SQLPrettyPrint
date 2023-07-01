using Newtonsoft.Json;
using RawSQLPrettyPrint.AST;
using RawSQLPrettyPrint.ConfigurationOptions;
using RawSQLPrettyPrint.Formatters.General;
using RawSQLPrettyPrint.Tokenization;

namespace RawSQLPrettyPrint.Tests.Unit.ReservedWordCapitalization
{
    public class ReservedWordCapitalizationCapitalizationSelectTest
    {
        [Fact]
        public void ReservedWordCapitalizationFormatter_Format_ReturnsFormattedSql()
        {
            // Arrange
            var formatter = new ReservedWordCapitalizationFormatter();
            var inputSql = "select * from x where a = 'robert'; drop table students;-- 'smith);";
            var expectedSql = "SELECT * FROM x WHERE a = 'robert'; DROP TABLE students;-- 'smith);";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("ReservedWordCapitalization/SimpleSelect/KeywordCapitalizationSelectTest1.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("ReservedWordCapitalization/SimpleSelect/ListOfTokensSimpleSelect.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Left);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }

        [Fact]
        public void ReservedWordCapitalizationFormatter_Format_ReturnsFormattedSql_Complex()
        {
            // Arrange
            var formatter = new ReservedWordCapitalizationFormatter();
            var inputSql = "with cte as (select col1, col2, sum(col3) as total from table1 where col4 = 'value' group by col1, col2) select t1.col1, t2.col2, t3.col3 from cte join table2 as t1 on t1.col1 = cte.col1 left join table3 as t2 on t2.col2 = cte.col2 right join table4 as t3 on t3.col3 = cte.total where t1.col1 > 100 group by t1.col1, t2.col2, t3.col3 having count(*) > 5 order by t1.col1 desc, t2.col2 asc;";
            var expectedSql = "WITH cte AS (SELECT col1, col2, SUM(col3) AS total FROM table1 WHERE col4 = 'value' GROUP BY col1, col2) SELECT t1.col1, t2.col2, t3.col3 FROM cte JOIN table2 AS t1 ON t1.col1 = cte.col1 LEFT JOIN table3 AS t2 ON t2.col2 = cte.col2 RIGHT JOIN table4 AS t3 ON t3.col3 = cte.total WHERE t1.col1 > 100 GROUP BY t1.col1, t2.col2, t3.col3 HAVING COUNT(*) > 5 ORDER BY t1.col1 DESC, t2.col2 ASC;";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("ReservedWordCapitalization/ComplexSelect/KeywordCapitalizationSelectTest2.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("ReservedWordCapitalization/ComplexSelect/ListOfTokensComplexSelect.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Left);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }

        [Fact]
        public void ReservedWordCapitalizationFormatter_Format_ReturnsFormattedSql_Complex_Two()
        {
            // Arrange
            var formatter = new ReservedWordCapitalizationFormatter();
            var inputSql = "select distinct top 10 case when Salary > 5000 then 'High' when Salary > 3000 then 'Medium' else 'Low' end as SalaryCategory, FirstName, LastName, sum(Salary) over (partition by Department order by Salary desc) as DepartmentTotal, count(*) over (partition by Department) as EmployeeCount from Employees inner join Departments on Employees.DepartmentId = Departments.DepartmentId where HireDate between '2022-01-01' and '2022-12-31' and Title in ('Manager', 'Supervisor') and FirstName like 'J%' and (LastName = 'Smith' or LastName = 'Johnson') group by SalaryCategory, FirstName, LastName, Department having EmployeeCount > 5 order by DepartmentTotal desc;";
            var expectedSql = "SELECT DISTINCT TOP 10 CASE WHEN Salary > 5000 THEN 'High' WHEN Salary > 3000 THEN 'Medium' ELSE 'Low' END AS SalaryCategory, FirstName, LastName, SUM(Salary) OVER (PARTITION BY Department ORDER BY Salary DESC) AS DepartmentTotal, COUNT(*) OVER (PARTITION BY Department) AS EmployeeCount FROM Employees INNER JOIN Departments ON Employees.DepartmentId = Departments.DepartmentId WHERE HireDate BETWEEN '2022-01-01' AND '2022-12-31' AND Title IN ('Manager', 'Supervisor') AND FirstName LIKE 'J%' AND (LastName = 'Smith' OR LastName = 'Johnson') GROUP BY SalaryCategory, FirstName, LastName, Department HAVING EmployeeCount > 5 ORDER BY DepartmentTotal DESC;";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("ReservedWordCapitalization/VeryComplexSelect/VeryComplexSelectAST.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("ReservedWordCapitalization/VeryComplexSelect/VeryComplexSelectTokens.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Left);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }

        [Fact]
        public void ReservedWordCapitalizationFormatter_Format_ReturnsFormattedSql_Complex_Three()
        {
            // Arrange
            var formatter = new ReservedWordCapitalizationFormatter();
            var inputSql = "select concat(upper('hello'), lower('WORLD'), substring('example', 2, 4)) as Result from table1;";
            var expectedSql = "SELECT CONCAT(UPPER('hello'), LOWER('WORLD'), SUBSTRING('example', 2, 4)) AS Result FROM table1;";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("ReservedWordCapitalization/ComplexThree/VeryComplexThreeSelectAST.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("ReservedWordCapitalization/ComplexThree/VeryComplexThreeSelectTokens.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Left);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }

        [Fact]
        public void ReservedWordCapitalizationFormatter_Format_ReturnsFormattedSql_Complex_Four()
        {
            // Arrange
            var formatter = new ReservedWordCapitalizationFormatter();
            var inputSql = "select col1 from (select col2 from table1) as t1;";
            var expectedSql = "SELECT col1 FROM (SELECT col2 FROM table1) AS t1;";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("ReservedWordCapitalization/ComplexFour/VeryComplexFourSelectAST.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("ReservedWordCapitalization/ComplexFour/VeryComplexFourSelectTokens.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Left);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }

        [Fact]
        public void ReservedWordCapitalizationFormatter_Format_ReturnsFormattedSql_Complex_Five()
        {
            // Arrange
            var formatter = new ReservedWordCapitalizationFormatter();
            var inputSql = "select ((col1 + (col2 * col3)) - ((col4 / col5) + col6)) as Result from table1;";
            var expectedSql = "SELECT ((col1 + (col2 * col3)) - ((col4 / col5) + col6)) AS Result FROM table1;";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("ReservedWordCapitalization/ComplexFive/VeryComplexFiveSelectAST.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("ReservedWordCapitalization/ComplexFive/VeryComplexFiveSelectTokens.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Left);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }

        [Fact]
        public void ReservedWordCapitalizationFormatter_Format_ReturnsFormattedSql_Complex_Six()
        {
            // Arrange
            var formatter = new ReservedWordCapitalizationFormatter();
            var inputSql = "select (count(distinct col1) / (sum(col2) + avg(col3))) as Result from table1;";
            var expectedSql = "SELECT (COUNT(DISTINCT col1) / (SUM(col2) + AVG(col3))) AS Result FROM table1;";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("ReservedWordCapitalization/ComplexSix/VeryComplexSixSelectAST.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("ReservedWordCapitalization/ComplexSix/VeryComplexSixSelectTokens.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Left);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }



        [Fact]
        public void ReservedWordCapitalizationFormatter_Format_ReturnsFormattedSql_Complex_Seven()
        {
            // Arrange
            var formatter = new ReservedWordCapitalizationFormatter();
            var inputSql = "select * from stuff where(col1 > 0)and((col2 = 'value')or(col3 IS NULL));";
            var expectedSql = "SELECT * FROM stuff WHERE(col1 > 0)AND((col2 = 'value')OR(col3 IS NULL));";

            // Read the token data from the JSON file
            var astJson = File.ReadAllText("ReservedWordCapitalization/ComplexSeven/VeryComplexSevenSelectAST.json");
            var astNodePayload = JsonConvert.DeserializeObject<AstNodePayload>(astJson);
            var astRoot = astNodePayload?.Value?.Nodes?.FirstOrDefault();

            var json = File.ReadAllText("ReservedWordCapitalization/ComplexSeven/VeryComplexSevenSelectTokens.json");
            var tokenizationServiceResponsePayload = JsonConvert.DeserializeObject<TokenizationServiceResponsePayload>(json);
            var tokens = tokenizationServiceResponsePayload?.Value?.Tokens;

            // Act
            var formattedSql = formatter.Format(inputSql, tokens, astRoot, WordCaseOption.Uppercase, FirstWordOfClauseAlignmentOption.Left);

            // Assert
            Assert.Equal(expectedSql, formattedSql);
        }

    }
}