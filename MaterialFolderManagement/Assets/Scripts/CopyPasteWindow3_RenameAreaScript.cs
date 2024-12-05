using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CopyPasteWindow3_RenameAreaScript : MonoBehaviour
{
    public event ConfirmedInput ConfirmedInputHandle;
    public delegate void ConfirmedInput(string inputText);

    [SerializeField] private CopyPasteWindow3_RenameArea_ButtonScript[] _buttons;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject _suggestionButtons;

    public enum SuggestionButtons:int
    {
        Suggestion,
        Name,
        Alias,
    }

    public void OnEndEdit(string text)
    {
        ConfirmedInputHandle(text);

        //_suggestionButtons.SetActive(false);
    }

    public void OnSelect()
    {
        _suggestionButtons.SetActive(true);
    }

    public void OnSuggestionButton(SuggestionButtons suggestionButtons,string name)
    {
        ConfirmedInputHandle(name);

        _inputField.text = name;

        _suggestionButtons.SetActive(false);
    }

    public void Initialization(string suggestion,string name,string alias)
    {
        int index=0;

        foreach (var button in _buttons)
        {
            button.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(suggestion))
        {
            _buttons[index].Initialization(SuggestionButtons.Suggestion, suggestion);
            _buttons[index].gameObject.SetActive(true);
            index++;
        }

        if (!string.IsNullOrEmpty(name))
        {
            _buttons[index].Initialization(SuggestionButtons.Name, name);
            _buttons[index].gameObject.SetActive(true);
            index++;
        }

        if (!string.IsNullOrEmpty(alias))
        {
            _buttons[index].Initialization(SuggestionButtons.Alias, alias);
            _buttons[index].gameObject.SetActive(true);
            index++;
        }
    }
}
