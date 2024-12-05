using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;

public class CopyPasteButtonWindow_1Script : CopyPasteButtonWindowCommonScript
{
    [SerializeField] private Image[] _buttonImages;
    [SerializeField] private AssetReference[] _iconReferences;
    [SerializeField] private TMP_InputField inputField;

    private (Sprite check, Sprite noCheck) _checkSheetSprite;
    private bool[] _onButtons = new bool[0];

    public enum Operation : int
    {
        OnCopyButton,
        OnCancelButton,
        OnAliasCopyButton,
        OffAliasCopyButton,
        OnAddNameTextButton,
        OffAddNameTextButton,
        OnAddAliasTextButton,
        OffAddAliasTextButton,
        OnAddMemorandumTextButton,
        OffAddMemorandumTextButton,
        OnAddHashtagTextButton,
        OffAddHashtagTextButton,
        InputDestinationFullPath,
    }

    private enum ButtonIndexEnum : int
    {
        AliasCopy = 0,
        AddNameText,
        AddAliasText,
        AddMemorandumText,
        AddHashtagText,
        CancelButton,
        CopyButton,

    }

    public void OnAliasCopyButton()
    {
        _onButtons[(int)ButtonIndexEnum.AliasCopy] = (_onButtons[(int)ButtonIndexEnum.AliasCopy]) ? false : true;

        _buttonImages[(int)ButtonIndexEnum.AliasCopy].sprite = (_onButtons[(int)ButtonIndexEnum.AliasCopy]) ? _checkSheetSprite.check : _checkSheetSprite.noCheck;

        if(_onButtons[(int)ButtonIndexEnum.AliasCopy])
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAliasCopyButton, null);
        }
        else
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OffAliasCopyButton, null);
        }
    }

    public void OnAddNameTextButton()
    {
        _onButtons[(int)ButtonIndexEnum.AddNameText] = (_onButtons[(int)ButtonIndexEnum.AddNameText]) ? false : true;

        _buttonImages[(int)ButtonIndexEnum.AddNameText].sprite = (_onButtons[(int)ButtonIndexEnum.AddNameText]) ? _checkSheetSprite.check : _checkSheetSprite.noCheck;

        if (_onButtons[(int)ButtonIndexEnum.AddNameText])
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAddNameTextButton, null);
        }
        else
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OffAddNameTextButton, null);
        }
    }

    public void OnAddAliasTextButton()
    {
        _onButtons[(int)ButtonIndexEnum.AddAliasText] = (_onButtons[(int)ButtonIndexEnum.AddAliasText]) ? false : true;

        _buttonImages[(int)ButtonIndexEnum.AddAliasText].sprite = (_onButtons[(int)ButtonIndexEnum.AddAliasText]) ? _checkSheetSprite.check : _checkSheetSprite.noCheck;

        if (_onButtons[(int)ButtonIndexEnum.AddAliasText])
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAddAliasTextButton, null);
        }
        else
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OffAddAliasTextButton, null);
        }
    }

    public void OnAddMemorandumTextButton()
    {
        _onButtons[(int)ButtonIndexEnum.AddMemorandumText] = (_onButtons[(int)ButtonIndexEnum.AddMemorandumText]) ? false : true;

        _buttonImages[(int)ButtonIndexEnum.AddMemorandumText].sprite = (_onButtons[(int)ButtonIndexEnum.AddMemorandumText]) ? _checkSheetSprite.check : _checkSheetSprite.noCheck;

        if (_onButtons[(int)ButtonIndexEnum.AddMemorandumText])
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAddMemorandumTextButton, null);
        }
        else
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OffAddMemorandumTextButton, null);
        }
    }

    public void OnAddHashtagTextButton()
    {
        _onButtons[(int)ButtonIndexEnum.AddHashtagText] = (_onButtons[(int)ButtonIndexEnum.AddHashtagText]) ? false : true;

        _buttonImages[(int)ButtonIndexEnum.AddHashtagText].sprite = (_onButtons[(int)ButtonIndexEnum.AddHashtagText]) ? _checkSheetSprite.check : _checkSheetSprite.noCheck;

        if (_onButtons[(int)ButtonIndexEnum.AddHashtagText])
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAddHashtagTextButton, null);
        }
        else
        {
            _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OffAddHashtagTextButton, null);
        }
    }

    public void OnCancelButton()
    {
        _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnCancelButton, null);
    }
    public void OnCopyButton()
    {

        _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnCopyButton, null);
    }

    public void OnEndEdit(string text)
    {
        _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.InputDestinationFullPath, text);
    }

    public override void Initialization(CursorInformationScript cursorInformationScript, CopyPasteButtonScript copyPasteButtonScript)
    {
        base.Initialization(cursorInformationScript, copyPasteButtonScript);

        _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.InputDestinationFullPath, inputField.text);

        if (_onButtons[(int)ButtonIndexEnum.AliasCopy]) _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAliasCopyButton, null);
        if (_onButtons[(int)ButtonIndexEnum.AddNameText]) _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAddNameTextButton, null);
        if (_onButtons[(int)ButtonIndexEnum.AddAliasText]) _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAddAliasTextButton, null);
        if (_onButtons[(int)ButtonIndexEnum.AddMemorandumText]) _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAddMemorandumTextButton, null);
        if (_onButtons[(int)ButtonIndexEnum.AddHashtagText]) _copyPasteButtonScript.ReceivingActionNotifications_Window1(Operation.OnAddHashtagTextButton, null);

    }

    private void Awake()
    {
        _onButtons = new bool[Enum.GetNames(typeof(ButtonIndexEnum)).Length];

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
        _checkSheetSprite.noCheck = iconSprites[0];
        _checkSheetSprite.check = iconSprites[1];

        //読み込みが終わり次第、反映させる。
        for (int i = 0; i < _buttonImages.Length; i++)
        {
            _buttonImages[i].sprite = (_onButtons[i]) ? _checkSheetSprite.check : _checkSheetSprite.noCheck;
        }
    }
}