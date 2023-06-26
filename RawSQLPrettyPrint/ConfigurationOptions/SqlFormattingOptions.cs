namespace RawSQLPrettyPrint.ConfigurationOptions
{
    public class SqlFormattingOptions
    {
        public WordCaseOption WordCase { get; set; }
        public FirstWordOfClauseAlignmentOption QueryAlignment { get; set; }
        public ClauseElementPlacementOption ClauseElementPlacement { get; set; }
    }

}
