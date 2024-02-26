using GenericDiceBot.Dtos.V1;
using System.Security.Cryptography;

namespace GenericDiceBot.Utilities
{
    public static class DiceThrow
    {
        /// <summary>
        /// Parses an input string and returns the resulting dice throws.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="DiceParserException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static (DiceResultDtoV1[], DiceErrorDtoV1[]) Parse(string input)
        {
            List<DiceErrorDtoV1> errors = new List<DiceErrorDtoV1>();
            if(string.IsNullOrEmpty(input))
            {
                errors.Add(new DiceErrorDtoV1()
                {
                    Token = input,
                    Error = "Input cannot be empty or null"
                });
                return (Array.Empty<DiceResultDtoV1>(), errors.ToArray());
            }
            List<DiceResultDtoV1> results = new List<DiceResultDtoV1>();
            input = input.Replace("+", ":+")
                .Replace("-", ":-");
            string[] tokens = input.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string token in tokens)
            {
                string currentToken = token;
                try
                {
                    int operation = 1;
                    if (currentToken.StartsWith("+"))
                    {
                        operation = 1;
                    }
                    else if (currentToken.StartsWith("-"))
                    {
                        operation = -1;
                    }
                    else
                    {
                        currentToken = $"+{token}";
                    }
                    if (currentToken.Contains("d"))
                    {
                        string[] diceTokens = currentToken[1..].Split("d");
                        if (diceTokens.Length != 2)
                        {
                            throw new DiceParserException("A dice token must be two numbers separated by a 'd'");
                        }
                        int count = int.Parse(diceTokens[0]);
                        int faces = int.Parse(diceTokens[1]);
                        for (int i = 0; i < count; i++)
                        {
                            int result = operation * (RandomNumberGenerator.GetInt32(faces) + 1);
                            results.Add(new DiceResultDtoV1()
                            {
                                Token = $"1d{faces}",
                                Result = result,
                                Type = DiceResultType.Random
                            });
                        }
                    }
                    else
                    {
                        int result = operation * int.Parse(currentToken[1..]);
                        results.Add(new DiceResultDtoV1()
                        {
                            Token = currentToken,
                            Result = result,
                            Type = DiceResultType.Constant
                        });
                    }
                }
                catch(Exception ex)
                {
                    errors.Add(new DiceErrorDtoV1()
                    {
                        Token = currentToken,
                        Error = ex.Message
                    });
                }
            }
            return (results.ToArray(), errors.ToArray());
        }
    }
}
