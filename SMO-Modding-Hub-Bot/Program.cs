using DSharpPlus;
using DSharpPlus.CommandsNext;
using SMO_Modding_Hub_Bot.Commands;
using Microsoft.Extensions.Logging;

namespace SMO_Modding_Hub_Bot
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        static async Task Main(string[] args)
        {
            var JsonReader = new config.JSONReader();
            await JsonReader.ReadJson();

            var discordConfig = new DiscordConfiguration
            {
                Token = JsonReader.token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Information // sorgt dafür, dass Logs sichtbar sind
            };

            Client = new DiscordClient(discordConfig);

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { JsonReader.prefix },
                EnableDms = true,
                EnableMentionPrefix = true,
                EnableDefaultHelp = true,
                CaseSensitive = false

            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<Resource>();

            // 🔥 Globales Logging für alle Commands
            Commands.CommandExecuted += async (s, e) =>
            {
                var ctx = e.Context;
                Client.Logger.LogInformation("{User} executed {Command} in {Channel} ({Guild})",
                    ctx.User.Username,
                    ctx.Command.Name,
                    ctx.Channel.Name,
                    ctx.Guild?.Name ?? "DM");
            };

            // 🔥 Optional: Fehler-Logging
            Commands.CommandErrored += async (s, e) =>
            {
                var ctx = e.Context;
                Client.Logger.LogError(e.Exception, "Error in command {Command} by {User}",
                    ctx.Command?.Name,
                    ctx.User.Username);
            };

            Console.WriteLine("registert Commands");

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            Console.WriteLine("Bot is online!");
            return Task.CompletedTask;
        }
    }
}
