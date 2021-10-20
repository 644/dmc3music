using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;

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

        public bool isFading { get; set; } = false;

        public float OldVolume { get; set; } = 1.0f;

        public double TrackPos { get; set; } = 0.0d;

        public int TrackLength { get; set; } = 0;

        public double SkipLength { get; set; } = 0;

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
            isFading = true;
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
            if (missionNumber != oldMissionNumber)
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
                    OutputDevice.Dispose();
                    OutputDevice = new WaveOut();
                    VorbisWaveReader vorbis = new VorbisWaveReader(Path.Combine(Config.MusicPath, track + Config.ExtensionType));
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

                    if (TrackStartTime.TryGetValue(OldTrack, out long startTime))
                    {
                        TrackPos = SkipLength + DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
                    }

                    if (TrackPositions.TryGetValue(OldTrack, out _))
                    {
                        TrackPositions[OldTrack] = TrackPos;
                    }
                    else
                    {
                        TrackPositions.Add(OldTrack, TrackPos);
                    }

                    if (isFading)
                    {
                        if (fadeTimer++ >= 10)
                        {
                            Stop();
                        }
                        return;
                    }

                    if (roomId != RoomId)
                    {
                        FadeOut();
                        return;
                    }

                    if (enemyCount > 0)
                    {
                        EnemiesGoneTimer = 0;
                        if (AmbientIsPlaying && Config.RoomTracks.TryGetValue(roomId.ToString() + "_" + missionNumber.ToString(), out _))
                        {
                            Stop();
                        }
                    }
                    else if (!AmbientIsPlaying)
                    {
                        if (EnemiesGoneTimer++ >= 120)
                        {
                            if (OutputDevice.Volume >= 0.01f)
                            {
                                OutputDevice.Volume -= 0.01f;
                            }
                            else if (OldVolume > 0.0f)
                            {
                                Stop();
                            }
                        }
                    }

                    return;
                }

                EnemiesGoneTimer = 0;
                RoomId = roomId;
                string track;

                if (enemyCount > 0 && Config.RoomTracks.TryGetValue(roomId.ToString() + "_" + missionNumber.ToString(), out List<string> trackInfo))
                {
                    AmbientIsPlaying = false;
                    track = trackInfo[0];
                }
                else
                {
                    if (Config.AmbientTracks.TryGetValue(roomId.ToString() + "_" + missionNumber.ToString(), out trackInfo))
                    {
                        AmbientIsPlaying = true;
                        track = trackInfo[0];
                    }
                    else
                    {
                        return;
                    }
                }

                OutputDevice.Dispose();
                OutputDevice = new WaveOut();

                vorbis = new VorbisWaveReader(Path.Combine(Config.MusicPath, track + Config.ExtensionType));

                if (track != OldTrack)
                {
                    TrackLength = Convert.ToInt32(vorbis.TotalTime.TotalSeconds);
                }

                try
                {
                    long currentTime = 0;

                    if (TrackStartTime.TryGetValue(OldTrack, out long startTime))
                    {
                        currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    }

                    if (TrackPositions.TryGetValue(track, out double pos))
                    {
                        if (TrackLength - (pos / 1000) <= 3)
                        {
                            TrackLength = Convert.ToInt32(vorbis.TotalTime.TotalSeconds);
                            vorbis.Skip(Convert.ToInt32(trackInfo[1]));
                            TrackLength -= Convert.ToInt32(trackInfo[1]);
                        }
                        else if ((pos / 1000 < TrackLength) && (AmbientIsPlaying && currentTime - startTime < (Config.AmbientTimer * 1000)) || currentTime - startTime < (Config.BattleTimer * 1000))
                        {
                            vorbis.Skip(Convert.ToInt32(pos / 1000));
                            SkipLength = pos;
                        }
                        else
                        {
                            TrackPos = 0;
                            SkipLength = 0;
                        }
                    }
                    else
                    {
                        TrackPos = 0;
                        SkipLength = 0;
                    }
                }
                catch { }

                fade = new FadeInOutSampleProvider(vorbis, true);
                if (OldTrack == track)
                {
                    fade.BeginFadeIn(50);
                }
                else
                {
                    fade.BeginFadeIn(Convert.ToInt32(trackInfo[2]));
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
            }
        }

        public void Volume(float vol)
        {
            if (vol < 0.0f)
            {
                OutputDevice.Volume = 0.0f;
            }
            else
            {
                OutputDevice.Volume = vol;
            }

            OldVolume = OutputDevice.Volume;
        }

        public void Stop()
        {
            isPlaying = false;
            if (OutputDevice.PlaybackState == PlaybackState.Playing)
            {
                OutputDevice.Stop();
                AmbientIsPlaying = false;
                fadeTimer = 0;
                isFading = false;
            }
        }
    }
}
