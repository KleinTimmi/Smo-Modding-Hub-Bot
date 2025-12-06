using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.Logging;
using SMO_Modding_Hub_Bot.Commands;
using SMO_Modding_Hub_Bot.NewFolder.Sound;
using System.Collections.Concurrent;

namespace SMO_Modding_Hub_Bot
{
    internal class Program
    {
        public static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        static async Task Main(string[] args)
        {
            var JsonReader = new config.JSONReader();
            await JsonReader.ReadJson();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new DiscordChannelLoggerProvider(Client, 1441732463389970517));
            });
            var discordConfig = new DiscordConfiguration
            {
                Token = JsonReader.token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                AutoReconnect = true,
                
            };

            Client = new DiscordClient(discordConfig);
            Client.Ready += OnClientReady;
            Client.GuildCreated += async (s, e) =>
            {
                //these wired symbols are for coloring the console output (in this case Blue)
                Client.Logger.LogInformation("\u001b[36mBot joined guild: {GuildName} ({GuildId})\u001b[0m", e.Guild.Name, e.Guild.Id);
            };

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] { JsonReader.prefix },
                EnableDms = true,
                EnableMentionPrefix = true,
                EnableDefaultHelp = false,
                CaseSensitive = false,
                

            };

            Commands = Client.UseCommandsNext(commandsConfig);
            var slash = Client.UseSlashCommands();


            slash.RegisterCommands<Slash>();
            slash.RegisterCommands<Mods>();
            slash.RegisterCommands<Resource>();
            slash.RegisterCommands<Sound>();

            // Logging
            Commands.CommandExecuted += async (s, e) => await LogCommandExecution(e.Context);
            Commands.CommandErrored += async (s, e) => await LogCommandError(e);

            slash.SlashCommandExecuted += async (s, e) => await LogSlashExecution(e.Context);
            slash.SlashCommandErrored += async (s, e) => await LogSlashError(e);

            SoundUtil.LoadSounds();
            #if !RELEASE
            SoundUtil.WriteToJson();
            #endif
            Console.WriteLine(Util.AnsiAttribute.Green + "Commands registered" + Util.AnsiAttribute.Reset );

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        // ---- Hilfsmethoden auf Klassenebene ----

        private static Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            //these wired symbols are for coloring the console output (in this case Green)
            Console.WriteLine(Util.AnsiAttribute.Green + "Bot is online!" + Util.AnsiAttribute.Reset);
            return Task.CompletedTask;
        }
        #region Logging helper

        private static Task LogCommandExecution(CommandContext ctx)
        {
            //these wired symbols are for coloring the console output (in this case Green)
            Client.Logger.LogInformation("\u001b[32m{User} executed slash {Command} in {Channel} ({Guild})\u001b[0m",
                ctx.User.Username,
                ctx.Command.Name,
                ctx.Channel.Name,
                ctx.Guild?.Name ?? "DM");

            return Task.CompletedTask;
        }
        public static ConcurrentQueue<string> ExtraLogs = new();


        private static Task LogSlashExecution(InteractionContext ctx)
        {
            Client.Logger.LogInformation(Util.AnsiAttribute.Green + "{User} executed slash {Command} in {Channel} ({Guild})"+ Util.AnsiAttribute.Reset ,
                ctx.User.Username,
                ctx.CommandName,
                ctx.Channel.Name,
                ctx.Guild?.Name ?? "DM");

            if (ExtraLogs.TryDequeue(out var extraLog))
                Client.Logger.LogInformation(Util.AnsiAttribute.Cyan + extraLog + Util.AnsiAttribute.Reset );

            return Task.CompletedTask;
        }


        private static Task LogCommandError(CommandErrorEventArgs e)
        {
            var ctx = e.Context;
            Client.Logger.LogError(e.Exception, Util.AnsiAttribute.Red + "Error in command {Command} by {User}" + Util.AnsiAttribute.Reset,
                ctx.Command?.Name,
                ctx.User.Username);
            return Task.CompletedTask;
        }

        private static Task LogSlashError(SlashCommandErrorEventArgs e)
        {
            var ctx = e.Context;
            var message = $"{Util.AnsiAttribute.Red}Error in slash command {ctx.CommandName} by {ctx.User.Username}\n{e.Exception}{Util.AnsiAttribute.Reset}";
            Client.Logger.LogError(message);
            return Task.CompletedTask;
        }

        #endregion
    }
    public class DiscordChannelLoggerProvider : ILoggerProvider
    {
        private readonly DiscordClient _client;
        private readonly ulong _channelId;

        public DiscordChannelLoggerProvider(DiscordClient client, ulong channelId)
        {
            _client = client;
            _channelId = channelId;
        }

        public ILogger CreateLogger(string categoryName)
            => new DiscordChannelLogger(_client, _channelId);

        public void Dispose() { }
    }

    public class DiscordChannelLogger : ILogger
    {
        private readonly DiscordClient _client;
        private readonly ulong _channelId;

        public DiscordChannelLogger(DiscordClient client, ulong channelId)
        {
            _client = client;
            _channelId = channelId;
        }

        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            // Normales Logging in die Konsole
            Console.WriteLine($"[{logLevel}] {message}");

            // Nur Warnings und Errors in den Channel schreiben
            if (logLevel >= LogLevel.Warning)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var channel = await _client.GetChannelAsync(_channelId);
                        await channel.SendMessageAsync($"[{logLevel}] {message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Channel-Log: {ex.Message}");
                    }
                });
            }
        }
    }

}
