using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;

namespace dmc3music
{
    public class SongPlayer
    {
        private WaveOut OutputDevice { get; set; }

        public VorbisWaveReader vorbis { get; set; }

        public FadeInOutSampleProvider fade { get; set; }

        public int RoomId { get; set; }

        public int fadeTimer { get; set; } = 0;

        public int oldMissionNumber { get; set; }

        public int EnemiesGoneTimer { get; set; }

        public bool AmbientIsPlaying { get; set; }

        public bool isPlaying { get; set; } = false;

        public float OldVolume { get; set; } = 1.0f;

        public double TrackPos { get; set; } = 0.0d;

        public int TrackPercent { get; set; } = 0;

        public Dictionary<string, double> TrackPositions { get; set; } = new Dictionary<string, double>();

        public Dictionary<string, long> TrackStartTime { get; set; } = new Dictionary<string, long>();

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

        public void FadeOut()
        {
            if (fadeTimer == 0)
            {
                fade.BeginFadeOut(2000);
            }
            if (fadeTimer++ >= 10)
            {
                Stop();
            }
        }

        public void PlayRoomSong(int roomId, int enemyCount, int missionNumber)
        {
            if(missionNumber != oldMissionNumber)
            {
                oldMissionNumber = missionNumber;
                TrackPositions.Clear();
                TrackStartTime.Clear();
            }

            if (Config.Shuffle)
            {
                if (OutputDevice.PlaybackState == PlaybackState.Playing)
                {
                    if (roomId != RoomId)
                    {
                        FadeOut();
                    }
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
                    //TrackPos = OutputDevice.GetPosition() * 1000.0 / OutputDevice.OutputWaveFormat.BitsPerSample / OutputDevice.OutputWaveFormat.Channels * 8 / OutputDevice.OutputWaveFormat.SampleRate;
                    TrackPos += 50;

                    if (TrackPositions.TryGetValue(OldTrack, out _))
                    {
                        TrackPositions[OldTrack] = TrackPos;
                    }
                    else
                    {
                        TrackPositions.Add(OldTrack, TrackPos);
                    }

                    if (roomId != RoomId)
                    {
                        FadeOut();
                        return;
                    }
                    
                    if(fadeTimer > 0)
                    {
                        fadeTimer = 0;
                    }
                    
                    if (enemyCount > 0)
                    {
                        EnemiesGoneTimer = 0;
                        if (AmbientIsPlaying && Config.RoomTracks.TryGetValue(roomId.ToString() + "_" + missionNumber.ToString(), out _)) Stop();
                    }
                    else if (!AmbientIsPlaying)
                    {
                        if (EnemiesGoneTimer++ >= 120)
                        {
                            if (OutputDevice.Volume >= 0.01f)
                                OutputDevice.Volume -= 0.01f;
                            else if (OldVolume > 0.0f)
                                Stop();
                        }
                    }

                    return;
                }

                EnemiesGoneTimer = 0;
                RoomId = roomId;

                if (enemyCount > 0 && Config.RoomTracks.TryGetValue(roomId.ToString() + "_" + missionNumber.ToString(), out string track))
                {
                    AmbientIsPlaying = false;
                }
                else
                {
                    if (Config.AmbientTracks.TryGetValue(roomId.ToString() + "_" + missionNumber.ToString(), out track))
                    {
                        AmbientIsPlaying = true;
                    }
                    else
                    {
                        return;
                    }
                }

                OutputDevice.Dispose();
                OutputDevice = new WaveOut();
                vorbis = new VorbisWaveReader(@"tracks/" + track);
                int TrackLength = Convert.ToInt32(vorbis.TotalTime.TotalSeconds);

                try
                {
                    long currentTime = 0;
                    long startTime = 0;
                    
                    if(TrackStartTime.TryGetValue(OldTrack, out startTime)){
                        currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    }

                    if (TrackPositions.TryGetValue(track, out double pos) && Convert.ToInt32(pos / 1000) < TrackLength)
                    {
                        if ((AmbientIsPlaying && currentTime - startTime < (Config.AmbientTimer * 1000)) || currentTime - startTime < (Config.BattleTimer * 1000))
                        {
                            vorbis.Skip(Convert.ToInt32(pos / 1000));
                        }
                    }
                    else
                    {
                        TrackPos = 0;
                    }
                }
                catch { }

                fade = new FadeInOutSampleProvider(vorbis, true);
                if(OldTrack == track)
                {
                    fade.BeginFadeIn(100);
                } else
                {
                    fade.BeginFadeIn(2000);
                }
                OldTrack = track;
                OutputDevice.Volume = OldVolume;
                OutputDevice.Init(fade);
                OutputDevice.Play();
                long timeStarted = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (TrackStartTime.TryGetValue(OldTrack, out _))
                {
                    TrackStartTime[OldTrack] = timeStarted;
                }
                else
                {
                    TrackStartTime.Add(OldTrack, timeStarted);
                }
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
                fadeTimer = 0;
            }
        }
    }
}
