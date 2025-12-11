using DSharpPlus;
using Microsoft.Extensions.Logging;
using SMO_Modding_Hub_Bot;
using System.Text.Json;

public class SoundUtil
{
    #region Fields and Properties
    private static readonly string SoundFilePath = "Stuff/Sound/";

    public static Dictionary<string, string[]> SoundArchives { get; private set; }
    #endregion

    #region Load Sounds on Startup
    public static void LoadSounds()
    {
        SoundArchives = new Dictionary<string, string[]>();

        if (!Directory.Exists(SoundFilePath))
        {
            
            Program.Client.Logger.LogDebug($"{Util.Ansi.Red} Sound directory not found: {SoundFilePath}" + Util.Ansi.Reset);
            
            return;
        }
            

        foreach (var dir in Directory.GetDirectories(SoundFilePath))
        {
            string folderName = Path.GetFileName(dir);

            var wavFiles = Directory.GetFiles(dir, "*.wav", SearchOption.AllDirectories)
                                    .Select(f => Path.GetFileNameWithoutExtension(f))
                                    .ToArray();

            SoundArchives[folderName] = wavFiles;
        }
#if !RELEASE
        Console.WriteLine($"{Util.Ansi.Green} Loaded {SoundArchives.Count} sound archives." + Util.Ansi.Reset);
#endif
    }
#endregion

    #region Debug Function to write sounds to json
    public static void WriteToJson(string outputFile = "sounds.json")
    {
        if (SoundArchives == null)
            LoadSounds();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true 
        };

        string json = JsonSerializer.Serialize(SoundArchives, options);
        File.WriteAllText(outputFile, json);
#if !RELEASE
        Console.WriteLine($"{Util.Ansi.Green} Sound archives written to {outputFile}" + Util.Ansi.Reset);
#endif
    }
#endregion

    #region Get Autocomplete Options
    public static IEnumerable<string> GetAutocompleteOptions()
    {
        if (SoundArchives == null)
            LoadSounds();

        return SoundArchives.SelectMany(
            kvp => kvp.Value.Select(file => $"{kvp.Key}.{file}")
        );
    }
    #endregion def Get Autocomplete Options
}
