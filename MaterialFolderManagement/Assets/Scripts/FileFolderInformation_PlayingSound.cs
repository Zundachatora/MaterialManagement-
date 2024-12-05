using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class FileFolderInformation_PlayingSound : InformationCommonUI
{
    //UniTaskの参考 https://qiita.com/toRisouP/items/60673e4a39319e69fbc0
    
    [SerializeField] private float _lineSize = 56;
    [SerializeField] private GameObject _gOWithPlayOrStopButtonImage;
    [SerializeField] private Sprite _musicPlayIcon;
    [SerializeField] private Sprite _musicStopIcon;

    private Image _playOrStopImage;
    private bool _isPlaying = false;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public override void InitialSetting(bool opening, int scrollIndex, (Sprite expand, Sprite fold) icons)
    {
        base.InitialSetting(opening, scrollIndex, icons);

        SetContentBackgroundSize_Expanded(_lineSize * 2);
    }

    public void WavPlayOrStopButton()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();

        PlayOrStop(_cancellationTokenSource.Token);
        
        async void PlayOrStop(CancellationToken token)
        {

            if (_isPlaying)
            {
                Stop();
            }
            else
            {
                if (_fileFolderInformationAreaScript.WavPlay(out float playbackTime))
                {
                    Play();

                    //※ボタンは連打するので出来れば例外処理を使わずにキャンセル処理をする//後で修正
                    try
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(playbackTime), cancellationToken: token);

                        await UniTask.SwitchToMainThread(token);
                    }
                    catch(OperationCanceledException)
                    {
                        return;
                    }
                    
                    Stop();
                }
                else
                {
                    return;
                }
            }

            void Play()
            {
                _isPlaying = true;

                _playOrStopImage.sprite = _musicStopIcon;
            }

            void Stop()
            {
                _isPlaying = false;

                _playOrStopImage.sprite = _musicPlayIcon;

                _fileFolderInformationAreaScript.WavStop();
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _playOrStopImage = _gOWithPlayOrStopButtonImage.GetComponent<Image>();
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}
