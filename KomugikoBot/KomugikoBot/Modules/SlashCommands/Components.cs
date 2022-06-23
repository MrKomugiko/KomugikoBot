using Discord;
using Discord.Interactions;

namespace KomugikoBot.Modules.SlashCommands
{
    public partial class Components : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("components", "Demonstrate button and select modal")]
        public async Task HandleComponentCommand()
        {
            var button = new ButtonBuilder()
            {
                CustomId = "button",
                Label = "Buton",
                Style = ButtonStyle.Primary
            };

            var menu = new SelectMenuBuilder()
            {
                CustomId = "menu",
                Placeholder = "Sample menu"
            };
            menu.AddOption("first Option", "first");
            menu.AddOption("second Option", "second");

            var component = new ComponentBuilder();
            component.WithButton(button);
            component.WithSelectMenu(menu);

            await RespondAsync("testing", components: component.Build());
        }

        [ComponentInteraction("button")]
        public async Task HandleButtonInput()
        {
            await RespondWithModalAsync<DemoModal>("demo_modal");
        }

        [ComponentInteraction("menu")]
        public async Task HandleManuSelection(string[] inputs)
        {
            await RespondAsync(inputs[0]);
        }

        [ModalInteraction("demo_modal")]
        public async Task HandleModalInput(DemoModal modal)
        {
            string input = modal.Greetings;
            await RespondAsync(input);
        }
        public class DemoModal : IModal
        {
            public string Title => "demo modal";

            [InputLabel("send a greetings")]
            [ModalTextInput("greeting_input", TextInputStyle.Short, placeholder: "Be nice..", maxLength: 100)]
            public string Greetings { get; set; }
        }
    }
}
