using Newtonsoft.Json.Linq;

namespace RawSQLPrettyPrint.Tokenization
{
    public class TokenizationServiceResponseValue
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<Token> Tokens { get; set; }
    }
}
