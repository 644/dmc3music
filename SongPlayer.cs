using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace dmc3music
{
    public class SongPlayer
    {
        private static Dictionary<int, string> BossTable = new Dictionary<int, string>
        {
            {6, "Boss_01.ogg"},
            {111, "Boss_08.ogg"},
            {422, "Jester.ogg" },
            {448, "Jester.ogg" },
            {449, "Jester.ogg" },
            {121, "Boss_02.ogg"},
            {144, "Versil_01.ogg" },
            {302, "T_boss.ogg" },
            {210, "Boss_03.ogg" },
            {217, "Boss_04.ogg" },
            {228, "Boss_05.ogg" },
            {234, "Versil_02.ogg" },
            {115, "Lady.ogg" },
            {139, "Boss_06.ogg" },
            {409, "hine_02.ogg" },
            {411, "Versil_03_ver2.ogg" }
        };

        private WaveOut OutputDevice { get; set; }

        public int RoomId { get; private set; }
        
        private DMC3MusicConfig Config { get; set; }

        public SongPlayer(DMC3MusicConfig config) 
        {
            Config = config;
            OutputDevice = new WaveOut();
            RoomId = -1;
        }

        public void PlayRoomSong(int roomId, int enemyCount)
        {
            if (OutputDevice.PlaybackState == PlaybackState.Playing)
            {
                if (roomId != RoomId) Stop();
                return;
            }

            if (enemyCount > 0)
            {
                RoomId = roomId;
                if (!BossTable.TryGetValue(roomId, out string track))
                {
                    Random rnd = new Random();
                    int r = rnd.Next(Config.ShuffleRotation.Count);
                    track = Config.ShuffleRotation[r];
                }

                Console.WriteLine(track);
                OutputDevice.Dispose();
                OutputDevice = new WaveOut();
                var vorbis = new VorbisWaveReader(@"tracks/" + track);
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
