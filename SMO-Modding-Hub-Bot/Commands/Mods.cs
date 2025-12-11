using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SMO_Modding_Hub_Bot.Config;

namespace SMO_Modding_Hub_Bot.Commands
{
    public class Mods : ApplicationCommandModule
    {
        // ModLinks aus JSON laden
        public static Dictionary<string, ModInfo> ModLinks => Program.Config?.Mods ?? [];

        [SlashCommand("modlink", "Get a link to a mod resource")]
        public static async Task ModLinkCommand(
            InteractionContext ctx,
            [Option("mod", "Choose a mod name")]
            [Autocomplete(typeof(ModAutocompleteProvider))] string mod)
        {
            if (ModLinks.TryGetValue(mod.ToLower(), out var info))
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle(info.Description)
                    .WithUrl(info.Url)
                    .WithFooter(info.Maker)
                    .WithImageUrl(info.ImageUrl);

                await ctx.CreateResponseAsync(
                    new DiscordInteractionResponseBuilder().AddEmbed(embed)
                );
            }
            else
            {
                await ctx.CreateResponseAsync($"❌ Unknown mod: {mod}");
            }
        }
    }

    public class ModAutocompleteProvider : IAutocompleteProvider
    {
        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            string userInput = ctx.OptionValue?.ToString() ?? "";

            var matches = Mods.ModLinks.Keys
                .Where(k => k.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(25)
                .Select(k => new DiscordAutoCompleteChoice(k, k));

            return await Task.FromResult(matches);
        }
    }
}
