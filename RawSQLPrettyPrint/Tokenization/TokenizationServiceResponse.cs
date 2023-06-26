namespace RawSQLPrettyPrint.Tokenization
{
    public class TokenizationServiceResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<Token> Tokens { get; set; }
    }

    public class TokenizationServiceResponsePayload
    {
        public TokenizationServiceResponse Value { get; set; }
    }
}
