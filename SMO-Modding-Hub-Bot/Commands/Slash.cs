using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Text.Json;
using static Util;

namespace SMO_Modding_Hub_Bot.Commands
{
    #region main class
    public class Slash : ApplicationCommandModule
    {
        #region Helpers
        /// <summary>
        /// this is used for random nummbers
        /// </summary>
        private static readonly Random random = new Random();
        /// <summary>
        /// used for getting the request file name
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static string GetRequestFileName(InteractionContext ctx)
        {
            if (ctx.Guild != null)
                return $"{ctx.Guild.Id}_requests.txt";
            else
                return "global_requests.txt"; // Fallback für DMs
        }



        /// <summary>
        /// used for getting the Mod request file name
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static string GetModRequestFileName(InteractionContext ctx)
        {
            if (ctx.Guild != null)
                return $"{ctx.Guild.Id}_ModRequests.txt";
            else
                return "DM_ModRequests.txt";
        }


        /// <summary>
        /// this is used for random responses
        /// </summary>
        private static readonly string[] responses =
        {
            ":thumbsup:",
            "https://cdn.discordapp.com/emojis/849804413027352576.webp?size=96",
            "https://cdn.discordapp.com/emojis/974893065288421416.webp?size=96",
            ":fire:",
            "https://cdn.discordapp.com/emojis/852812833385480232.webp?size=96",
            ":eyes:",
        };
        #endregion

            #region Ping
            [SlashCommand("ping", "Replies with Pong!")]
            public async Task PingCommand(InteractionContext ctx)
            {
                await ctx.CreateResponseAsync("Pong!");
            }
        #endregion

        #region Request Commands
            #region request
            [SlashCommand("request", "Request stuff")]
            public async Task RequestCommand(InteractionContext ctx, [Option("test", "balls")] string test)
            {
                if (string.IsNullOrWhiteSpace(test))
                {
                    await ctx.CreateResponseAsync(
                        DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent(":X: Request can't be nothing")
                            .AsEphemeral(true)
                    );
                    return;
                }

                // chose random index
                int index = random.Next(responses.Length);

                // sends the Answer
                await ctx.CreateResponseAsync(responses[index]);

                // Write to file
                await File.AppendAllTextAsync(GetRequestFileName(ctx),
                    $"{ctx.User.Username}#{ctx.User.Discriminator} requested: {test}{Environment.NewLine}");

                // Logging
                var memberName = ctx.Member?.ToString() ?? ctx.User.ToString();
                var guildName = ctx.Guild?.Name ?? "DirectMessage";

                Program.ExtraLogs.Enqueue($"{memberName} requested \"{test}\" in {guildName}");
            }
            #endregion

            #region requestCreator
            [SlashCommand("requestCreator", "Request the mod creator Role")]
            public async Task RequestCreatorCommand(InteractionContext ctx, [Option("Link", "Link to where you send a showcase")] string Link)
            {
                // chose random index
                int index = random.Next(responses.Length);

                // sends the Answer
                await ctx.CreateResponseAsync(
                    DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(responses[index])
                );

                // Write to the file (creates automatically if missing)
                await File.AppendAllTextAsync("requestModCreator.txt",
                    $"{ctx.User.Username}#{ctx.User.Discriminator} requested: {Link} in {ctx.Guild.Name} {Environment.NewLine}");

                // Logging
                Program.ExtraLogs.Enqueue($"{ctx.Member} requested {Util.Ansi.BlueUnderline} {Link} {Util.Ansi.Reset}");
            }
            #endregion

            #region requests
            [SlashCommand("requests", "ONLY ADMINS CAN USE THIS")]
            public async Task RequestsCommand(InteractionContext ctx, [Option("debug", "no")] string debug = "no")
            {
                var JsonReader = new config.JSONReader();
                await JsonReader.ReadJson();
                ulong[] ownerIds = JsonReader.admins.ToArray();

                if (ownerIds.Contains(ctx.User.Id) && (debug == "no" || string.IsNullOrWhiteSpace(debug)))
                {
                    var fileName = GetRequestFileName(ctx);

                    string content = File.Exists(fileName)
                        ? File.ReadAllText(fileName)
                        : "No requests.";

                    if (content.Length > 2000)
                    {
                        content = content.Substring(0, 1988) + "\n... (more)";
                    }

                    await ctx.CreateResponseAsync(
                        DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent(content)
                            .AsEphemeral(true)
                    );
                }
                else
                {
                    await ctx.CreateResponseAsync(
                        DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent("❌ You are not allowed to use this. :fish:")
                            .AsEphemeral(true)
                    );
                }
            }
            #endregion
        #endregion

        #region Random Commands
            #region randomcap
            [SlashCommand("randomcap", "Draw a random cap from https://smo.monsterdruide.one/resources/ItemCap.json")]
            public async Task RandomCapCommand(
                InteractionContext ctx,
                [Option("visible", "Should the result be visible for everyone?")] bool visibleForAll = false)
            {
                string url = "https://smo.monsterdruide.one/resources/ItemCap.json";

                using var http = new HttpClient();
                string json = await http.GetStringAsync(url);

                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                var caps = data.Keys.Where(k => !k.EndsWith("_Explain")).ToList();

                var random = new Random();
                string capKey = caps[random.Next(caps.Count)];

                string capName = data[capKey];
                string capExplain = data[$"{capKey}_Explain"];

                // Bild-URL dynamisch bauen
                string imageUrl = $"https://raw.githubusercontent.com/Amethyst-szs/smo-thumbnail-database/main/outfit/{capKey}Cap.png";

            await Util.EmbedHelper.SendEmbedAsync(ctx, capName, capExplain, imageUrl, visibleForAll);
        }
            #endregion

            #region randomoutfit
            [SlashCommand("randomoutfit", "Draw a random outfit from the online JSON")]
            public async Task RandomOutfitCommand(
                InteractionContext ctx,
                [Option("visible", "Should the result be visible for everyone?")] bool visibleForAll = false)
            {
                string url = "https://smo.monsterdruide.one/resources/ItemCloth.json";

                using var http = new HttpClient();
                string json = await http.GetStringAsync(url);

                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                var outfits = data.Keys.Where(k => !k.EndsWith("_Explain")).ToList();

                var random = new Random();
                string outfitKey = outfits[random.Next(outfits.Count)];

                string outfitName = data[outfitKey];
                string outfitExplain = data[$"{outfitKey}_Explain"];

                string imageUrl = $"https://raw.githubusercontent.com/Amethyst-szs/smo-thumbnail-database/main/outfit/{outfitKey}.png";

            await Util.EmbedHelper.SendEmbedAsync(ctx, outfitName, outfitExplain, imageUrl, visibleForAll);
        }
        #endregion

        #region randomstage
        [SlashCommand("randomstage", "Draw a random stage from Google Sheet")]
        public async Task RandomStageCommand(
            InteractionContext ctx,
            [Option("visible", "Should the result be visible for everyone?")] bool visibleForAll = false,
            [Option("onlyhome", "Restrict to stages ending with WorldHomeStage")] bool onlyHome = false)
        {
            string url = "https://docs.google.com/spreadsheets/d/1hxtcnaspE0n8jXSCL6ZijDkq1gfSQvSUzctpMmMwpDA/export?format=csv&gid=0";

            using var http = new HttpClient();
            string csv = await http.GetStringAsync(url);

            // CSV in Zeilen zerlegen und die ersten 4 Metazeilen überspringen
            var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                           .Skip(4)
                           .ToList();

            // In Datensätze parsen: (InternalName, Description)
            var rows = lines
                .Select(l => l.Split(',', 2)) // max. 2 Teile: Name, Description
                .Where(p => p.Length >= 1 && !string.IsNullOrWhiteSpace(p[0]))
                .Select(p => new {
                    Name = p[0].Trim(),
                    Description = p.Length > 1 ? p[1].Trim() : ""
                })
                .ToList();

            // Optional nur Home-Stages
            if (onlyHome)
                rows = rows.Where(r => r.Name.EndsWith("WorldHomeStage", StringComparison.Ordinal)).ToList();

            // Falls nach dem Filter nichts übrig ist
            if (rows.Count == 0)
            {
                await ctx.CreateResponseAsync(
                    new DiscordInteractionResponseBuilder()
                        .WithContent("❌ No matching stages found.")
                        .AsEphemeral(!visibleForAll)
                );
                return;
            }




            // Zufällige Auswahl
            var random = new Random();
            var pick = rows[random.Next(rows.Count)];

            string imageUrl = $"https://raw.githubusercontent.com/Amethyst-szs/smo-thumbnail-database/main/high/{pick.Name}.jpg";

            await Util.EmbedHelper.SendEmbedAsync(ctx, pick.Name, pick.Description, imageUrl, visibleForAll);

        }
        #endregion
        #endregion

            #region WhatStage
            [SlashCommand("whatstage", "Shows info and image for a given stage")]
            public async Task WhatStageCommand(
            InteractionContext ctx,
            [Option("name", "Internal stage name, e.g. WaterfallWorldHomeStage")] 
            [Autocomplete(typeof(StageAutocompleteProvider))] string stageName,
            [Option("visible", "Should the result be visible for everyone?")] bool visibleForAll = false)
            {
                string url = "https://docs.google.com/spreadsheets/d/1hxtcnaspE0n8jXSCL6ZijDkq1gfSQvSUzctpMmMwpDA/export?format=csv&gid=0";

                using var http = new HttpClient();
                string csv = await http.GetStringAsync(url);

                // CSV in Zeilen zerlegen und die ersten 4 Metazeilen überspringen
                var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                .Skip(4)
                                .ToList();

                // In Datensätze parsen: (InternalName, Description)
                var rows = lines
                    .Select(l => l.Split(',', 2))
                    .Where(p => p.Length >= 1 && !string.IsNullOrWhiteSpace(p[0]))
                    .Select(p => new {
                        Name = p[0].Trim(),
                        Description = p.Length > 1 ? p[1].Trim() : ""
                    })
                    .ToList();

                // Gesuchte Stage finden
                var match = rows.FirstOrDefault(r =>
                    r.Name.Equals(stageName, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    await ctx.CreateResponseAsync(
                        new DiscordInteractionResponseBuilder()
                            .WithContent($"❌ Stage `{stageName}` not found.")
                            .AsEphemeral(!visibleForAll)
                    );
                    return;
                }

                // Bild-URL (achte auf .jpg statt .png)
                string imageUrl = $"https://raw.githubusercontent.com/Amethyst-szs/smo-thumbnail-database/main/high/{match.Name}.jpg";

                await Util.EmbedHelper.SendEmbedAsync(ctx, match.Name, match.Description, imageUrl, visibleForAll);
            }
            #endregion
    }
    #endregion
    #region Autocomplete Provider
    /// <summary>
    /// Provides autocomplete suggestions for Discord commands by fetching data
    /// from a Google Sheets CSV. The provider:
    /// - Downloads the CSV file from the given Google Sheets URL.
    /// - Skips the first 4 header lines.
    /// - Extracts and trims the first column values.
    /// - Filters rows based on the user's current input.
    /// - Returns up to 25 matching choices (Discord's limit).
    /// </summary>
    public class StageAutocompleteProvider : IAutocompleteProvider
{
    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        string url = "https://docs.google.com/spreadsheets/d/1hxtcnaspE0n8jXSCL6ZijDkq1gfSQvSUzctpMmMwpDA/export?format=csv&gid=0";

        using var http = new HttpClient();
        string csv = await http.GetStringAsync(url);

        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                       .Skip(4)
                       .ToList();

        var rows = lines
            .Select(l => l.Split(',', 2))
            .Where(p => p.Length >= 1 && !string.IsNullOrWhiteSpace(p[0]))
            .Select(p => p[0].Trim())
            .ToList();

        // Eingabe des Users (Filter)
        string userInput = ctx.OptionValue?.ToString() ?? "";

        var matches = rows
            .Where(r => r.Contains(userInput, StringComparison.OrdinalIgnoreCase))
            .Take(25) // Discord allows max. 25 choices
            .Select(r => new DiscordAutoCompleteChoice(r, r));

        return matches;
    }
}

    public class SoundAutocompleteProvider : IAutocompleteProvider
    {
        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var options = SoundUtil.GetAutocompleteOptions();

            // Filter nach Eingabe
            string userInput = ctx.OptionValue?.ToString() ?? "";
            var filtered = options.Where(o => o.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                                  .Take(25); // Discord erlaubt max. 25 Vorschläge

            return filtered.Select(o => new DiscordAutoCompleteChoice(o, o));
        }
    }
    #endregion
}