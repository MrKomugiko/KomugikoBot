using Discord.Interactions;
using Discord;


namespace KomugikoBot.Modules.InteractionCommands
{
    public class Uptime : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("uptime", "Current bot uptime.")]
        public async Task HandleUptimeCommand()
        {
            string start = Komugiko.Instance.StartDateUTC.ToString().PadRight(25);
            TimeSpan time = Komugiko.Instance.CurrentUptimeUTC;
            string uptimeString = $"{(time.Days == 0 ? string.Empty : (time.Days + 'd'+' '))}{time.Hours.ToString("00")}:{time.Minutes.ToString("00")}:{time.Seconds.ToString("00")}".PadRight(25);

            string result = 
                $"`╔════════╤═══════════════════════════╗`\n" +
                $"`║ Start  │ { start                 } ║`\n" +
                $"`╟────────┼───────────────────────────╢`\n" +
                $"`║ UpTime │ { uptimeString          } ║`\n" +
                $"`╚════════╧═══════════════════════════╝`\n";

            await RespondAsync(result);
        }
    }
}

//     ╟  ╤ ╔╗ ═ ─
//     ╢  ╧ ╚╝ ║ │ 