using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SMO_Modding_Hub_Bot.Commands
{
    public class Resource : BaseCommandModule
    {
        [Command("ping")]
        public async Task PingCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong!");
        }

        [Command("MoonFlow")]
        public async Task MoonflowCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("https://discord.com/channels/774687602996936747/774687812364664836/1404839117778456656");
        }

        [Command("Toolbox")]
        public async Task ToolboxCommand(CommandContext ctx, string string1 = "zip")
        {
            if (string1 == "github")
            {
                await ctx.Channel.SendMessageAsync(" https://github.com/KillzXGaming/Switch-Toolbox");
            }
            else if (string1 == "zip")
            {
                await ctx.Channel.SendMessageAsync(" https://github.com/KillzXGaming/Switch-Toolbox/releases/download/Final/Toolbox-Latest.zip ");
            }else
            {
                await ctx.Channel.SendMessageAsync(" https://github.com/KillzXGaming/Switch-Toolbox/releases/download/Final/Toolbox-Latest.zip ");
            }
        }

        [Command("resources")]
        public async Task resourcesCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("https://smo.monsterdruide.one/resources");
        }

        [Command("res")]
        public async Task resCommand(CommandContext ctx)
        {
            await resourcesCommand(ctx);
        }

        [Command("CamArea")]
        public async Task CamAreaCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("https://discord.com/channels/774687602996936747/1354952152187015188");
        }
        /// <summary>
        /// Sends a message containing a Spotlight-related link based on the specified game and GitHub resource type.
        /// </summary>
        /// <remarks>If an unsupported game identifier is provided, a message indicating limited
        /// availability will be sent.</remarks>
        /// <param name="ctx">The command context, which provides information about the command invocation and allows interaction with the
        /// Discord channel.</param>
        /// <param name="SMO">The game identifier. Use "SMO" for Super Mario Odyssey or "3DW" for Super Mario 3D World. Defaults to "SMO".</param>
        /// <param name="Github">The type of GitHub resource to retrieve. Use "0" for the default link, "code" for the repository link, or
        /// "releases" for the releases page. Defaults to "0".</param>
        /// <returns>A task that represents the asynchronous operation of sending the message.</returns>
        [Command("Spotlight")]
        public async Task SpotlightCommand(CommandContext ctx, string SMO = "SMO", string Github = "0")
        {
            if (SMO == "SMO")
            {
                if (Github == "0")
                {
                    await ctx.Channel.SendMessageAsync("https://discord.com/channels/774687602996936747/1354952152187015188");
                }
                else if (Github == "code")
                {
                    await ctx.Channel.SendMessageAsync("https://github.com/Kirbymimi/Spotlight");
                }
                else if (Github == "releases")
                {
                    await ctx.Channel.SendMessageAsync("https://github.com/Kirbymimi/Spotlight/releases");
                }
            }
            else if (SMO == "3dw")
            {
                if (Github == "0")
                {
                    await ctx.Channel.SendMessageAsync("https://github.com/jupahe64/Spotlight");
                }
                else if (Github == "code")
                {
                    await ctx.Channel.SendMessageAsync("https://github.com/jupahe64/Spotlight");
                }
                else if (Github == "releases")
                {
                    await ctx.Channel.SendMessageAsync("https://github.com/jupahe64/Spotlight/releases");
                }
            }
            else
            {
                await ctx.Channel.SendMessageAsync("https://discord.com/channels/774687602996936747/1354952152187015188");
            }
            
        }

        [Command("Moonlight")]
        public async Task MoonlightCommand(CommandContext ctx, string Github = "0")
        {
            await SpotlightCommand(ctx, "SMO", Github);
        }

        /// <summary>
        /// Sends a message containing an invitation link to the Crafty boss Discord Server.
        /// </summary>
        /// <param name="ctx">The context of the command, including the channel where the message will be sent.</param>
        /// <returns>A task that represents the asynchronous operation of sending the message.</returns>
        [Command("Crafty")]
        public async Task CraftyCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("https://discord.gg/PwbkgprXN6");
        }
        /// <summary>
        /// Executes the "CraftyBoss" command, delegating to the appropriate handler.
        /// </summary>
        /// <param name="ctx">The context of the command, including the user input and execution environment.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Command("CraftyBoss")]
        public async Task CraftyBossCommand(CommandContext ctx)
        {
            await CraftyCommand(ctx);
        }
        /// <summary>
        /// Sends a message containing a link to the Galaxy Story Discord server.
        /// </summary>
        /// <param name="ctx">The context of the command, which provides information about the invocation and access to the channel where
        /// the command was executed.</param>
        /// <returns>A task that represents the asynchronous operation of sending the message.</returns>
        [Command("GalaxyStory")]
        public async Task GalaxyStoryCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("https://discord.gg/BNSM9wt8zu");
        }
        /// <summary>
        /// Executes the "ags" command, which triggers the Galaxy Story command.
        /// </summary>
        /// <param name="ctx">The context of the command, including information about the caller and the execution environment.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Command("ags")]
        public async Task agsCommand(CommandContext ctx)
        {
            await GalaxyStoryCommand(ctx);
        }
    }
}
