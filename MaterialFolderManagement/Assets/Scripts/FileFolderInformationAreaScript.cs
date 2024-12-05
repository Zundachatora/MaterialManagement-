using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FileFolderInformationAreaScript : ScrollController<bool, InformationCommonUI>
{
    //名前、別名、ハッシュタグ、備忘録、音の再生(.wavのみ)

    public event FileFolderUpdatedDelegate FileFolderUpdatedEvent = delegate { };
    public delegate void FileFolderUpdatedDelegate();
    public FileFoldersGrantData[] SelectGrantData { get; private set; }= new FileFoldersGrantData[0];

    //[SerializeField] private float _scrollElementSize_Folded = 57;
    [SerializeField] private RectTransform[] _scrollInformationObjects = new RectTransform[0];
    [SerializeField] private FileFolderDisplayAreaScript _fileFolderDisplayAreaScript;
    [SerializeField] private AssetReference _expandIconReferences;
    [SerializeField] private AssetReference _foldIconReferences;
    [SerializeField] private LoadingAndSavingFileFoldersScript _loadingAndSavingFileFoldersScript;

    private (Sprite expand, Sprite fold) _iconSprites = new(null, null);

    /// <summary>
    /// 名前の変更はShowingGrantDatasSave_Renamingを使うこと
    /// </summary>
    public void ShowingGrantDatasSave()
    {
        _loadingAndSavingFileFoldersScript.Save(SelectGrantData);
    }

    public bool ShowingGrantDatasSave_Renaming(string newName)
    {
        if (SelectGrantData.Length == 1)
        {
            if (_loadingAndSavingFileFoldersScript.FileFolderRenaming(ref SelectGrantData[0], newName))
            {
                FileFolderUpdatedEvent();

                return true;
            }
        }
        return false;
    }

    public void ScrollElementResize(int index,float size)
    {
        VerticalSizeResize(index,size);
    }

    public bool WavPlay(out float playbackTime)
    {
        playbackTime = 0;

        if (!(SelectGrantData.Length > 0)) return false;
        if(!(SelectGrantData[0].Extension == FileFoldersGrantData.extension.wav))return false;

        if(!_loadingAndSavingFileFoldersScript.WavPlay(SelectGrantData[0].UserPath,out playbackTime))
        {
            return false;
        }

        return true;
    }

    public void WavStop()
    {
        _loadingAndSavingFileFoldersScript.WavStop();
    }

    private void Awake()
    {
        _fileFolderDisplayAreaScript.SelectDisplayObjectHandle += SetNewInformation;

        foreach (var item in _scrollInformationObjects)
        {
            item.gameObject.SetActive(false);
        }
    }

    private async void Start()
    {
        //アイコンの準備
        AsyncOperationHandle[] handles = new AsyncOperationHandle[2];
        handles[0] = Addressables.LoadAssetAsync<Sprite>(_expandIconReferences);
        handles[1] = Addressables.LoadAssetAsync<Sprite>(_foldIconReferences);
        foreach (var handle in handles)
        {
            await handle.Task;
        }
        if ((handles[0].Status == AsyncOperationStatus.Succeeded) &&
            (handles[1].Status == AsyncOperationStatus.Succeeded))
        {
            _iconSprites.expand = (Sprite)handles[0].Result;
            _iconSprites.fold = (Sprite)handles[1].Result;
        }

        //既出のオブジェクトに反映
        DisplayObjectAccess(
            (InformationCommonUI displayObject, int index, bool opening) =>
            {
                displayObject.SetIcons(_iconSprites);
            });

    }

    private void SetNewInformation(FileFoldersGrantData[] grantDatas)
    {
        if ((grantDatas == null) || (grantDatas.Length == 0))
        {//選択しているファイルフォルダがない時
            SelectGrantData = new FileFoldersGrantData[0];

            ScrollClear();

            return;
        }

        SelectGrantData = grantDatas;

        //スクロールの準備
        List<(bool opening, int displayGObjectIndex, float verticalSize)> scrollDatas =
            new List<(bool opening, int displayGObjectIndex, float verticalSize)>
            {//共通、全てのファイルフォルダで表示する。
                (true, 0, 0),
                (true, 1, 0),
                (true, 2, 0),
                (true, 3, 0),
            };
        if (grantDatas[0].Extension == FileFoldersGrantData.extension.wav)
        {
            scrollDatas.Add((true, 4, 0));
        }

        SetScrollData(
            scrollDatas.ToArray(),
            _scrollInformationObjects,
            (InformationCommonUI displayObject, int index, ref bool opening) =>
            {
                displayObject.InitialSetting(opening,index, _iconSprites);

                return displayObject;
            },
            (InformationCommonUI displayObject, int index, ref bool opening) =>
            {

                opening = displayObject.Opening;

                return displayObject;
            });
    }


}
