using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileFolderInformation_Hashtag : InformationCommonUI
{
    [SerializeField] private float _lineSize = 56;
    [SerializeField] private GameObject _inputField;

    private TMP_InputField _tMP_InputField;

    public void OnValueChanged(string text)
    {
        SetContentBackgroundSize_Expanded(_lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1));


        _tMP_InputField.text = AddHeadHashtag(text);
    }


    public void OnSelect(string text)
    {
        SetContentBackgroundSize_Expanded(_lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1));

        _tMP_InputField.text = AddHeadHashtag(text);

        _tMP_InputField.caretPosition = _tMP_InputField.text.Length;
    }

    public void OnEndEdit(string text)
    {
        text = AddHeadHashtag(text);

        //�擪�́u#�v���������^�O�z��̍쐬�B
        List<string> saveTags = new List<string>(text.Split("\n"));
        for (int i = 0; i < saveTags.Count; i++)
        {
            if(saveTags[i].Length == 0)
            {
                saveTags.RemoveAt(i);
                i--;
            }
            else if(saveTags[i].StartsWith("#"))
            {
                saveTags[i] = saveTags[i].Substring(1);

                if (saveTags[i].Length==0)
                {
                    saveTags.RemoveAt(i);
                    i--;
                }
            }
        }

        //���͂��Ȃ���΃Z�[�u���Ȃ�
        if (saveTags.Count == 0) return;

        //������ăZ�[�u
        foreach (var grantData in _fileFolderInformationAreaScript.SelectGrantData)
        {
            grantData.Hashtag = saveTags.ToArray();
            grantData.IsUpdated = true;
        }
        _fileFolderInformationAreaScript.ShowingGrantDatasSave();

        //���̓t�B�[���h�ɕ\��
        if (_fileFolderInformationAreaScript.SelectGrantData.Length == 0)
        {
            _tMP_InputField.SetTextWithoutNotify("");
        }
        else 
        {
            _tMP_InputField.SetTextWithoutNotify(GenerateHashtagTextFromGrantData(saveTags.ToArray()));
        }
    }


    public override void InitialSetting(bool opening, int scrollIndex, (Sprite expand, Sprite fold) icons)
    {
        base.InitialSetting(opening, scrollIndex, icons);

        //�����I�����Ă���ꍇ�̕���
        if (_fileFolderInformationAreaScript.SelectGrantData.Length == 1)
        {
            _tMP_InputField.SetTextWithoutNotify(GenerateHashtagTextFromGrantData(_fileFolderInformationAreaScript.SelectGrantData[0].Hashtag));
        }
        else
        {
            _tMP_InputField.SetTextWithoutNotify("");
        }


        //�{���͂�肽������ _lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1);
        //�e�L�X�g�Z�b�g�シ���ɂ�lineCount���X�V�ł��Ȃ��̂Ŏd���Ȃ��������Ă���
        SetContentBackgroundSize_Expanded(_lineSize * 5);
    }

    protected override void Awake()
    {
        base.Awake();

        _tMP_InputField = _inputField.GetComponent<TMP_InputField>();

    }

    /// <summary>
    /// �S�Ă̍s�̐擪�Ɂu#�v��t����B���ɂ���ꍇ�͕t���Ȃ��BforceAdd=true�őS�Ă̍s�ɒǉ�����B
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string AddHeadHashtag(string text,bool forceAdd=false)
    {
        string[] lines = text.Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            if(forceAdd)
            {
                lines[i] = "#" + lines[i];
            }
            else
            {
                if (!lines[i].StartsWith("#")) lines[i] = "#" + lines[i];
            }
        }
        return string.Join("\n", lines);
    }

    private string GenerateHashtagTextFromGrantData(string[] strings)
    {
        string returnText = string.Join("\n", strings);

        returnText = AddHeadHashtag(returnText, true);

        return returnText;
    }
}
