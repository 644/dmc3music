using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace dmc3music
{
    public class SongPlayer
    {
        private WaveOut OutputDevice { get; set; }

        public int RoomId { get; set; }

        public int EnemiesGoneTimer { get; set; }

        public bool AmbientIsPlaying { get; set; }

        public bool isPlaying { get; set; } = false;

        public float OldVolume { get; set; } = 1.0f;

        public double TrackPos { get; set; } = 0.0d;

        public int TrackPercent { get; set; } = 0;

        public Dictionary<string, double> TrackPositions { get; set; } = new Dictionary<string, double>();

        public string OldTrack { get; set; }

        private DMC3MusicConfig Config { get; set; }

        public SongPlayer(DMC3MusicConfig config) 
        {
            Config = config;
            OutputDevice = new WaveOut();
            RoomId = -1;
            EnemiesGoneTimer = 0;
            AmbientIsPlaying = false;
        }

        public void PlayRoomSong(int roomId, int enemyCount)
        {
            if (Config.Shuffle)
            {
                if (OutputDevice.PlaybackState == PlaybackState.Playing)
                {
                    if (roomId != RoomId) Stop();
                    return;
                }

                if (enemyCount > 0)
                {
                    RoomId = roomId;
                    Random rnd = new Random();
                    int r = rnd.Next(Config.ShuffleRotation.Count);
                    string track = Config.ShuffleRotation[r];

                    OldTrack = track;
                    Console.WriteLine(track);
                    OutputDevice.Dispose();
                    OutputDevice = new WaveOut();
                    var vorbis = new VorbisWaveReader(@"tracks/" + track);
                    OutputDevice.Init(vorbis);
                    OutputDevice.Play();
                }
            }
            else
            {
                if (OutputDevice.PlaybackState == PlaybackState.Playing)
                {
                    isPlaying = true;
                    TrackPos = OutputDevice.GetPosition() * 1000.0 / OutputDevice.OutputWaveFormat.BitsPerSample / OutputDevice.OutputWaveFormat.Channels * 8 / OutputDevice.OutputWaveFormat.SampleRate;

                    if (TrackPositions.TryGetValue(OldTrack, out _))
                        TrackPositions[OldTrack] = TrackPos;
                    else
                        TrackPositions.Add(OldTrack, TrackPos);

                    if (roomId != RoomId)
                    {
                        Stop();
                    }
                    else if (enemyCount > 0)
                    {
                        EnemiesGoneTimer = 0;
                        if (AmbientIsPlaying && Config.RoomTracks.TryGetValue(roomId.ToString(), out _)) Stop();
                    }
                    else if (!AmbientIsPlaying)
                    {
                        if (EnemiesGoneTimer++ >= 24)
                        {
                            if (OutputDevice.Volume >= 0.05f)
                                OutputDevice.Volume -= 0.05f;
                            else if (OldVolume > 0.0f)
                                Stop();
                        }
                    }

                    return;
                }

                EnemiesGoneTimer = 0;
                RoomId = roomId;

                if (enemyCount > 0 && Config.RoomTracks.TryGetValue(roomId.ToString(), out string track))
                {
                    AmbientIsPlaying = false;
                }
                else
                {
                    if (Config.AmbientTracks.TryGetValue(roomId.ToString(), out track))
                    {
                        AmbientIsPlaying = true;
                    }
                    else
                    {
                        Random rnd = new Random();
                        int r = rnd.Next(Config.ShuffleRotation.Count);
                        track = Config.ShuffleRotation[r];
                    }
                }

                OutputDevice.Dispose();
                OutputDevice = new WaveOut();
                var vorbis = new VorbisWaveReader(@"tracks/" + track);
                int TrackLength = Convert.ToInt32(vorbis.TotalTime.TotalSeconds / 2);

                try
                {
                    if (TrackPositions.TryGetValue(track, out double pos) && Convert.ToInt32(pos / 1000) < TrackLength)
                    {
                        Console.WriteLine(Convert.ToInt32(pos / 1000).ToString() + " " + TrackLength.ToString());
                        vorbis.Skip(Convert.ToInt32(pos / 1000));
                    }
                }
                catch { }

                OldTrack = track;
                OutputDevice.Volume = OldVolume;
                OutputDevice.Init(vorbis);
                OutputDevice.Play();
                Console.WriteLine(track);
            }
        }

        public void Volume(float vol)
        {
            if (vol < 0.0f)
                OutputDevice.Volume = 0.0f;
            else
                OutputDevice.Volume = vol;

            OldVolume = OutputDevice.Volume;
        }

        public void Stop()
        {
            if (OutputDevice.PlaybackState == PlaybackState.Playing)
            {
                OutputDevice.Stop();
                isPlaying = false;
                AmbientIsPlaying = false;
            }
        }
    }
}
