namespace GenericDiceBot.Dtos.V1
{
    public class DiceRequestDtoV1
    {
        public string DiceThrow { get; set; } = string.Empty;
        public string? Requester { get; set; } = null;
        public string? Reason { get; set; } = null;
        public string? ImageUrl { get; set; } = null;
    }
}
