using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SMO_Modding_Hub_Bot;
using System;
using System.Collections;
using System.Diagnostics;

public class Util
{
    public class Ansi
    {
        //I know I could've just used the the ANSI from DSharp, but whatever
        //or do it manuelly but this is waaaay more readable

        
        /// <summary>
        /// \x1B[0m resets all attributes (color, underline, etc.)
        /// </summary>
        public const string Reset = "\x1B[0m";

        /// <summary>
        /// \x1B[31m sets the text color to red
        /// </summary>
        public const string Red = "\x1B[31m";
            public const string RedUnderline = "\x1B[31m;4m";
        /// <summary>
        /// \x1B[32m sets the text color to green
        /// </summary>
        public const string Green = "\x1B[32m";
            public const string GreenUnderline = "\x1B[32;4m";
        /// <summary>
        /// \x1B[33m sets the text color to yellow
        /// </summary>
        public const string Yellow = "\x1B[33m";
            public const string YellowUnderline = "\x1B[33m;4m";
        /// <summary>
        /// \x1B[34m sets the text color to Blue
        /// </summary>
        public const string Blue = "\x1b[34m";
            public const string BlueUnderline = "\x1B[34;4m";
        /// <summary>
        ///  \x1B[36m sets the text color to cyan
        /// </summary>
        public const string Cyan = "\x1B[36m";
            public const string CyanUnderline = "\x1B[36m;4m";
    }

    public static class EmbedHelper
    {
        #region SendEmbedAsync Utility Method
        /// <summary>
        /// Builds a Discord embed with a title, Description, and image URL,
        /// then sends it as a response to an interaction.
        /// Additionally, logs the embed details with an ANSI-formatted URL for console output.
        /// </summary>
        /// <param name="ctx">
        /// The current <see cref="InteractionContext"/> used to send the response.
        /// </param>
        /// <param name="title">
        /// The title of the embed, typically the name of the item or stage.
        /// </param>
        /// <param name="description">
        /// The description text displayed inside the embed.
        /// </param>
        /// <param name="imageUrl">
        /// The URL of the image to be shown in the embed.
        /// </param>
        /// <param name="visibleForAll">
        /// If <c>true</c>, the response is visible to everyone in the channel.
        /// If <c>false</c>, the response is ephemeral and only visible to the user.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation of sending the embed.
        /// </returns>
        public static async Task SendEmbedAsync(
            InteractionContext ctx,
            string title,
            string description,
            string imageUrl,
            bool visibleForAll)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithImageUrl(imageUrl);

            await ctx.CreateResponseAsync(
                new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .AsEphemeral(!visibleForAll)
            );
            Program.ExtraLogs.Enqueue(
                $"{title} ({description} {Util.Ansi.BlueUnderline}{imageUrl}{Util.Ansi.Reset})"
            );
        }
        #endregion
    }


}


