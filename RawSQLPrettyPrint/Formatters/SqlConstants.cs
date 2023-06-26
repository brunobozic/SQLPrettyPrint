namespace RawSQLPrettyPrint.Formatters
{
    public static class SqlConstants
    {
        public static class Length
        {
            public const int SelectKeyword = 6; // Length of "SELECT"
            public const int FromKeyword = 4; // Length of "FROM"
            public const int WhereKeyword = 5; // Length of "WHERE"
                                               // Add more length constants as needed
        }

        public enum SqlKeyword
        {
            Select,
            From,
            Where,
            // Add more SQL keywords as needed
        }
    }

}
