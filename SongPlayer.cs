using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace dmc3music
{
    public class SongPlayer
    {
        private static Dictionary<int, string> SongTable = new Dictionary<int, string>
        {
            {0, "Battle_00.ogg"},
            {1, "Battle_01.ogg"},
            {2, "Battle_01.ogg"},
            {6, "Boss_01.ogg"}
        };

        private WaveOut OutputDevice { get; set; }

        public int RoomId { get; private set; }

        public SongPlayer() 
        {
            OutputDevice = new WaveOut();
            RoomId = -1;
        }

        public void PlayRoomSong(int roomId)
        {
            if (
                OutputDevice.PlaybackState == PlaybackState.Playing &&
                RoomId == roomId
            ) return;
            
            if (SongTable.TryGetValue(roomId, out string track))
            {
                RoomId = roomId;
                Console.WriteLine(track);
                OutputDevice.Dispose();
                OutputDevice = new WaveOut();
                var vorbis = new VorbisWaveReader(@"D:\SteamLibrary\steamapps\common\Devil May Cry 3\native\sound\" + track);
                OutputDevice.Init(vorbis);
                OutputDevice.Play();
            }
        }

        public void Stop()
        {
            if (OutputDevice.PlaybackState == PlaybackState.Playing) 
                OutputDevice.Stop();
        }
    }
}
