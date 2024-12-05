using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileFolderInformation_Memorandum : InformationCommonUI
{
    [SerializeField] private float _lineSize = 56;
    [SerializeField] private GameObject _inputField;

    private TMP_InputField _tMP_InputField;

    public void OnValueChanged(string text)
    {
        SetContentBackgroundSize_Expanded(_lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1));
    }

    public void OnSelect(string text)
    {
        SetContentBackgroundSize_Expanded(_lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1));

        _tMP_InputField.caretPosition = _tMP_InputField.text.Length;
    }

    public void OnEndEdit(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            return;
        }

        foreach (var grantData in _fileFolderInformationAreaScript.SelectGrantData)
        {
            grantData.Memorandum = text;
            grantData.IsUpdated = true;
        }

        _fileFolderInformationAreaScript.ShowingGrantDatasSave();

        //入力フィールドの表示
        if (_fileFolderInformationAreaScript.SelectGrantData.Length == 0)
        {
            _tMP_InputField.SetTextWithoutNotify("");
        }
        else
        {
            _tMP_InputField.SetTextWithoutNotify(_fileFolderInformationAreaScript.SelectGrantData[0].Memorandum);
        }
    }


    public override void InitialSetting(bool opening, int scrollIndex, (Sprite expand, Sprite fold) icons)
    {
        base.InitialSetting(opening, scrollIndex, icons);

        //入力フィールドの表示
        if (_fileFolderInformationAreaScript.SelectGrantData.Length == 0)
        {
            _tMP_InputField.SetTextWithoutNotify("");
        }
        else if (_fileFolderInformationAreaScript.SelectGrantData.Length == 1)
        {
            _tMP_InputField.SetTextWithoutNotify(_fileFolderInformationAreaScript.SelectGrantData[0].Memorandum);
        }
        else
        {
            _tMP_InputField.SetTextWithoutNotify("-");
        }


        //本当はやりたいこと _lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1);
        //テキストセット後すぐにはlineCountが更新できないので仕方なくこうしている
        SetContentBackgroundSize_Expanded(_lineSize * 5);
    }

    protected override void Awake()
    {
        base.Awake();

        _tMP_InputField = _inputField.GetComponent<TMP_InputField>();

    }
}
