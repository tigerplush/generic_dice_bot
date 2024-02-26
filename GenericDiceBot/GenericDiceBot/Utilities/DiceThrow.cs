using GenericDiceBot.Dtos.V1;
using System.Security.Cryptography;

namespace GenericDiceBot.Utilities
{
    public static class DiceThrow
    {
        public static DiceResultDtoV1[] Parse(string input)
        {
            List<DiceResultDtoV1> results = new List<DiceResultDtoV1>();
            input = input.Replace("+", ":+")
                .Replace("-", ":-");
            string[] tokens = input.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string token in tokens)
            {
                string currentToken = token;
                int operation = 1;
                if(currentToken.StartsWith("+"))
                {
                    operation = 1;
                }
                else if(currentToken.StartsWith("-"))
                {
                    operation = -1;
                }
                else
                {
                    currentToken = $"+{token}";
                }
                if (currentToken.Contains("d"))
                {
                    string[] diceTokens = currentToken.Substring(1).Split("d");
                    if(diceTokens.Length != 2)
                    {
                        throw new DiceParserException();
                    }
                    int count = int.Parse(diceTokens[0]);
                    int faces = int.Parse(diceTokens[1]);
                    for(int i = 0; i < count; i++)
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
                    int result = operation * int.Parse(currentToken.Substring(1));
                    results.Add(new DiceResultDtoV1()
                    {
                        Token = currentToken,
                        Result = result,
                        Type = DiceResultType.Constant
                    });
                }
            }
            return results.ToArray();
        }
    }
}
