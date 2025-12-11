using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SMO_Modding_Hub_Bot.Config
{
    internal class JSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public List<ulong> admins { get; set; }
        public string quality { get; set; }

        // Neu: Mod-Daten aus mods.json
        public Dictionary<string, ModInfo> Mods { get; set; }

        public async Task ReadJson()
        {
            //
            // 1. config.json lesen
            //
            using (StreamReader sr = new("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure jsonData = JsonSerializer.Deserialize<JSONStructure>(json);

                this.token = jsonData?.token ?? "";
                this.prefix = jsonData?.prefix ?? "!";
                this.admins = jsonData?.admins ?? new List<ulong>();
                this.quality = jsonData?.quality ?? "high";

                Console.WriteLine(Util.Ansi.Green + "Config loaded successfully." + Util.Ansi.Reset);
                Console.WriteLine($"Admins: {string.Join(", ", this.admins)}, Quality: {this.quality}");
                

            }
            using (StreamReader sr = new StreamReader("mods.json"))
            {
                //
                // 2. mods.json lesen
                //
                Console.WriteLine("Loading Mods.json...");

                if (File.Exists("mods.json"))
                {

                    string json = await sr.ReadToEndAsync();
                    ModsJsonStructure modsData = JsonSerializer.Deserialize<ModsJsonStructure>(json);

                    // Sicherheit gegen null + Schlüssel lowercase
                    this.Mods = (modsData?.Mods ?? new Dictionary<string, ModInfo>())
                        .ToDictionary(k => k.Key.ToLower(), v => v.Value);


                    Console.WriteLine(Util.Ansi.Green + "Mods.json loaded successfully." + Util.Ansi.Reset);

                    foreach (var key in Mods.Keys)
                    {
                        Console.WriteLine($"Loaded mod key: '{key}'");
                    }

                }
            else
            {
                this.Mods = new Dictionary<string, ModInfo>();
                Console.WriteLine(Util.Ansi.Red + "mods.json not found!" + Util.Ansi.Reset);
            }
        }

    }

}

    /// <summary>
    /// config.json Struktur
    /// </summary>
    internal sealed class JSONStructure
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public List<ulong> admins { get; set; }
        public string quality { get; set; }
    }

    /// <summary>
    /// mods.json Struktur
    /// </summary>
    internal sealed class ModsJsonStructure
    {
        public Dictionary<string, ModInfo> Mods { get; set; }
    }

    /// <summary>
    /// Ein einzelner Mod-Datensatz
    /// </summary>
    public class ModInfo
    {
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Maker { get; set; }
    }
}
