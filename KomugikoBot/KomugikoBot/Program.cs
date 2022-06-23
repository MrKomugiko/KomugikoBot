using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using KomugikoBot;
using KomugikoBot.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    // program entry point
    public static Task Main() => new Program().MainAsync();

    public async Task MainAsync()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("config.json")
            .Build();

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => services
                .AddSingleton(config)
                .AddSingleton(x => new DiscordSocketClient( new DiscordSocketConfig {
                        GatewayIntents = Discord.GatewayIntents.AllUnprivileged,
                        AlwaysDownloadUsers = true
                    }))
                .AddSingleton(x=>new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .AddSingleton(x=> new CommandService())
                .AddSingleton<PrefixHandler>()
                )
            .Build();

        await RunAsync(host);
    }

    public async Task RunAsync(IHost host)
    {
        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

        var _client = provider.GetRequiredService<DiscordSocketClient>();
        var sCommand = provider.GetRequiredService<InteractionService>();
        await provider.GetRequiredService<InteractionHandler>().IntitializeAsync();
        var config = provider.GetRequiredService<IConfigurationRoot>();
        var pCommand = provider.GetRequiredService<PrefixHandler>();
        pCommand.AddModule<PrefixModule>();
        await pCommand.IntitializeAsync();

        _client.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
        sCommand.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
       
        _client.Ready += async () => {
            Console.WriteLine("Bot ready!");
            await sCommand.RegisterCommandsToGuildAsync(UInt64.Parse(config["guild:samuraje"]));
        };
        
        await _client.LoginAsync(TokenType.Bot, config["token:samuraje"]);
        await _client.StartAsync();

        await Task.Delay(-1); // infinite loop, listening never ends
    }
}