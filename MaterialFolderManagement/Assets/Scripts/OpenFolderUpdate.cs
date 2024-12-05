using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenFolderUpdate : MonoBehaviour
{
    [SerializeField] private LoadingAndSavingFileFoldersScript _lAndSFFScript;

    public void OnUpdateButton()
    {
        _lAndSFFScript.SelectFolder(_lAndSFFScript.SelectFolderUserPath);
    }

}
