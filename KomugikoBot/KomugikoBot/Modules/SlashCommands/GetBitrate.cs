using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomugikoBot.Modules.SlashCommands
{
    public class GetBitrate : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("bitrate", "Gets the bitrate of a specific voice channel.")]
        public async Task HandleBitrateCommand([ChannelTypes(ChannelType.Voice, ChannelType.Stage)] IChannel channel)
              => await RespondAsync(text: $"This voice channel has a bitrate of {(channel as IVoiceChannel).Bitrate}");
    }
}
