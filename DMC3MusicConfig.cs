using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace dmc3music
{
    public class DMC3MusicConfig
    {
        public string MusicPath { get; set; }
        public string DMC3Path { get; set; }
        public Dictionary<string, List<string>> RoomTracks { get; set; }
        public Dictionary<string, List<string>> AmbientTracks { get; set; }
        public bool Shuffle { get; set; }
        public bool CutsceneMovement { get; set; }
        public List<string> ShuffleRotation { get; set; }
        public int BattleTimer { get; set; }
        public int AmbientTimer { get; set; }
        public string ExtensionType { get; set; } = ".ogg";

        internal object GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public static class DMC3MusicConfigWriter
    {
        public static DMC3MusicConfig ReadConfig()
        {
            return JsonSerializer.Deserialize<DMC3MusicConfig>(File.ReadAllText("./Config/config.json"));
        }

        public static void WriteConfig(DMC3MusicConfig config)
        {
            File.WriteAllText("./Config/config.json", JsonSerializer.Serialize(config));
        }

        public static DMC3MusicConfig ResetConfig()
        {
            DMC3MusicConfig defaultConfig = JsonSerializer.Deserialize<DMC3MusicConfig>(File.ReadAllText("./Config/config.default.json"));
            WriteConfig(defaultConfig);
            return ReadConfig();
        }
    }
}
