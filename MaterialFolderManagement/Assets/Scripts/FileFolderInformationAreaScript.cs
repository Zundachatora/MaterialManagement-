using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FileFolderInformationAreaScript : ScrollController<bool, InformationCommonUI>
{
    //���O�A�ʖ��A�n�b�V���^�O�A���Y�^�A���̍Đ�(.wav�̂�)

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
    /// ���O�̕ύX��ShowingGrantDatasSave_Renaming���g������
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
        //�A�C�R���̏���
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

        //���o�̃I�u�W�F�N�g�ɔ��f
        DisplayObjectAccess(
            (InformationCommonUI displayObject, int index, bool opening) =>
            {
                displayObject.SetIcons(_iconSprites);
            });

    }

    private void SetNewInformation(FileFoldersGrantData[] grantDatas)
    {
        if ((grantDatas == null) || (grantDatas.Length == 0))
        {//�I�����Ă���t�@�C���t�H���_���Ȃ���
            SelectGrantData = new FileFoldersGrantData[0];

            ScrollClear();

            return;
        }

        SelectGrantData = grantDatas;

        //�X�N���[���̏���
        List<(bool opening, int displayGObjectIndex, float verticalSize)> scrollDatas =
            new List<(bool opening, int displayGObjectIndex, float verticalSize)>
            {//���ʁA�S�Ẵt�@�C���t�H���_�ŕ\������B
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
