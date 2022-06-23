using Discord.Interactions;
using KomugikoBot.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomugikoBot.Modules.SlashCommands
{
    public class ChoiceExample : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("choice_example", "Enums create choices")]
        public async Task HAndleChoiceExampleCommand(ExampleEnum input) => await RespondAsync(input.ToString());
    }
}
