using Microsoft.Extensions.Logging;

namespace SMO_Modding_Hub_Bot.config
{
    internal class JSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public List<ulong> admins { get; set; }

        public async Task ReadJson()
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure jsonData = System.Text.Json.JsonSerializer.Deserialize<JSONStructure>(json);
                this.token = jsonData.token;
                this.prefix = jsonData.prefix;
                this.admins = jsonData.admins;
#if !RELEASE
                Console.WriteLine(Util.AnsiAttribute.Green + "Config", "Configuration file loaded successfully." + Util.AnsiAttribute.Reset );
#endif
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public List<ulong> admins { get; set; }
    }
}
