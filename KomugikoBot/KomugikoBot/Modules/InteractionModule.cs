using Discord.Interactions;
using Discord;


namespace KomugikoBot.Modules
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping","Recieve a ping message!")]
        public async Task HandlePingCommand()
        {
            await RespondAsync("Pong!");
        }
    }
}
