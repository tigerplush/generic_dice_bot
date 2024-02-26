namespace GenericDiceBot.Utilities
{
    public class DiceParserException : Exception
    {
        public DiceParserException() : base()
        {

        }
        
        public DiceParserException(string? message) : base(message)
        {

        }

        public DiceParserException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
