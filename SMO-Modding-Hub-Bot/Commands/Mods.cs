using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace SMO_Modding_Hub_Bot.Commands
{

    /// <summary>
    /// This is for Links to mods like Galaxy Story, Kingdom expansion...
    /// </summary>
    public class Mods : ApplicationCommandModule
    {
        #region CraftyBoss
        /// <summary>
        /// Sends an invitation link to the Crafty Boss Discord server in the current channel.
        /// </summary>
        [SlashCommand("craftyboss", "Sends an invitation link to the Crafty Boss Discord Server")]
        public async Task CraftyBossCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://discord.gg/PwbkgprXN6");
        }
        #endregion

        #region GalaxyStory
        /// <summary>
        /// Sends a message containing a link to the Galaxy Story Discord server.
        /// </summary>
        [SlashCommand("galaxystory", "Sends an invitation link to the Galaxy Story Discord Server")]
        public async Task GalaxyStoryCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://discord.gg/BNSM9wt8zu");
        }
        #endregion

        #region KingdomExpansion
        /// <summary>
        /// sends invitation links for Kingdom Expansion with optional target selection
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [SlashCommand("kingdomexpansion", "Sends invitation links for Kingdom Expansion")]
        public async Task KingdomExpansionCommand(
            InteractionContext ctx,
            [Option("target", "Choose 'discord' or 'modpage' (optional)")] string target = null)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                // keine Auswahl → private Buttons
                var builder = new DiscordInteractionResponseBuilder()
                    .WithContent("Press 1 for Discord or 2 for the Modpage:")
                    .AsEphemeral(true) // privat
                    .AddComponents(
                new DiscordLinkButtonComponent("1: Discord", "https://discord.gg/uUrHE23u5a"),
                new DiscordLinkButtonComponent("2: Modpage", "https://gamebanana.com/mods/568934"));

                await ctx.CreateResponseAsync(builder);
            }
            else if (target.Equals("discord", StringComparison.OrdinalIgnoreCase))
            {
                // direkte Eingabe → öffentlich Discord-Link
                await ctx.CreateResponseAsync(
                    new DiscordInteractionResponseBuilder()
                        .WithContent("https://discord.gg/uUrHE23u5a")
                        .AsEphemeral(false) // öffentlich
                );
            }
            else if (target.Equals("modpage", StringComparison.OrdinalIgnoreCase))
            {
                // direkte Eingabe → öffentlich Modpage-Link
                await ctx.CreateResponseAsync(
                    new DiscordInteractionResponseBuilder()
                        .WithContent("https://gamebanana.com/mods/568934")
                        .AsEphemeral(false) // öffentlich
                );
            }
            else
            {
                await ctx.CreateResponseAsync(
                    new DiscordInteractionResponseBuilder()
                        .WithContent("Ungültige Option. Bitte 'discord' oder 'modpage' wählen.")
                        .AsEphemeral(true)
                );
            }
        }
        #endregion

        #region Amethyst
        [SlashCommand("Amethyst", "Sends an invitation link to Amethyst's Discord Server")]
        public async Task AmethystCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://discord.gg/wZ6aZecTBc");
        }
        #endregion

        #region SMOO
        [SlashCommand("SMOO", "Sends an link to most of the SMOO mods")]
        public async Task SMOOCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://github.com/DaDev123/Super-Mario-Odyssey-Online-EXTENSIONS/releases");
        }
        #endregion
    }

}
