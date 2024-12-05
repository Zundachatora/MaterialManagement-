using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BreadcrumbListScript : MonoBehaviour
{
    [SerializeField] private LoadingAndSavingFileFoldersScript _lAndSFFScript;
    private TMP_InputField _inputField;

    public void OnEndEdit()
    {
        if(!_lAndSFFScript.SelectFolder(_inputField.text)) BreadcrumbsTextUpdate();
    }

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();

        //path���ǂݍ��߂Ȃ��������̃f�t�H���g�̒l
        _inputField.text = "�p��/����/���X�g";

        _lAndSFFScript.OnFolderSelected += BreadcrumbsTextUpdate;
    }

    private void BreadcrumbsTextUpdate()
    {
        _inputField.text = _lAndSFFScript.SelectFolderUserPath;
    }
}
