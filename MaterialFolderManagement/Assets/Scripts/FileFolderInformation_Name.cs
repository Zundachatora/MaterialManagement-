using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class FileFolderInformation_Name : InformationCommonUI
{
    [SerializeField] private float _lineSize= 56;
    [SerializeField] private GameObject _inputField;

    private TMP_InputField _tMP_InputField;

    public void OnValueChanged(string text)
    {
        SetContentBackgroundSize_Expanded(_lineSize * (_tMP_InputField.textComponent.textInfo.lineCount+1));
    }

    public void OnSelect(string text)
    {
        SetContentBackgroundSize_Expanded(_lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1));

        _tMP_InputField.caretPosition = _tMP_InputField.text.Length;
    }

    public void OnEndEdit(string text)
    { 
       bool renamed = _fileFolderInformationAreaScript.ShowingGrantDatasSave_Renaming(text);

        if (renamed)
        {

        }
        else
        {
            if (_fileFolderInformationAreaScript.SelectGrantData.Length == 0)
            {
                _tMP_InputField.SetTextWithoutNotify("");
            }
            else if (_fileFolderInformationAreaScript.SelectGrantData.Length == 1)
            {
                _tMP_InputField.SetTextWithoutNotify(_fileFolderInformationAreaScript.SelectGrantData[0].Name);
            }
            else
            {
                _tMP_InputField.SetTextWithoutNotify("-");
            }
        }
    }


    public override void InitialSetting(bool opening, int scrollIndex, (Sprite expand, Sprite fold) icons)
    {
        base.InitialSetting(opening, scrollIndex, icons);

        //�����I�����Ă���ꍇ�̕���
        if(_fileFolderInformationAreaScript.SelectGrantData.Length==0)
        {
            _tMP_InputField.SetTextWithoutNotify("");
        }
        else if(_fileFolderInformationAreaScript.SelectGrantData.Length == 1)
        {
            _tMP_InputField.SetTextWithoutNotify(_fileFolderInformationAreaScript.SelectGrantData[0].Name);
        }
        else
        {
            _tMP_InputField.SetTextWithoutNotify("-");
        }


        //�{���͂�肽������ _lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1);
        //�e�L�X�g�Z�b�g�シ���ɂ�lineCount���X�V�ł��Ȃ��̂Ŏd���Ȃ��������Ă���
        SetContentBackgroundSize_Expanded(_lineSize * (5));
    }

    protected override void Awake()
    {
        base.Awake();

        _tMP_InputField = _inputField.GetComponent<TMP_InputField>();

    }

   
}
