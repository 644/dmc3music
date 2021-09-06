﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace dmc3music
{
    public class DMC3MusicConfig
    {
        public string MusicPath { get; set; }
        public Dictionary<string, string> RoomTracks { get; set; }
        public Dictionary<string, string> AmbientTracks { get; set; }
        public bool Shuffle { get; set; }
        public List<string> ShuffleRotation { get; set; }

        internal object GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public static class DMC3MusicConfigWriter
    {
        public static DMC3MusicConfig ReadConfig() =>
            JsonSerializer.Deserialize<DMC3MusicConfig>(File.ReadAllText("./Config/config.json"));

        public static void WriteConfig(DMC3MusicConfig config) =>
            File.WriteAllText("./Config/config.json", JsonSerializer.Serialize(config));

        public static DMC3MusicConfig ResetConfig()
        {
            DMC3MusicConfig defaultConfig = JsonSerializer.Deserialize<DMC3MusicConfig>(File.ReadAllText("./Config/config.default.json"));
            WriteConfig(defaultConfig);
            return ReadConfig();
        }
    }
}