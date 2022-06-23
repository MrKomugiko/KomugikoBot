using Discord;
using Discord.Commands;

namespace KomugikoBot.Modules.Commands
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task HandlePingCommand()
        {
            await Context.Message.ReplyAsync("Pong!");
        }
    }
}
