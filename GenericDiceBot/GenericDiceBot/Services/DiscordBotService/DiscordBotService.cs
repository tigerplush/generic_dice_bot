
using Discord;
using Discord.Net;
using Discord.WebSocket;
using GenericDiceBot.Dtos.V1;
using GenericDiceBot.Utilities;
using Newtonsoft.Json;

namespace GenericDiceBot.Services.DiscordBotService
{
    public class DiscordBotService : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private string _token;
        private ulong? _diceChannelId = null;
        private Dictionary<string, Func<SocketSlashCommand, Task>> _commandMap = new Dictionary<string, Func<SocketSlashCommand, Task>>();

        public DiscordBotService(IConfiguration configuration)
        {
            string? token = configuration["DiscordToken"];
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }
            _token = token;
            _client = new DiscordSocketClient();
            _client.Log += message =>
{
                Console.WriteLine(message);
                return Task.CompletedTask;
            };
            _client.Ready += ClientReady;
            _client.SlashCommandExecuted += SlashCommandExecuted;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.LoginAsync(Discord.TokenType.Bot, _token);
            await _client.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task ClientReady()
        {
            SlashCommandBuilder hereCommand = new SlashCommandBuilder()
                .WithName("here")
                .WithDescription("Sets the dice throw channel");

            SlashCommandBuilder throwCommand = new SlashCommandBuilder()
                .WithName("throw")
                .WithDescription("Throws the requested dice and returns the result")
                .AddOption("dice", ApplicationCommandOptionType.String, "a dice string you want to throw", isRequired: true);

            try
            {
                await _client.CreateGlobalApplicationCommandAsync(hereCommand.Build());
                await _client.CreateGlobalApplicationCommandAsync(throwCommand.Build());
                _commandMap.Add(hereCommand.Name, HereCommand);
                _commandMap.Add(throwCommand.Name, ThrowCommand);
            }
            catch(HttpException ex)
            {
                string message = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                Console.WriteLine(message);
            }
        }

        private async Task SlashCommandExecuted(SocketSlashCommand command)
        {
            if(_commandMap.ContainsKey(command.CommandName))
            {
                await _commandMap[command.CommandName].Invoke(command);
            }
            else
            {
                await command.RespondAsync("I don't know this command");
            }
        }

        private async Task HereCommand(SocketSlashCommand command)
        {
            _diceChannelId = command.ChannelId;
            await command.RespondAsync($"Dice throw channel is now <#{_diceChannelId}>");
        }

        private async Task ThrowCommand(SocketSlashCommand command)
        {
            string request = (string)command.Data.Options.First().Value;
            DiceResultDtoV1[] results = DiceThrow.Parse(request);
            int sum = results.Sum(r => r.Result);
            await command.RespondAsync(embed: CreateDiceThrowEmbed(request, results, sum));
        }

        public async Task PostToDiceChannel(string request, DiceResultDtoV1[] results, int sum, string? reason = null, string? requester = null, string? imageUrl = null)
        {
            if(_diceChannelId == null)
            {
                Console.WriteLine("There is no dice channel set");
                return;
            }
            IChannel channel = await _client.GetChannelAsync((ulong)_diceChannelId);
            if(channel == null)
            {
                Console.WriteLine($"There is no channel found for set id {_diceChannelId}");
                return;
            }
            IMessageChannel? messageChannel = channel as IMessageChannel;
            if(messageChannel == null)
            {
                Console.WriteLine($"Channel is not a message channel");
                return;
            }

            await messageChannel.SendMessageAsync(embed: CreateDiceThrowEmbed(request, results, sum, reason, requester, imageUrl));
        }

        private Embed CreateDiceThrowEmbed(string request, DiceResultDtoV1[] results, int sum, string? reason = null, string? requester = null, string? imageUrl = null)
        {
            EmbedFieldBuilder requestBuilder = new EmbedFieldBuilder()
                .WithName("Request")
                .WithValue(request);

            string resultValue = string.Empty;
            foreach (DiceResultDtoV1 result in results.Where(r => r.Type == Utilities.DiceResultType.Random))
            {
                resultValue += $"{result.Result}, ";
            }

            EmbedFieldBuilder resultsBuilder = new EmbedFieldBuilder()
                .WithName("Results")
                .WithValue(resultValue.Substring(0, resultValue.Length - 2));

            EmbedFieldBuilder sumBuilder = new EmbedFieldBuilder()
                .WithName("Result")
                .WithValue(sum);

            List<EmbedFieldBuilder> fields = [requestBuilder, resultsBuilder, sumBuilder];
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle(reason ?? "Unknown Reason")
                .WithFields(fields)
                .WithFooter(footer => footer.Text = "Pipsi Dice - A product of Pipsi Duke™")
                .WithCurrentTimestamp();

            EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder()
                .WithName(requester ?? "Unknown Requester");
            if (!string.IsNullOrEmpty(imageUrl))
            {
                authorBuilder.WithIconUrl(imageUrl);
            }
            embedBuilder.WithAuthor(authorBuilder);

            return embedBuilder.Build();
        }
    }
}
