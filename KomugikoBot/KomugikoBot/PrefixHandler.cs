using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace KomugikoBot
{
    public class PrefixHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;

        public PrefixHandler(DiscordSocketClient client, CommandService commands, IConfigurationRoot config)
        {
            _client= client;
            _commands = commands;
            _config = config;
        }

        public async Task IntitializeAsync()
        {
            _client.MessageReceived += HsndleCommandAsync;
        }

        public void AddModule<T>()
        {
            _commands.AddModuleAsync<T>(null);
        }

        private async Task HsndleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
           
            if (message == null) return;

            int argPos = 0;
          
            if (!(message.HasCharPrefix(_config["prefix"][0], ref argPos) || 
                 !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                  message.Author.IsBot ) 
                return;

            var context = new SocketCommandContext(_client, message);
          
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }
    }
}
