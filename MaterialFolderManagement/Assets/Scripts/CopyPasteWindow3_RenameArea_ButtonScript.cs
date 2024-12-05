using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CopyPasteWindow3_RenameArea_ButtonScript : MonoBehaviour
{
    [SerializeField] private CopyPasteWindow3_RenameAreaScript _copyPasteWindow3_RenameAreaScript;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private string _name;
    private CopyPasteWindow3_RenameAreaScript.SuggestionButtons _thisSuggestionButtons;

    public void Initialization(CopyPasteWindow3_RenameAreaScript.SuggestionButtons suggestionButtons,string name)
    {
        _name = name;
        _thisSuggestionButtons = suggestionButtons;
        _buttonText.text = _name;

        switch (suggestionButtons)
        {
            case CopyPasteWindow3_RenameAreaScript.SuggestionButtons.Suggestion:
                _titleText.text = "’ñˆÄ";
                break;
            case CopyPasteWindow3_RenameAreaScript.SuggestionButtons.Name:
                _titleText.text = "–¼‘O";
                break;
            case CopyPasteWindow3_RenameAreaScript.SuggestionButtons.Alias:
                _titleText.text = "•Ê–¼";
                break;
        }
    }

    public void OnButton()
    {
        _copyPasteWindow3_RenameAreaScript.OnSuggestionButton(_thisSuggestionButtons, _name);
    }
}
