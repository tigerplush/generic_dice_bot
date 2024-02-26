using GenericDiceBot.Dtos.V1;
using GenericDiceBot.Services.DiscordBotService;
using GenericDiceBot.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace GenericDiceBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiceController : ControllerBase
    {
        private readonly DiscordBotService _discordBotService;
        public DiceController(DiscordBotService discordBotService)
        {
            _discordBotService = discordBotService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(DiceThrowResultDtoV1), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DiceThrowErrorDtoV1), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DiceThrowResultDtoV1>> PostDiceAsync(DiceRequestDtoV1 diceRequest)
        {
            if(string.IsNullOrEmpty(diceRequest.DiceThrow))
            {
                return BadRequest(new DiceThrowErrorDtoV1());
            }
            try
            {
                (DiceResultDtoV1[] results, DiceErrorDtoV1[] errors) = DiceThrow.Parse(diceRequest.DiceThrow);
                if(errors.Length > 0)
                {
                    return BadRequest(new DiceThrowErrorDtoV1()
                    {
                        Errors = errors
                    });
                }
                int sum = 0;
                foreach(DiceResultDtoV1 result in results)
                {
                    sum += result.Result;
                }
                await _discordBotService.PostToDiceChannel(diceRequest.DiceThrow,results, sum, diceRequest.Reason, diceRequest.Requester, diceRequest.ImageUrl);
                return Ok(new DiceThrowResultDtoV1()
                {
                    DiceThrow = diceRequest.DiceThrow,
                    Reason = diceRequest.Reason,
                    Results = results,
                    Result = sum
                });
            }
            catch
            {
                return BadRequest(new DiceThrowErrorDtoV1());
            }
        }
    }
}
