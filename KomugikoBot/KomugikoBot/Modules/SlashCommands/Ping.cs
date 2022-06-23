using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomugikoBot.Modules.SlashCommands
{
    public class Ping : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Pings the bot and returns its latency.")]
        public async Task HandlePingCommand()
           => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);
    }
}
