using GenericDiceBot.Services.DiscordBotService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<DiscordBotService>();
#pragma warning disable 8603
builder.Services.AddSingleton<IHostedService, DiscordBotService>(serviceProvider => serviceProvider.GetService<DiscordBotService>());
#pragma warning restore 8603

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
