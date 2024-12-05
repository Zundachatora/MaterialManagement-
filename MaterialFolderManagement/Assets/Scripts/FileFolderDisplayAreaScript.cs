using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.InputSystem;

public class FileFolderDisplayAreaScript : ScrollController<FileFoldersGrantData, FileFolderButtonsScript>
{
    public event SelectDisplayObject SelectDisplayObjectHandle =delegate { };
    public delegate void SelectDisplayObject(FileFoldersGrantData[] grantDatas);

    [SerializeField] private RectTransform _scrollItem;
    [SerializeField] private float _scrollItemeSize = 55;
    [SerializeField] private AssetReference[] _iconReferences;
    [SerializeField] private LoadingAndSavingFileFoldersScript _lAndSFFScript;
    [SerializeField] private CursorInformationScript _cursorInformation;
    [SerializeField] private RectTransform _cursorRegionRT;
    [SerializeField] private FileFolderInformationAreaScript _fileFolderInformationAreaScript;

    private List<FileFoldersGrantData> _selectDisplayObjectHandleArgument = new List<FileFoldersGrantData>();
    private Sprite[] _iconSprits = null;
    private KeyconFigInputs _keyconFigInputs;
    private FileFolderDisplayAreaScript _fileFolderDisplayAreaScript;
    private bool _onLeftCtrl = false;
    private bool _onLeftShift = false;
    //private bool _onAKye = false;
    private int? _previousIndex = null;

    public void AreaButtonClick(int index, bool doubleClick, bool lockButton)
    {
        if (!(index >= 0) || !(index < _lAndSFFScript.SelectFileFolderGrantDatas.Length)) return;

        if (lockButton)
        {
            LockClick();
        }
        else
        {

            MainClick();

            _previousIndex = index;

            SelectDisplayObjectHandle(_selectDisplayObjectHandleArgument.ToArray());

        }

        void MainClick()
        {
            if (doubleClick)
            {
                if (_lAndSFFScript.SelectFileFolderGrantDatas[index].Extension == FileFoldersGrantData.extension.folder)
                {
                    _lAndSFFScript.SelectFolder(_lAndSFFScript.SelectFileFolderGrantDatas[index].UserPath);
                }
            }
            else
            {

                if (_onLeftShift)
                {
                    if (_previousIndex != null)
                    {
                        RangeSelection((int)_previousIndex, index, true);
                    }
                }
                else if (_onLeftCtrl)
                {
                    if (_lAndSFFScript.SelectFileFolderGrantDatas[index].IsSelected)
                    {
                        Selection(index, false, false);
                    }
                    else
                    {
                        Selection(index, true, false);
                    }
                }
                else
                {
                    Selection(index);
                }
            }
        }
        void LockClick()
        {
            if (doubleClick) return;

            if (_lAndSFFScript.SelectFileFolderGrantDatas[index].IsLocked)
            {
                LockSelection(index, false);
            }
            else
            {
                LockSelection(index, true);
            }
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    private void Awake()
    {
        _fileFolderInformationAreaScript.FileFolderUpdatedEvent += DisplayObjectUpdate;

        _scrollItem.gameObject.SetActive(false);

        _cursorInformation.RegionSet(_cursorRegionRT, CursorInformationScript.region.fileFolderDisplayArea, 0);

        _fileFolderDisplayAreaScript = GetComponent<FileFolderDisplayAreaScript>();

        //キー入力受け取り準備
        _keyconFigInputs = new KeyconFigInputs();
        _keyconFigInputs.Main.LeftCtrl_Standard.performed += OnLeftCtrl;
        _keyconFigInputs.Main.LeftShift_Standard.performed += OnLeftShift;
        _keyconFigInputs.Main.AKye_Standard.performed += OnAKye;
        _keyconFigInputs.Main.LeftCtrl_Standard.canceled += OffLeftCtrl;
        _keyconFigInputs.Main.LeftShift_Standard.canceled += OffLeftShift;
        _keyconFigInputs.Main.AKye_Standard.canceled += OffAKye;
        _keyconFigInputs.Enable();

        _lAndSFFScript.OnFolderSelected += ScrollDataChange;
    }

    private async void Start()
    {
        //アイコン画像をロードする。
        AsyncOperationHandle handle;
        List<Sprite> iconSprites = new List<Sprite>();
        foreach (var reference in _iconReferences)
        {
            handle = Addressables.LoadAssetAsync<Sprite>(reference);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                iconSprites.Add((Sprite)handle.Result);
            }
        }
        _iconSprits = iconSprites.ToArray();

        //アイコンの準備が出来たので既に表示してしまっているアイコンを更新していく。
        DisplayObjectAccess((FileFolderButtonsScript displayObject, int index, FileFoldersGrantData data) =>
        {
            if (data.Extension == FileFoldersGrantData.extension.folder)
            {
                displayObject.IconSetting(_iconSprits[0]);
            }
            else
            {
                displayObject.IconSetting(_iconSprits[1]);
            }
        });
    }

    private void DisplayObjectUpdate()
    {
        //アイコンの準備が出来たので既に表示してしまっているアイコンを更新していく。
        DisplayObjectAccess((FileFolderButtonsScript displayObject, int index, FileFoldersGrantData data) =>
        {
            displayObject.InitialSetting(data, index, _fileFolderDisplayAreaScript);
        });
    }

    private void ScrollDataChange()
    {
        //選択解除を伝える
        _selectDisplayObjectHandleArgument.Clear();
        SelectDisplayObjectHandle(_selectDisplayObjectHandleArgument.ToArray());

        //引数配列の準備
        (FileFoldersGrantData data, int displayGObjectIndex, float verticalSize)[] tmpArgument = new (FileFoldersGrantData data, int displayGObjectIndex, float verticalSize)[_lAndSFFScript.SelectFileFolderGrantDatas.Length];
        for (int i = 0; i < tmpArgument.Length; i++)
        {
            tmpArgument[i] = (
                _lAndSFFScript.SelectFileFolderGrantDatas[i],
                0,
                _scrollItemeSize);
        }
        //スクロールの準備(初期化)
        SetScrollData(
            tmpArgument,
            new RectTransform[] { _scrollItem },
            (FileFolderButtonsScript displayObject, int index, ref FileFoldersGrantData data) =>
            {
                displayObject.InitialSetting(data, index, _fileFolderDisplayAreaScript);

                if (_iconSprits != null)
                {
                    if (data.Extension == FileFoldersGrantData.extension.folder)
                    {
                        displayObject.IconSetting(_iconSprits[0]);
                    }
                    else
                    {
                        displayObject.IconSetting(_iconSprits[1]);
                    }
                }

                return displayObject;
            });
        //アイコン
        if (_iconSprits != null)
        {
            DisplayObjectAccess((FileFolderButtonsScript displayObject, int index, FileFoldersGrantData data) =>
            {
                if (data.Extension == FileFoldersGrantData.extension.folder)
                {
                    displayObject.IconSetting(_iconSprits[0]);
                }
                else
                {
                    displayObject.IconSetting(_iconSprits[1]);
                }
            });
        }
    }
    private void RangeSelection(int startIndex,int endIndex, bool select = true)
    {
        if (!(startIndex >= 0) || !(startIndex < _lAndSFFScript.SelectFileFolderGrantDatas.Length)) return;
        if (!(endIndex >= 0) || !(endIndex < _lAndSFFScript.SelectFileFolderGrantDatas.Length)) return;

        //小から大にかけてindexを操作するため
        var nums = GetMaxMin(startIndex,endIndex);
        startIndex = nums.min;
        endIndex = nums.max;

        //一旦全てを選択解除させるために関数を使いまわす。
        Selection(endIndex,true,true);

        //GrantData 選択
        for (int i = startIndex; i < endIndex; i++)
        {
            if(_lAndSFFScript.SelectFileFolderGrantDatas[i].IsLocked == false)
            {
                _lAndSFFScript.SelectFileFolderGrantDatas[i].IsSelected = select;

                if(select)
                {
                    _selectDisplayObjectHandleArgument.Add(_lAndSFFScript.SelectFileFolderGrantDatas[i]);
                }
                else
                {
                    _selectDisplayObjectHandleArgument.Remove(_lAndSFFScript.SelectFileFolderGrantDatas[i]);
                }
            }
        }

        //ボタン
        DisplayObjectAccess((FileFolderButtonsScript displayObject, int index, FileFoldersGrantData data) =>
        {
            if (!(index < endIndex)) return;
            if (!(index >= startIndex)) return;

            displayObject.IsSelected(select);
        });
        
        (int max,int min)GetMaxMin(int x,int y)
        {
            return (x >= y) ? (x, y) : (y, x);
        }
    }

    private void Selection(int selectIndex,bool select=true, bool deselectOthers=true)
    {
        if (!(selectIndex >= 0) || !(selectIndex < _lAndSFFScript.SelectFileFolderGrantDatas.Length)) return;

        //全て選択解除
        if (deselectOthers)
        {
            foreach (var sFFGD in _lAndSFFScript.SelectFileFolderGrantDatas)
            {
                sFFGD.IsSelected = false;
            }

            //ボタン(表示アイテム)
            DisplayObjectAccess((FileFolderButtonsScript displayObject, int index, FileFoldersGrantData data) =>
            {
                displayObject.IsSelected(false);
            });

            _selectDisplayObjectHandleArgument.Clear();
        }

        //選択
        if (!_lAndSFFScript.SelectFileFolderGrantDatas[selectIndex].IsLocked)
        {
            _lAndSFFScript.SelectFileFolderGrantDatas[selectIndex].IsSelected = select;

            if(select)
            {
                _selectDisplayObjectHandleArgument.Add(_lAndSFFScript.SelectFileFolderGrantDatas[selectIndex]);
            }
            else
            {
                _selectDisplayObjectHandleArgument.Remove(_lAndSFFScript.SelectFileFolderGrantDatas[selectIndex]);
            }

            //ボタン(表示アイテム)
            DisplayObjectAccess((FileFolderButtonsScript displayObject, int index, FileFoldersGrantData data) =>
            {
                if (index == selectIndex) displayObject.IsSelected(select);
            });
            
        }
    }

    private void LockSelection(int selectIndex, bool select = true)
    {
        if (!(selectIndex >= 0) || !(selectIndex < _lAndSFFScript.SelectFileFolderGrantDatas.Length)) return;

        _lAndSFFScript.SelectFileFolderGrantDatas[selectIndex].IsLocked = select;

        //仕様、ロックするとファイルフォルダは選択出来なくなる。ロックする前に選択していても解除される。
        if (select)
        {
            _lAndSFFScript.SelectFileFolderGrantDatas[selectIndex].IsSelected = false;
        }

        //ボタン(表示アイテム)
        DisplayObjectAccess((FileFolderButtonsScript displayObject, int index, FileFoldersGrantData data) =>
        {
            if (index == selectIndex)
            {
                displayObject.IsLocked(select);
                displayObject.IsSelected(false);
            }
        });


    }

    private void OnLeftCtrl(InputAction.CallbackContext context)
    {
        _onLeftCtrl = true;
    }

    private void OffLeftCtrl(InputAction.CallbackContext context)
    {
        _onLeftCtrl = false;
    }

    private void OnLeftShift(InputAction.CallbackContext context)
    {
        _onLeftShift = true;
    }

    private void OffLeftShift(InputAction.CallbackContext context)
    {
        _onLeftShift = false;
    }

    private void OnAKye(InputAction.CallbackContext context)
    {
        //_onAKye = true;

        if (_onLeftCtrl && (_cursorInformation.LastClickLocation == CursorInformationScript.region.fileFolderDisplayArea))
        {
            RangeSelection(0,(_lAndSFFScript.SelectFileFolderGrantDatas.Length-1));
        }

    }

    private void OffAKye(InputAction.CallbackContext context)
    {
        //_onAKye = false;
    }

    private void OnDestroy()
    {
        _keyconFigInputs.Dispose();
    }
}
