using FindTheThing.DiscordBot.Commands;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Hosting.Extensions;
using Remora.Rest.Core;
using YoutubeExplode;

// Setup
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services
            .AddDiscordCommands(true)
            .AddCommandGroup<UserCommands>()
            .AddTransient<YoutubeClient>()
            ;
    })
    .AddDiscordService(serviceProvider => GetDiscordToken(serviceProvider.GetRequiredService<IConfiguration>()))
    .Build();

var config = host.Services.GetRequiredService<IConfiguration>();

_ = await host.Services.GetRequiredService<SlashService>()
    .UpdateSlashCommandsAsync(new Snowflake(config.GetValue<ulong>("SERVER_SNOWFLAKE"
                ?? throw new InvalidOperationException("No targeted server found in config under SERVER_SNOWFLAKE"))));

// Run the thing
await host.RunAsync();

static string GetDiscordToken(IConfiguration configuration) =>
    configuration.GetValue<string>("BOT_TOKEN")
        ?? throw new InvalidOperationException("No token found in config under BOT_TOKEN");