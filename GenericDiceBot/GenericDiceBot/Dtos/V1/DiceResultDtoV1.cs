using GenericDiceBot.Utilities;
using System.Text.Json.Serialization;

namespace GenericDiceBot.Dtos.V1
{
    public class DiceResultDtoV1
    {
        public string Token { get; set; } = string.Empty;
        public int Result { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DiceResultType Type { get; set; }
    }
}
