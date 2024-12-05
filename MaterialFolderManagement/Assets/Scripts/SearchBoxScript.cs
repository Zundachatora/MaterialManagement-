using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class SearchBoxScript : MonoBehaviour
{
    public event CloseTheSearchBoxHandle CloseTheSearchBox = delegate { };
    public delegate void CloseTheSearchBoxHandle();

    [SerializeField] private RectTransform _canvasRT;
    [SerializeField] private GameObject _searchBoxMenuPrefab;
    [SerializeField] private GameObject _gameObjectWithCursorInformationScript;
    [SerializeField] private GameObject _gameObjectWithLoadingAndSavingFileFoldersScript;

    private GameObject _searchBoxMenuInstance;
    private KeyconFigInputs _keyconFigInputs;
    private TMP_InputField _inputField;
    private bool _isSearchInput = false;
    private bool _onEnter = false;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private CursorInformationScript _cursorInformationScript;
    private LoadingAndSavingFileFoldersScript _loadingAndSavingFileFoldersScript;
    private SearchConditionData searchConditionData = new SearchConditionData();

    public enum ButtonNameEnum
    {
        others,
        exclusionOrInclusion,
        ascendingOrDescending,
        IncludeName,
        IncludeFile,
        IncludeFolder,
        IncludeAlias,
        IncludeMemorandum,
        IncludeHashtag,
    }

    public void OnSearchBoxMenuButton(SearchBoxButtonCommon searchBoxButtonCommon)
    {
        switch (searchBoxButtonCommon.ButtonNameEnum)
        {
            case ButtonNameEnum.others:
                break;

            case ButtonNameEnum.exclusionOrInclusion:
                searchConditionData.ExcludeMatchingConditions = searchBoxButtonCommon.CheckSheetOn;
                break;

            case ButtonNameEnum.ascendingOrDescending:
                searchConditionData.AscendingOrder = !searchBoxButtonCommon.CheckSheetOn;
                break;

            case ButtonNameEnum.IncludeName:
                searchConditionData.IncludeName = searchBoxButtonCommon.CheckSheetOn;
                break;

            case ButtonNameEnum.IncludeFile:
                searchConditionData.IncludeFile = searchBoxButtonCommon.CheckSheetOn;
                break;

            case ButtonNameEnum.IncludeFolder:
                searchConditionData.IncludeFolder = searchBoxButtonCommon.CheckSheetOn;
                break;

            case ButtonNameEnum.IncludeAlias:
                searchConditionData.IncludeAlias = searchBoxButtonCommon.CheckSheetOn;
                break;

            case ButtonNameEnum.IncludeMemorandum:
                searchConditionData.IncludeMemorandum = searchBoxButtonCommon.CheckSheetOn;
                break;

            case ButtonNameEnum.IncludeHashtag:
                if(searchBoxButtonCommon.CheckSheetOn)
                {
                    searchConditionData.IncludeHashtag = searchBoxButtonCommon.SearchStrings;
                }
                else
                {
                    searchConditionData.IncludeHashtag = new string[0];
                }
                break;

            default:
                break;
        }

        
    }


    public void CloseSearchBox()
    {
        _isSearchInput = false;

        CloseTheSearchBox();

        _searchBoxMenuInstance.SetActive(false);

        //イベント解除
        _cursorInformationScript.OnClickEvent -= OnClick;
    }

    public void OpenSearchBox()
    {
        _isSearchInput = true;

        if (_searchBoxMenuInstance == null)
        {
            _searchBoxMenuInstance = Instantiate(_searchBoxMenuPrefab, _canvasRT);

            _searchBoxMenuInstance.GetComponent<SearchBoxMenuScript>().Initialization( _cursorInformationScript, GetComponent<SearchBoxScript>());
        }

        _searchBoxMenuInstance.SetActive(true);

        //イベント登録
        _cursorInformationScript.OnClickEvent += OnClick;
    }

    public async void OnEndEdit(string text)
    {

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.3f));

        await CloseAndSearch(_cancellationTokenSource.Token);

        async UniTask CloseAndSearch(CancellationToken token)
        {
            bool isClose = false;

            try
            {
                await InputInTime(token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if(isClose)
            {
                await UniTask.SwitchToMainThread();

                searchConditionData.SearchString = text;
                _loadingAndSavingFileFoldersScript.SelectFolder(_loadingAndSavingFileFoldersScript.SelectFolderUserPath, searchConditionData);

                CloseSearchBox();
            }

            async UniTask InputInTime(CancellationToken token)
            {
                while (true)
                {
                    if (_onEnter)
                    {
                        isClose = true;
                    }

                    try
                    {
                        await UniTask.Yield(token);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            }
        }
    }

    public void OnSelect(string text)
    {
        _inputField.caretPosition = _inputField.text.Length;

        OpenSearchBox();
    }

    public void OnValueChanged(string text)
    {
        if(!_isSearchInput) OpenSearchBox();
    }

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
        _cursorInformationScript = _gameObjectWithCursorInformationScript.GetComponent<CursorInformationScript>();
        _loadingAndSavingFileFoldersScript= _gameObjectWithLoadingAndSavingFileFoldersScript.GetComponent<LoadingAndSavingFileFoldersScript>();

        //キー入力受け取り準備
        _keyconFigInputs = new KeyconFigInputs();
        _keyconFigInputs.Main.Enter_Standard.performed += OnEnter;
        _keyconFigInputs.Main.Enter_Standard.canceled += OffEnter;
        _keyconFigInputs.Enable();

        
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        _keyconFigInputs.Dispose();

        //イベント解除
        _cursorInformationScript.OnClickEvent -= OnClick;

    }

    private void OnClick()
    {
        if (_cursorInformationScript.LastClickLocation == CursorInformationScript.region.searchBoxMenu) return;
        
        CloseSearchBox();
    }

    private void OnEnter(InputAction.CallbackContext context)
    {
        _onEnter = true;
    }

    private void OffEnter(InputAction.CallbackContext context)
    {
        _onEnter = false;
    }
}
