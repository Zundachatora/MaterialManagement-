using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayingWavFilesScript : FileFolderLoadingScript
{

    [SerializeField] private GameObject _audioSourceWithGameObject;

    private AudioSource _audioSource;
    private string _playingClipUserPath="";

    public bool WavPlay(string userPath,out float playbackTime)
    {
        playbackTime = 0;

        var path = PathConversion(userPath);
        if (path == null) return false;

        //現在読み込んでいるwavと異なるwavを読み込む時。又はクリップが消滅している時に処理する。
        if ((!_playingClipUserPath.Equals(path.Value.userPath)) || (_audioSource.clip == null))
        {
            if (_audioSource.clip != null) AudioClip.Destroy(_audioSource.clip);

            _audioSource.clip = GenerateAudioClipFromWav(path.Value.userPath);

            _playingClipUserPath = path.Value.userPath;
        }

        _audioSource.Play();

        playbackTime = _audioSource.clip.length;

        return true;
    }



    public void WavStop()
    {
        _audioSource.Stop();
    }

    protected override void Awake()
    {
        base.Awake();

        _audioSource = _audioSourceWithGameObject.GetComponent<AudioSource>();
    }
    

    private AudioClip GenerateAudioClipFromWav(string userPath)
    {
        //参考とライセンス表記
        // LICENSE/SourceCode/Wav2AudioClipSample
        //https://www.hanachiru-blog.com/entry/2022/08/01/120000

        var path = PathConversion(userPath);

        //WAVEファイルを読み取り、解析する
        var fileBytes = File.ReadAllBytes(path.Value.fullPath);
        using var memoryStream = new MemoryStream(fileBytes);

        // RIFF
        var riffBytes = new byte[4];
        memoryStream.Read(riffBytes);
        if (riffBytes[0] != 0x52 || riffBytes[1] != 0x49 || riffBytes[2] != 0x46 || riffBytes[3] != 0x46) throw new ArgumentException("fileBytes is not the correct Wav file format.");

        // chunk size
        var chunkSizeBytes = new byte[4];
        memoryStream.Read(chunkSizeBytes);
        var chunkSize = BitConverter.ToInt32(chunkSizeBytes);

        // WAVE
        var wavBytes = new byte[4];
        memoryStream.Read(wavBytes);
        if (wavBytes[0] != 0x57 || wavBytes[1] != 0x41 || wavBytes[2] != 0x56 || wavBytes[3] != 0x45) throw new ArgumentException("fileBytes is not the correct Wav file format.");

        //サブチャンクID
        var subChunkID = new byte[4];
        memoryStream.Read(subChunkID);

        //サブチャンクサイズ
        var fmtSizeBytes = new byte[4];
        memoryStream.Read(fmtSizeBytes);
        var subChunkSize = BitConverter.ToInt32(fmtSizeBytes);

        //fmt のSize、IDが出るまでループ
        while (true)
        {
            /*デバック用
            Debug.Log("ループ開始");
            foreach (var item in subChunkID)
            {
                Debug.Log("16進数[0x" + item.ToString("X")+"]char["+(char)item+"]");
            }
            */
            if (subChunkID[0] == 0x66 || subChunkID[1] == 0x6d || subChunkID[2] == 0x74 || subChunkID[3] == 0x20)
            {// "fmt "
                break;
            }
            else //if (subChunkID[0] == 0x4A || subChunkID[1] == 0x55 || subChunkID[2] == 0x4E || subChunkID[3] == 0x4B)
            {

                //スキップ
                memoryStream.Seek(subChunkSize, SeekOrigin.Current);

                subChunkID = new byte[4];
                memoryStream.Read(subChunkID);

                // subChunkSize
                fmtSizeBytes = new byte[4];
                memoryStream.Read(fmtSizeBytes);
                subChunkSize = BitConverter.ToInt32(fmtSizeBytes);

            }
            /*
            else
            {
                throw new ArgumentException("fileBytes is not the correct Wav file format.");
            }
            */
        }

        // AudioFormat
        var audioFormatBytes = new byte[2];
        memoryStream.Read(audioFormatBytes);
        var isPCM = audioFormatBytes[0] == 0x1 && audioFormatBytes[1] == 0x0;

        // NumChannels   Mono = 1, Stereo = 2
        var numChannelsBytes = new byte[2];
        memoryStream.Read(numChannelsBytes);
        var channels = (int)BitConverter.ToUInt16(numChannelsBytes);

        // SampleRate
        var sampleRateBytes = new byte[4];
        memoryStream.Read(sampleRateBytes);
        var sampleRate = BitConverter.ToInt32(sampleRateBytes);

        // ByteRate (=SampleRate * NumChannels * BitsPerSample/8)
        var byteRateBytes = new byte[4];
        memoryStream.Read(byteRateBytes);

        // BlockAlign (=NumChannels * BitsPerSample/8)
        var blockAlignBytes = new byte[2];
        memoryStream.Read(blockAlignBytes);

        // BitsPerSample
        var bitsPerSampleBytes = new byte[2];
        memoryStream.Read(bitsPerSampleBytes);
        var bitPerSample = BitConverter.ToUInt16(bitsPerSampleBytes);

        // Discard Extra Parameters
        if (subChunkSize > 16) memoryStream.Seek(subChunkSize - 16, SeekOrigin.Current);

        // Data
        var subChunkIDBytes = new byte[4];
        memoryStream.Read(subChunkIDBytes);

        // If fact exists, discard fact
        if (subChunkIDBytes[0] == 0x66 && subChunkIDBytes[1] == 0x61 && subChunkIDBytes[2] == 0x63 && subChunkIDBytes[3] == 0x74)
        {
            var factSizeBytes = new byte[4];
            memoryStream.Read(factSizeBytes);
            var factSize = BitConverter.ToInt32(factSizeBytes);
            memoryStream.Seek(factSize, SeekOrigin.Current);
            memoryStream.Read(subChunkIDBytes);
        }
        if (subChunkIDBytes[0] != 0x64 || subChunkIDBytes[1] != 0x61 || subChunkIDBytes[2] != 0x74 || subChunkIDBytes[3] != 0x61) throw new ArgumentException("fileBytes is not the correct Wav file format.");

        // dataSize (=NumSamples * NumChannels * BitsPerSample/8)
        var dataSizeBytes = new byte[4];
        memoryStream.Read(dataSizeBytes);
        var dataSize = BitConverter.ToInt32(dataSizeBytes);

        var data = new byte[dataSize];
        memoryStream.Read(data);

        return CreateAudioClip(data, channels, sampleRate, bitPerSample, "name");

        AudioClip CreateAudioClip(byte[] data, int channels, int sampleRate, UInt16 bitPerSample, string audioClipName)
        {
            var audioClipData = bitPerSample switch
            {
                8 => Create8BITAudioClipData(data),
                16 => Create16BITAudioClipData(data),
                32 => Create32BITAudioClipData(data),
                _ => throw new ArgumentException($"bitPerSample is not supported : bitPerSample = {bitPerSample}")
            };

            var audioClip = AudioClip.Create(audioClipName, audioClipData.Length, channels, sampleRate, false);
            audioClip.SetData(audioClipData, 0);
            return audioClip;

            float[] Create8BITAudioClipData(byte[] data)
            => data.Select((x, i) => (float)data[i] / sbyte.MaxValue).ToArray();

            float[] Create16BITAudioClipData(byte[] data)
            {
                var audioClipData = new float[data.Length / 2];
                var memoryStream = new MemoryStream(data);

                for (var i = 0; ; i++)
                {
                    var target = new byte[2];
                    var read = memoryStream.Read(target);

                    if (read <= 0) break;

                    audioClipData[i] = (float)BitConverter.ToInt16(target) / short.MaxValue;
                }

                return audioClipData;
            }

            float[] Create32BITAudioClipData(byte[] data)
            {
                var audioClipData = new float[data.Length / 4];
                var memoryStream = new MemoryStream(data);

                for (var i = 0; ; i++)
                {
                    var target = new byte[4];
                    var read = memoryStream.Read(target);

                    if (read <= 0) break;

                    //audioClipData[i] = (float)BitConverter.ToInt32(target) / int.MaxValue;
                    audioClipData[i] = (float)BitConverter.ToSingle(target, 0);// / int.MaxValue;

                }

                return audioClipData;
            }
        }

    }

}
