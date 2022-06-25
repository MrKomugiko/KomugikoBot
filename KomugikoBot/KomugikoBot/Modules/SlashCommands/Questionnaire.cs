using Discord;
using Discord.Commands;
using Discord.Interactions;
using KomugikoBot.Enums;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SummaryAttribute = Discord.Interactions.SummaryAttribute;
using Discord.WebSocket;
using static KomugikoBot.Komugiko;

namespace KomugikoBot.Modules.SlashCommands
{
    public class Questionnaire : InteractionModuleBase<SocketInteractionContext>
    {
     

        private readonly DiscordSocketClient _client;
        public Questionnaire(DiscordSocketClient client)
        {
            _client = client;
            Console.WriteLine("add listener to 'ReactionAdd' NORMALIZED");

            _client.ReactionAdded += RefreshChart;
            _client.ReactionRemoved += RefreshChart;
        }

        public enum EselectionMode
        {
            Single,
            Multiple
        }

     

        [SlashCommand("questionnaire", "embed test")]
        public async Task HandleQuestionnaireCommend(
            string title,
            [Summary(description: "(:one: / first option) (:two: / second option) ... ")] string fieldsInput,
            [Summary(description: "Description under form chart ")] string description,
            [Summary(description: "Select color for left border of embeded form")] DiscordColor BorderColor,
            [Summary(description: "Vote progres bars render mode (default:false)\ntest ")] bool normalized = false,
            [Summary(description: "Set time before end voting in minutes")] int timeout = 0
            )
        {
            Dictionary<IEmote, string> voteOptions = new Dictionary<IEmote, string>();

            Regex regex = new Regex(@"\([^)]+\)");
            MatchCollection matchedFields = regex.Matches(fieldsInput);
                
            if(matchedFields.Count == 0)
            {
                await WrongArgumentErrorHandler(title, fieldsInput, description, BorderColor, normalized, timeout, new Exception("Not found fields in correct format (emoji / text)"));
                return; 
            }

            for (int i = 0; i < matchedFields.Count; i++)
            {
                string fieldelement = matchedFields[i].Value.Replace("(", "").Replace(")", "");
                string[] elements = fieldelement.Trim().Split("/");
                if(elements.Length == 1)
                {
                    await WrongArgumentErrorHandler(title, fieldsInput, description, BorderColor, normalized, timeout, new Exception("missinng emoji or title"));
                    return;
                }

                Emote emote = new(elements[0]);

                Console.WriteLine("emoji length "+elements[0]);
                voteOptions.Add(emote, elements[1]+"\n");
            }

            var embed = new EmbedBuilder
            {
                Title = title,
                Color = (uint)BorderColor,
                Timestamp = DateTimeOffset.UtcNow,
            };
            
            foreach(var vote in voteOptions)
            {
                embed.AddField(vote.Key.Name.Trim() + " - " + vote.Value, "`█                                   |0%` (0)");
            }

            embed.WithDescription(description);

            await RespondAsync(embed: embed.Build());
            IMessage? recentmesage = await Context.Channel.GetMessagesAsync(1, CacheMode.AllowDownload).Flatten().FirstOrDefaultAsync();

            List<IEmote> emotes = voteOptions.Keys.ToList();
            emotes.ForEach(async x =>
            {
                try
                {
                    await recentmesage.AddReactionAsync(x);
                }
                catch (Exception ex)
                {
                    await EmojiErrorHandler(title, fieldsInput, description, BorderColor, normalized, timeout, recentmesage, ex);
                    return;
                }
            });

            if (normalized)
            {
                Komugiko.deployedforms.Add(key: recentmesage.Id, value: new FormSettingsInfo(isNormalized: true));
            }
            else
            {
                Komugiko.deployedforms.Add(key: recentmesage.Id, value: new FormSettingsInfo(isNormalized: false));
            }


            if(timeout != 0)
            {
                Console.WriteLine($"message with id: {recentmesage.Id} setter to delete");
                await DeleteFromAndPrintResults(voteOptions, recentmesage.Id, time: timeout*60).ConfigureAwait(false);
            }
        }

        private async Task RefreshChart(Cacheable<IUserMessage, ulong> msgCache, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            FormSettingsInfo? formmessage = Komugiko.deployedforms.Where(x => x.Key == msgCache.Id).SingleOrDefault().Value;
            if (formmessage == null) return;

            IMessage message = await channel.Value.GetMessageAsync(msgCache.Id);
            if(message == null)
            {
                Console.WriteLine("RefreshChart embeded in message id: NULL");
            }
            Console.WriteLine("RefreshChart embeded in message id: "+ message?.Id);
            
            channel.Value.ModifyMessageAsync(message.Id, x => x.Embed = RefreshChartEmbed(message, reaction, normalized:formmessage.Value.IsNormalized));
        }
        private async Task DeleteRespondMessage(IMessage recentmesage, int time)
        {
            if(recentmesage != null)
            {
                await Task.Delay(TimeSpan.FromSeconds(time));
                await Context.Channel.DeleteMessageAsync(recentmesage.Id);
                Console.WriteLine("removed:"+recentmesage.Id);
            }
        }
        private async Task DeleteFromAndPrintResults(Dictionary<IEmote, string> voteOptions, ulong messageId, int time)
        {
            Console.WriteLine("Timer to remove form, started");

            await Task.Delay(TimeSpan.FromSeconds(time));
            var formvotingresult = $"Ankieta zakończona.\n";
            foreach(var vote in voteOptions)
            {
                formvotingresult += vote.Value;
            }

            await Context.Channel.SendMessageAsync(formvotingresult);

            Console.WriteLine("delete form: "+ messageId);
            await Context.Channel.DeleteMessageAsync(messageId);

            Komugiko.deployedforms.Remove(messageId);
            _client.ReactionAdded -= RefreshChart;
            _client.ReactionRemoved -= RefreshChart;
            Console.WriteLine("removed:" + messageId);
        }
        
       
        
        // helper methods
        private Discord.Embed RefreshChartEmbed(IMessage message, SocketReaction reaction, bool normalized = false)
        {
            var oldEmbed = message.Embeds.First();
            var totalVotes = message.Reactions.Sum(x => x.Value.ReactionCount);
            List<EmbedFieldBuilder> newFields = oldEmbed.Fields
                .Select(x=>new EmbedFieldBuilder() {
                    Name = x.Name, 
                    Value = GenerateProgresBar(totalVotes,x.Name.Split("-")[0].Trim(), message, normalized)
                })
                .ToList();

            var updatedEmbed = new EmbedBuilder
            {
                Fields = newFields,
                Title = oldEmbed.Title,
                Description = oldEmbed.Description,
                Color = (uint)oldEmbed.Color,
                Timestamp = oldEmbed.Timestamp,
            };

            return updatedEmbed.Build();
        }
        private string GenerateProgresBar(int totalReactions, string addedReaction, IMessage message, bool normalized ,int width = 35)
        {
            string fillFull = "█", fillEmpty = " ";

            int sumthisemote = message.Reactions.FirstOrDefault(x => x.Key.Name == addedReaction).Value.ReactionCount;
            if (sumthisemote == 0) return "~ Loading ~";

            int percentage = (int)Math.Truncate(sumthisemote *100.00/ totalReactions );
            int fullCount = (int)Math.Truncate(width * ((double)percentage / 100.00));
            
            if (normalized)
            {
                var maxValueOnChart = (int)Math.Truncate(message.Reactions.Max(x => x.Value.ReactionCount) * 100.00 / totalReactions);
                var normalizationvalue = 100.00 / maxValueOnChart;
                if (maxValueOnChart == 0) normalizationvalue = 1;

                fullCount = (int)Math.Truncate(width * ((percentage * normalizationvalue) / 100.00));
               //Console.WriteLine($"{percentage}%, max to:{maxValueOnChart}% => to normalize,multiply by {normalizationvalue} nowa wartosc procentowa to {fullCount}");
            }

            var emptyCount = width - fullCount;

            var outputBar = fillFull;
            for (int index = 0; index < fullCount; index++)
            { outputBar += fillFull; }
            for (int index = 0; index < emptyCount; index++)
            { outputBar += fillEmpty; }

            string result = $"`{outputBar}|{percentage}%` ({sumthisemote})";
            return result;
        }

        // Error handlers
        private async Task EmojiErrorHandler(string title, string fieldsInput, string description, DiscordColor BorderColor, bool normalized, int timeout, IMessage? recentmesage, Exception ex)
        {
            await Context.Channel.DeleteMessageAsync(recentmesage.Id);

            string commandName = nameof(Questionnaire).ToLower();
            string normalized_ARG = normalized == false ? String.Empty : ($"normalized:{normalized}");
            string timeout_ARG = timeout == 0 ? String.Empty : ($"timeout:{timeout}");

            string errormessage = $"Nie udalo sie stworzyc ankiety, prawdopodobna przyczyna \n" +
                $" - błąd składni \n" +
                $" - serverowa custom emoji \n" +
                $"ostatnio wprowadzona komenda: \n" +
                $"```\n" +
                $"/{nameof(Questionnaire).ToLower()} title:{title} fields-input:{fieldsInput} description:{description} color:{BorderColor} {normalized_ARG} {timeout_ARG}" +
                $"\n" +
                $"\n" +
                $"{ex.Message}\n" +
                $"```\n";


            await ReplyAsync(errormessage);
        }
        private async Task WrongArgumentErrorHandler(string title, string fieldsInput, string description, DiscordColor BorderColor, bool normalized, int timeout, Exception ex)
        {
            string commandName = nameof(Questionnaire).ToLower();
            string normalized_ARG = normalized == false ? String.Empty : ($"normalized:{normalized}");
            string timeout_ARG = timeout == 0 ? String.Empty : ($"timeout:{timeout}");

            string errormessage = $"Nie udalo sie stworzyc ankiety, prawdopodobna przyczyna \n" +
                $" - niepoprawny argument \n" +
                $" - niewlasciwa składnia par emoji / text:pytanie \n" +
                $" - niewlaściwy znak w ciągu komendy \n" +
                $"ostatnio wprowadzona komenda: \n" +
                $"```\n" +
                $"/{nameof(Questionnaire).ToLower()} title:{title} fields-input:{fieldsInput} description:{description} color:{BorderColor} {normalized_ARG} {timeout_ARG}" +
                $"\n" +
                $"\n" +
                $"{ex.Message}\n" +
                $"```\n";

            await RespondAsync(errormessage, ephemeral:false);
        }

    }
}

