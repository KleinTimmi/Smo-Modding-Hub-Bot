using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace SMO_Modding_Hub_Bot.Commands
{
    public class Mods : ApplicationCommandModule
    {
        // Dictionary mit Mod-Namen, Beschreibung, Link und Bild
        public static readonly Dictionary<string, (string Description, string Url, string ImageUrl)> ModLinks = new()
        {
            { "craftyboss", ("Crafty Boss Discord Server", "https://discord.gg/PwbkgprXN6", "https://cdn.discordapp.com/icons/481991664085499924/4dd2e01a7a9c3be808882c828a4d5e6d.webp?size=1024") },
            { "galaxystory", ("Galaxy Story Discord Server", "https://discord.gg/BNSM9wt8zu", "https://cdn.discordapp.com/icons/1218139283215482960/f3db6565f303c19d3cb752fd7d72740d.webp?size=1024") },
            { "amethyst", ("Amethyst's Discord Server", "https://discord.gg/wZ6aZecTBc", "https://cdn.discordapp.com/icons/1066047249878089799/66216b10e00522e400ef6be4f0efad20.webp?size=1024") },
            { "kingdomexpansion-discord", ("Kingdom Expansion Discord", "https://discord.gg/uUrHE23u5a", "https://cdn.discordapp.com/icons/1342135514832044112/ca9b3ad26e1986cb14a6d119546a1ea3.webp?size=1024") },
            { "kingdomexpansion-modpage", ("Kingdom Expansion Modpage", "https://gamebanana.com/mods/568934", "https://media.discordapp.net/attachments/1337052385054294051/1447339321806950490/CmTitleLogow.png?ex=69374348&is=6935f1c8&hm=f3746d2aaf6b556fcd6c2b99f6d362a7782c1118bde844dbf4620bde75872f31&=&format=webp&quality=lossless&width=950&height=588") },
            { "smoo", ("Super Mario Odyssey Online Extensions", "https://github.com/DaDev123/Super-Mario-Odyssey-EXTENSIONS/releases", "https://cdn.discordapp.com/icons/1337052385054294048/14ab70f67345ad927fe9998734f1d1dc.webp?size=1024") }
        };

        [SlashCommand("modlink", "Get a link to a mod resource")]
        public async Task ModLinkCommand(
            InteractionContext ctx,
            [Option("mod", "Choose a mod name")]
            [Autocomplete(typeof(ModAutocompleteProvider))] string mod)
        {
            if (ModLinks.TryGetValue(mod.ToLower(), out var info))
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle(info.Description)
                    .WithUrl(info.Url)
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

    // Autocomplete Provider
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
