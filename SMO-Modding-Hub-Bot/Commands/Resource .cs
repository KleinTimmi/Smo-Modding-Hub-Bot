using DSharpPlus.SlashCommands;

namespace SMO_Modding_Hub_Bot.Commands
{
    public class Resource : ApplicationCommandModule
    {
        #region Moonflow
        /// <summary>
        /// Sends a link to Moonflow
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [SlashCommand("moonflow", "Sends a link to Moonflow")]
        public async Task MoonflowCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://discord.com/channels/774687602996936747/774687812364664836/1404839117778456656");
        }
        #endregion

        #region toolbox
        /// <summary>
        /// Sends a link to Toolbox
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="github"></param>
        /// <returns></returns>
        [SlashCommand("toolbox", "Sends a link to Toolbox")]
        public async Task ToolboxCommand(
            InteractionContext ctx,
            [Option("github", "Send GitHub page instead of direct .zip")] bool github)
        {
            if (github)
                await ctx.CreateResponseAsync("https://github.com/KillzXGaming/Switch-Toolbox");
            else
                await ctx.CreateResponseAsync("https://github.com/KillzXGaming/Switch-Toolbox/releases/download/Final/Toolbox-Latest.zip");
        }
        #endregion

        #region resourcepage
        [SlashCommand("resources", "Sends a link to MonsterDruid1's Resource Page")]
        public async Task ResourcesCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://smo.monsterdruide.one/resources");
        }
        #endregion

        #region Camera Area
        [SlashCommand("CameraArea", "Sends a link to How to make camera areas")]
        public async Task CamAreaCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://discord.com/channels/774687602996936747/1354952152187015188");
        }
        #endregion

        #region libnws
        [SlashCommand("libnws", "Sends a link to How to libnws")]
        public async Task libnwsCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://codeberg.org/Cookieso/libnws");
        }
        #endregion

        #region Downgrade
        [SlashCommand("downgrade", "Sends a link to the smo Downgrade")]
        public async Task DowngradeCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://github.com/Istador/odyssey-downgrade");
        }
        #endregion

        #region Citric
        [SlashCommand("Citric", "Sends a link to Citric Composer")]
        public async Task CitricCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("https://gota7.github.io/Citric-Composer/");
        }
        #endregion

        #region KFR recorder
        [SlashCommand("KFRrec", "Sends a link to the Koopa Freerunning Recorder")]
        public async Task KFRrecCommand(
            InteractionContext ctx,
            [Option("github", "Send source instead of build")] bool github)
        {
            if (github)
            {
                await ctx.CreateResponseAsync("https://smo.monsterdruide.one/resources/kfr-recorder-source.zip");
            }
            else
            {
                await ctx.CreateResponseAsync("https://smo.monsterdruide.one/resources/kfr-recorder-build.zip");
            }
        }
        #endregion

        #region Spotlight
        [SlashCommand("spotlight", "Provides Spotlight links for SMO or 3DW")]
        public async Task SpotlightCommand(
            InteractionContext ctx,
            [Option("threeDW", "Send link for 3DW instead of SMO")] bool threeDW,
            [Option("github", "Send GitHub page instead of direct .zip")] bool github)
            {
                string url;

                if (!threeDW) // SMO
                {
                    url = github
                        ? "https://github.com/Kirbymimi/Spotlight"
                        : "https://github.com/Kirbymimi/Spotlight/releases/download/Whatever/Moonlight.zip";
                }
                else // 3DW
                {
                    url = github
                        ? "https://github.com/jupahe64/Spotlight"
                        : "https://github.com/jupahe64/Spotlight/releases/download/Auto/Spotlight.zip"; // ggf. direkter Release-Link
                }

                await ctx.CreateResponseAsync(url);
            }
        #endregion

        #region Moonlight
        [SlashCommand("moonlight", "Shortcut for Spotlight (SMO)")]
        public async Task MoonlightCommand(
            InteractionContext ctx,
            [Option("github", "Send GitHub page instead of direct .zip")] bool github)
            {
                // Einfach SpotlightCommand mit threeDW = false aufrufen
                await SpotlightCommand(ctx, false, github);
        }
        #endregion

    }
}
