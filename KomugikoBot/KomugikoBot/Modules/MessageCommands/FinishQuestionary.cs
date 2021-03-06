using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomugikoBot.Modules.MessageCommands
{
    public class FinishQuestionary : InteractionModuleBase<SocketInteractionContext>
    {
        // Pins a message in the channel it is in.
        [MessageCommand("finish-questionary")]
        public async Task HandlePinCommand(IMessage message)
        {
            // make a safety cast to check if the message is ISystem- or IUserMessage
            if (message is not IUserMessage userMessage)
            {
                await RespondAsync(text: ":x: Its not a valid target.");
            }

            // if the pins in this channel are equal to or above 50, no more messages can be pinned.
            else if ((await Context.Channel.GetPinnedMessagesAsync()).Count >= 50)
                await RespondAsync(text: ":x: You cant pin any more messages, the max has already been reached in this channel!");

            else
            {
                await userMessage.PinAsync();
                await RespondAsync(":white_check_mark: Successfully pinned message!");
            }
        }

    }
}
