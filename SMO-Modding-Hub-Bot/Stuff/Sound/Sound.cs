using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SMO_Modding_Hub_Bot.Commands;

namespace SMO_Modding_Hub_Bot.Stuff.Sound
{
    internal class Sound : ApplicationCommandModule
    {

        #region PlaySound 
        [SlashCommand("playSound", "Play a sound")]
        public async Task PlaySoundCommand(
                InteractionContext ctx,
                [Option("sound", "Choose a sound")]
                [Autocomplete(typeof(SoundAutocompleteProvider))] string sound,
                [Option("ephemeral", "Send message only visible to you")] bool ephemeral = false
            )
        {
            if (sound.Contains('.'))
            {
                var parts = sound.Split('.');
                if (parts.Length != 2)
                {
                    await ctx.CreateResponseAsync("❌ not found.");
                    return;
                }

                string folder = parts[0];
                string fileName = parts[1] + ".wav";
                string fullPath = Path.Combine("Stuff", "Sound", folder, fileName);

                if (!File.Exists(fullPath))
                {
                    await ctx.CreateResponseAsync($"❌ File Not found: {fullPath}");
                    return;
                }

                // Acknowledge quickly
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.DeferredChannelMessageWithSource);

                // Then send the file as a follow-up
                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    var followup = new DiscordFollowupMessageBuilder()
                        .AddFile(fileName, fs);

                    await ctx.FollowUpAsync(followup);
                }

            }
            else
            {
                string folderPath = Path.Combine("Stuff", "Sound", sound);
                if (!Directory.Exists(folderPath))
                {
                    await ctx.CreateResponseAsync($"❌ folder not found: {folderPath}");
                    return;
                }

                var files = Directory.GetFiles(folderPath, "*.wav");
                if (files.Length == 0)
                {
                    await ctx.CreateResponseAsync($"❌ no sounds in folder {sound} found.");
                    return;
                }

                var fileNames = files.Select(f => Path.GetFileNameWithoutExtension(f));
                string response = $"🎵 Available sounds in **{sound}**:\n" +
                                  string.Join("\n", fileNames.Select(fn => $"- {sound}.{fn}"));

                if (response.Length > 2000)
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(response);
                    using (var ms = new MemoryStream(bytes))
                    {
                        var builder = new DiscordInteractionResponseBuilder()
                            .AddFile($"{sound}_sounds.txt", ms);

                        if (ephemeral)
                            builder.AsEphemeral(true);

                        await ctx.CreateResponseAsync(
                            DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                            builder
                        );
                    }
                }
                else
                {
                    await ctx.CreateResponseAsync(response);
                }
            }
        }
        #endregion

        #region RandomSound
        [SlashCommand("randomsound", "Play a random sound")]
        public async Task RandomSoundCommand(
            InteractionContext ctx,
            [Option("archive", "Choose a character/archive (optional)")] string archive = null,
            [Option("ephemeral", "Send message only visible to you")] bool ephemeral = false
        )
        {
            var random = new Random();

            string character;
            if (!string.IsNullOrEmpty(archive) && SoundUtil.SoundArchives.ContainsKey(archive))
            {
                // User specified an archive
                character = archive;
            }
            else
            {
                // Choose a random archive
                character = SoundUtil.SoundArchives.Keys.ElementAt(random.Next(SoundUtil.SoundArchives.Count));
            }

            // Choose a random sound from the archive
            var sounds = SoundUtil.SoundArchives[character];
            var sound = sounds[random.Next(sounds.Length)];

            // Build path to the file
            string folderPath = Path.Combine("Stuff", "Sound", character);
            string fullPath = Path.Combine(folderPath, sound + ".wav");

            if (!File.Exists(fullPath))
            {
                await ctx.CreateResponseAsync($"❌ File not found: {fullPath}");
                return;
            }
            using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                // Build embed with title
                var embed = new DiscordEmbedBuilder()
                .WithTitle($"{character}: {sound}");

                // ResponseBuilder with file + embed
                var builder = new DiscordInteractionResponseBuilder()
                    .AddFile($"{sound}.wav", fs)
                    .AddEmbed(embed);

                if (ephemeral)
                    builder.AsEphemeral(true);

                await ctx.CreateResponseAsync(
                    DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                    builder
                );
            }


            Program.ExtraLogs.Enqueue($"{character}: {sound} (invisible: {ephemeral})");
        }
        #endregion

    }
}
