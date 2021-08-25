using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace dmc3music
{
    public static class TrackReader
    {
        public static List<string> ReadTracks(string trackPath, string[] fileTypes)
        {
            var tracks = new List<string>();
            foreach (string fileType in fileTypes)
            {
                foreach (string filename in Directory.EnumerateFiles(trackPath, fileType))
                {
                    string trackName = filename.Split('\\').Last();
                    tracks.Add(trackName);
                }
            }
            return tracks;
        }
    }
}
