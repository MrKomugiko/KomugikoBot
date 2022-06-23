using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomugikoBot.Modules.UserCommands
{
    public class Greet : InteractionModuleBase<SocketInteractionContext>
    {  
        // This command will greet target user in the channel this was executed in.
        [UserCommand("greet")]
        public async Task HandleGreetCommand(IUser user)
            => await RespondAsync(text: $":wave: {Context.User} said hi to you, <@{user.Id}>!");

    }
}
