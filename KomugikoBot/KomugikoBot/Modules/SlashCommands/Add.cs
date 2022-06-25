using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomugikoBot.Modules.SlashCommands
{
    public class Add : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("add", "add")]
        public async Task HandleAddCommend(double number1, double number2)
        {
            double result = number1 + number2;
            await RespondAsync(result.ToString("0.00"), ephemeral: true);
            var msg = await ReplyAsync(result.ToString("0.00"));

            await Task.Delay(TimeSpan.FromSeconds(10));
            await msg.DeleteAsync();
        }
    }
}
