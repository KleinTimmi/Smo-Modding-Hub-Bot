using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.Logging;
using SMO_Modding_Hub_Bot.Commands;
using SMO_Modding_Hub_Bot.Stuff.Sound;
using System.Collections.Concurrent;

namespace SMO_Modding_Hub_Bot
{
    internal class Program
    {
        public static Config.JSONReader Config;
        public static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        private static FileSystemWatcher ModsWatcher;


        static async Task Main(string[] args)
        {
            Client.Logger.LogDebug(Util.Ansi.Blue + "Starting SMO Modding Hub Bot..." + Util.Ansi.Reset );

            Config = new Config.JSONReader();
            await Config.ReadJson();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new DiscordChannelLoggerProvider(Client, 1441732463389970517));
            });
            var discordConfig = new DiscordConfiguration
            {
                Token = Config.token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                AutoReconnect = true,
                
            };

            Client = new DiscordClient(discordConfig);
            Client.Ready += OnClientReady;
            Client.GuildCreated += async (s, e) =>
            {
                Client.Logger.LogInformation(Util.Ansi.Cyan + "Bot joined guild: {GuildName} ({GuildId})" + Util.Ansi.Reset , e.Guild.Name, e.Guild.Id);
            };

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] { Config.prefix },
                EnableDms = true,
                EnableMentionPrefix = true,
                EnableDefaultHelp = false,
                CaseSensitive = false,
                

            };
            if (Program.Config.Mods.Keys.Count == 0)
            {
                Program.Client.Logger.LogDebug(Util.Ansi.Yellow + "Warning: No mods loaded from config!" + Util.Ansi.Reset );
            }




            #region Mods.json Watcher

            ModsWatcher = new FileSystemWatcher
            {
                Path = AppContext.BaseDirectory,
                Filter = "mods.json",
                NotifyFilter = NotifyFilters.LastWrite
                             | NotifyFilters.FileName
                             | NotifyFilters.Size,
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };

            Program.Client.Logger.LogDebug($"{ModsWatcher.Path} + {ModsWatcher.Filter}");
            Program.Client.Logger.LogDebug(Util.Ansi.Green + "FileSystemWatcher for mods.json created successfully" + Util.Ansi.Reset);

            // Events registrieren
            ModsWatcher.Changed += OnModsJsonChanged;
            ModsWatcher.Created += OnModsJsonChanged;
            ModsWatcher.Renamed += OnModsJsonChanged;
            ModsWatcher.Deleted += OnModsJsonChanged;
            ModsWatcher.Error += (s, e) =>
            {
                Program.Client.Logger.LogDebug(Util.Ansi.Red + $"FileSystemWatcher error: {e.GetException().Message}" + Util.Ansi.Reset);
            };

            #endregion

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
            
            SoundUtil.WriteToJson();
            
            Program.Client.Logger.LogDebug(Util.Ansi.Green + "Commands registered" + Util.Ansi.Reset );

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        // ---- Hilfsmethoden auf Klassenebene ----
        private static bool _modsJsonProcessing = false;
        private static DateTime _lastTrigger = DateTime.MinValue;
        private static async void OnModsJsonChanged(object sender, FileSystemEventArgs e)
        {
            var now = DateTime.Now;

            // Ignoriere Events innerhalb von 300 ms
            if (_modsJsonProcessing || (now - _lastTrigger).TotalMilliseconds < 300)
                return;

            _modsJsonProcessing = true;
            _lastTrigger = now;

            Program.Client.Logger.LogDebug($"mods.json event: {e.ChangeType}");

            try
            {
                await Program.Config.ReadJson();
            }
            finally
            {
                // minimal delay
                await Task.Delay(300);
                _modsJsonProcessing = false;
            }
        }

        private static Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            Program.Client.Logger.LogDebug(Util.Ansi.Green + "Bot is online!" + Util.Ansi.Reset);
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
            Client.Logger.LogInformation(Util.Ansi.Green + "{User} executed slash {Command} in {Channel} ({Guild})"+ Util.Ansi.Reset ,
                ctx.User.Username,
                ctx.CommandName,
                ctx.Channel.Name,
                ctx.Guild?.Name ?? "DM");

            if (ExtraLogs.TryDequeue(out var extraLog))
                Client.Logger.LogInformation(Util.Ansi.Cyan + extraLog + Util.Ansi.Reset );

            return Task.CompletedTask;
        }


        private static Task LogCommandError(CommandErrorEventArgs e)
        {
            var ctx = e.Context;
            Client.Logger.LogError(e.Exception, Util.Ansi.Red + "Error in command {Command} by {User}" + Util.Ansi.Reset,
                ctx.Command?.Name,
                ctx.User.Username);
            return Task.CompletedTask;
        }

        private static Task LogSlashError(SlashCommandErrorEventArgs e)
        {
            var ctx = e.Context;
            var message = $"{Util.Ansi.Red}Error in slash command {ctx.CommandName} by {ctx.User.Username}\n{e.Exception}{Util.Ansi.Reset}";
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
            Program.Client.Logger.LogDebug($"[{logLevel}] {message}");

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
                        Program.Client.Logger.LogDebug($"Fehler beim Channel-Log: {ex.Message}");
                    }
                });
            }
        }
    }

}
