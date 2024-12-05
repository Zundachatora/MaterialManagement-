using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPasteButtonScript : MonoBehaviour
{
    [SerializeField] private FileFolderDisplayAreaScript _fileFolderDisplayAreaScript;
    [SerializeField] private RectTransform _canvasRT;
    [SerializeField] private CursorInformationScript _cursorInformationScript;
    [SerializeField] private GameObject[] _copyPasteWindowPrefabs = new GameObject[0];
    [SerializeField] private LoadingAndSavingFileFoldersScript _loadingAndSavingFileFoldersScript;

    private CopyPasteButtonWindowCommonScript[] _copyPasteWindows = new CopyPasteButtonWindowCommonScript[0];
    private FileFoldersGrantData[] _copyGrantDatas = new FileFoldersGrantData[0];
    private int _currentCopyGrantDatasIndex = 0;
    private bool _buttonOn = false;
    private CopyPasteButtonWindowEnum _currentWindowsEnum = CopyPasteButtonWindowEnum.window1;
    private MoveFileFolderScript.MoveFileFolderOption _moveFileFolderOption = new MoveFileFolderScript.MoveFileFolderOption();
    private string _destinationFullPath;
    private DateTime _copyStartTime;

    private enum CopyPasteButtonWindowEnum:int
    {
        window1 = 0,
        window2,
        window3,
    }

    
    public void OnButton()
    {
        _buttonOn = (_buttonOn) ? false : true;

        if (_buttonOn)
        {
            if (_currentWindowsEnum == CopyPasteButtonWindowEnum.window1)
            {
                _moveFileFolderOption = new MoveFileFolderScript.MoveFileFolderOption();
                _currentCopyGrantDatasIndex = 0;

                if (_copyGrantDatas.Length == 0)
                {
                    AllCloseWindows();
                    return;
                }

                OpenWindows(_currentWindowsEnum, _copyGrantDatas[_currentCopyGrantDatasIndex],null);
            }
        }
        else
        {
            AllCloseWindows();
        }
    }

    public void ReceivingActionNotifications_Window1(CopyPasteButtonWindow_1Script.Operation operation, string destinationFullPath)
    {
        switch (operation)
        {
            case CopyPasteButtonWindow_1Script.Operation.OnCopyButton:
                _moveFileFolderOption.Copy = true;
                CloseWindows(CopyPasteButtonWindowEnum.window1);
                _currentCopyGrantDatasIndex = 0;
                _copyStartTime = DateTime.Now;
                //コピーする元のファイルフォルダがなければ終了する。
                if ((_copyGrantDatas==null)||(_copyGrantDatas.Length == 0))
                {
                    AllCloseWindows();
                    break;
                }

                Copying();
                
                break;
            case CopyPasteButtonWindow_1Script.Operation.OnCancelButton:
                CloseWindows(CopyPasteButtonWindowEnum.window1);
                break;
            case CopyPasteButtonWindow_1Script.Operation.OnAliasCopyButton:
                _moveFileFolderOption.Alias = true;
                break;
            case CopyPasteButtonWindow_1Script.Operation.OffAliasCopyButton:
                _moveFileFolderOption.Alias = false;
                break;
            case CopyPasteButtonWindow_1Script.Operation.OnAddNameTextButton:
                _moveFileFolderOption.AddNameText = true;
                break;
            case CopyPasteButtonWindow_1Script.Operation.OffAddNameTextButton:
                _moveFileFolderOption.AddNameText = false;
                break;
            case CopyPasteButtonWindow_1Script.Operation.OnAddAliasTextButton:
                _moveFileFolderOption.AddAliasText = true;
                break;
            case CopyPasteButtonWindow_1Script.Operation.OffAddAliasTextButton:
                _moveFileFolderOption.AddAliasText = false;
                break;
            case CopyPasteButtonWindow_1Script.Operation.OnAddMemorandumTextButton:
                _moveFileFolderOption.AddMemorandumText = true;
                break;
            case CopyPasteButtonWindow_1Script.Operation.OffAddMemorandumTextButton:
                _moveFileFolderOption.AddMemorandumText = false;
                break;
            case CopyPasteButtonWindow_1Script.Operation.OnAddHashtagTextButton:
                _moveFileFolderOption.AddHashtagText = true;
                break;
            case CopyPasteButtonWindow_1Script.Operation.OffAddHashtagTextButton:
                _moveFileFolderOption.AddHashtagText = false;
                break;
            case CopyPasteButtonWindow_1Script.Operation.InputDestinationFullPath:
                _destinationFullPath = destinationFullPath;
                break;
        }
    }

    public void ReceivingActionNotifications_Window2(CopyPasteButtonWindow_2Script.Operation operation)
    {

        switch (operation)
        {
            case CopyPasteButtonWindow_2Script.Operation.OnCompletionButton:
                AllCloseWindows();
                break;
        }
    }

    public void ReceivingActionNotifications_Window3(CopyPasteButtonWindow_3Script.Operation operation, string newName)
    {

        switch (operation)
        {
            case CopyPasteButtonWindow_3Script.Operation.OnCancelButton:
                AllCloseWindows();
                break;
            case CopyPasteButtonWindow_3Script.Operation.OnOverwriteButton:
                _moveFileFolderOption.FileFolderOverwrite = true;
                Copying();
                break;
            case CopyPasteButtonWindow_3Script.Operation.OnRenameRetryButton:
                CloseWindows(CopyPasteButtonWindowEnum.window3);
                Copying();
                break;
            case CopyPasteButtonWindow_3Script.Operation.InputNewName:
                _moveFileFolderOption.NewFileFolderName = newName;
                break;
        }
    }

    private void Copying()
    {
        bool loop = true;
        while (loop)
        {
            //全てのGrantDatasのコピーが完了したら。
            if (_currentCopyGrantDatasIndex >= _copyGrantDatas.Length)
            {
                OpenWindows(CopyPasteButtonWindowEnum.window2,null,null);
                loop = false;
                break;
            }

            var result = _loadingAndSavingFileFoldersScript.MoveFileFolder(_copyGrantDatas[_currentCopyGrantDatasIndex], _destinationFullPath, _copyStartTime, _moveFileFolderOption);

            //一つ以降はこれまでの設定通りに処理する。
            _moveFileFolderOption.NewFileFolderName = null;
            _moveFileFolderOption.FileFolderOverwrite= false;

            switch (result.reasonsForFailureEnum)
            {
                case MoveFileFolderScript.ReasonsForFailureEnum.FileFolderDoesNotExist:
                case MoveFileFolderScript.ReasonsForFailureEnum.InvalidUserPath:
                case MoveFileFolderScript.ReasonsForFailureEnum.InvalidDestinationFullPath:
                case MoveFileFolderScript.ReasonsForFailureEnum.NameIsEmpty:
                case MoveFileFolderScript.ReasonsForFailureEnum.AliasIsEmpty:
                    loop = false;
                    AllCloseWindows();
                    continue;
                case MoveFileFolderScript.ReasonsForFailureEnum.DuplicateNameAtDestination:
                case MoveFileFolderScript.ReasonsForFailureEnum.SubfileFoldersHaveDuplicateNames:
                case MoveFileFolderScript.ReasonsForFailureEnum.NewNameIsInvalid:
                    loop = false;
                    OpenWindows(CopyPasteButtonWindowEnum.window3, _copyGrantDatas[_currentCopyGrantDatasIndex], result.nameSuggestion);
                    continue;
                case MoveFileFolderScript.ReasonsForFailureEnum.Success:
                    _currentCopyGrantDatasIndex++;
                    break;
            }
        }
    }

    private void Awake()
    {
        _fileFolderDisplayAreaScript.SelectDisplayObjectHandle += GetCopyFileFolders;

        _copyPasteWindows = new CopyPasteButtonWindowCommonScript[_copyPasteWindowPrefabs.Length];
    }

    private void OpenWindows(CopyPasteButtonWindowEnum windowEnum,FileFoldersGrantData grantData,string suggestion)
    {
        if (((int)windowEnum < 0) || ((int)windowEnum >= _copyPasteWindowPrefabs.Length)) return;

        if (_copyPasteWindows[(int)windowEnum] == null)
        {
            _copyPasteWindows[(int)windowEnum] = Instantiate(_copyPasteWindowPrefabs[(int)windowEnum], _canvasRT).GetComponent<CopyPasteButtonWindowCommonScript>();
        }

        if(CopyPasteButtonWindowEnum.window3 == windowEnum)
        {
            _copyPasteWindows[(int)windowEnum].gameObject.GetComponent<CopyPasteButtonWindow_3Script>().Initialization(_cursorInformationScript, GetComponent<CopyPasteButtonScript>(), grantData, suggestion);
        }
        else
        {
            _copyPasteWindows[(int)windowEnum].Initialization(_cursorInformationScript, GetComponent<CopyPasteButtonScript>());
        }

        _copyPasteWindows[(int)windowEnum].gameObject.SetActive(true);

        _currentWindowsEnum = windowEnum;
    }

    private void AllCloseWindows()
    {
        for (int i = 0; i < _copyPasteWindows.Length; i++)
        {
            if (_copyPasteWindows[i] != null)
            {
                _copyPasteWindows[i].Close();
                _copyPasteWindows[i].gameObject.SetActive(false);
            }
        }

        _currentWindowsEnum = CopyPasteButtonWindowEnum.window1;

        _buttonOn = false;
    }
    private void CloseWindows(CopyPasteButtonWindowEnum windowEnum)
    {
        if (_copyPasteWindows[(int)windowEnum] != null)
        {
            _copyPasteWindows[(int)windowEnum].Close();
            _copyPasteWindows[(int)windowEnum].gameObject.SetActive(false);
        }

        _currentWindowsEnum = CopyPasteButtonWindowEnum.window1;
    }

    private void GetCopyFileFolders(FileFoldersGrantData[] grantDatas)
    {
        _copyGrantDatas = grantDatas;

        //コピー作業中に選択しているファイルフォルダが変更されたらキャンセルする。
        AllCloseWindows();
        _currentCopyGrantDatasIndex = 0;
    }
}
