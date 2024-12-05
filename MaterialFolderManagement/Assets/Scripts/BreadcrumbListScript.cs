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

        //pathが読み込めなかった時のデフォルトの値
        _inputField.text = "パン/くず/リスト";

        _lAndSFFScript.OnFolderSelected += BreadcrumbsTextUpdate;
    }

    private void BreadcrumbsTextUpdate()
    {
        _inputField.text = _lAndSFFScript.SelectFolderUserPath;
    }
}
