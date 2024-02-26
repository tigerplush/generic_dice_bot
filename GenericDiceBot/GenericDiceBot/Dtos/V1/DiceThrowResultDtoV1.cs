namespace GenericDiceBot.Dtos.V1
{
    public class DiceThrowResultDtoV1
    {
        public string DiceThrow { get; set; } = string.Empty;
        public int Result { get; set; }
        public DiceResultDtoV1[] Results { get; set; } = [];
        public string? Reason { get; set; } = null;
    }
}
