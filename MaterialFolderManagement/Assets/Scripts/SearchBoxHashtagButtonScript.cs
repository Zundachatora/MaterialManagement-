using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchBoxHashtagButtonScript : SearchBoxButtonCommon
{
    [SerializeField] private float _lineSize = 57;
    [SerializeField] private TMP_InputField _tMP_InputField;
    [SerializeField] private RectTransform _inputFieldRT;
    [SerializeField] private Sprite _checkIconSprite;
    [SerializeField] private Sprite _noCheckIconSprite;
    [SerializeField] private GameObject _checkButtonImageGO;
    private Image _checkButtonImage;

    override public void Initialization(SearchBoxScript searchBoxScript)
    {
        base.Initialization(searchBoxScript);

        if (CheckSheetOn)
        {
            OnButton();
        }
        else
        {
            OffButton();
        }

        OnButtonNotification(GetComponent<SearchBoxButtonCommon>());
    }

    public void OnEndEdit(string text)
    {
        //先頭の「#」を除いたタグ配列の作成。
        List<string> saveTags = new List<string>(text.Split("\n"));
        for (int i = 0; i < saveTags.Count; i++)
        {
            if (saveTags[i].Length == 0)
            {
                saveTags.RemoveAt(i);
                i--;
            }
            else if (saveTags[i].StartsWith("#"))
            {
                saveTags[i] = saveTags[i].Substring(1);

                if (saveTags[i].Length == 0)
                {
                    saveTags.RemoveAt(i);
                    i--;
                }
            }
        }

        SearchStrings = saveTags.ToArray();

        OnButtonNotification(GetComponent<SearchBoxButtonCommon>());

        _inputFieldRT.gameObject.SetActive(false);
    }
    public void OnSelect(string text)
    {
        float setSize = _lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1);

        float difference = setSize - _inputFieldRT.sizeDelta.y;
        Vector2 position = _inputFieldRT.anchoredPosition;
        position.y -= difference / 2;

        _inputFieldRT.anchoredPosition = position;
        _inputFieldRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, setSize);

        _tMP_InputField.text = AddHeadHashtag(text);

        _tMP_InputField.caretPosition = _tMP_InputField.text.Length;
    }
    public void OnValueChanged(string text)
    {
        float setSize = _lineSize * (_tMP_InputField.textComponent.textInfo.lineCount + 1);

        float difference = setSize - _inputFieldRT.sizeDelta.y;
        Vector2 position = _inputFieldRT.anchoredPosition;
        position.y -= difference / 2;

        _inputFieldRT.anchoredPosition = position;
        _inputFieldRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, setSize);

        _tMP_InputField.text = AddHeadHashtag(text);
    }

    public void OnCheckSheetButton()
    {
        if (CheckSheetOn)
        {
            OffButton();
        }
        else
        {
            OnButton();
        }

        OnButtonNotification(GetComponent<SearchBoxButtonCommon>());
    }

    public void OnInputFieldActiveOrInactiveButton()
    {
        if(_inputFieldRT.gameObject.activeSelf)
        {
            _inputFieldRT.gameObject.SetActive(false);
        }
        else
        {
            _inputFieldRT.gameObject.SetActive(true);
        }
    }

    override protected void OnButton()
    {
        base.OnButton();

        _checkButtonImage.sprite = _checkIconSprite;
    }

    override protected void OffButton()
    {
        base.OffButton();

        _checkButtonImage.sprite = _noCheckIconSprite;

    }

    private void Awake()
    {
        _checkButtonImage = _checkButtonImageGO.GetComponent<Image>();
    }

    /// <summary>
    /// 全ての行の先頭に「#」を付ける。既にある場合は付けない。forceAdd=trueで全ての行に追加する。
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string AddHeadHashtag(string text, bool forceAdd = false)
    {
        string[] lines = text.Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            if (forceAdd)
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
}
