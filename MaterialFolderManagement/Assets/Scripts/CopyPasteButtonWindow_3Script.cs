using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPasteButtonWindow_3Script : CopyPasteButtonWindowCommonScript
{

    [SerializeField] CopyPasteWindow3_RenameAreaScript _renameAreaScript;

    private string _fileFolderNewName;

    public enum Operation : int
    {
        OnCancelButton,
        OnOverwriteButton,
        OnRenameRetryButton,
        InputNewName,

    }

    public override void Initialization(CursorInformationScript cursorInformationScript, CopyPasteButtonScript copyPasteButtonScript)
    {
        base.Initialization(cursorInformationScript, copyPasteButtonScript);
    }

    public  void Initialization(CursorInformationScript cursorInformationScript, CopyPasteButtonScript copyPasteButtonScript,FileFoldersGrantData grantData,string nameSuggestion)
    {
        base.Initialization(cursorInformationScript, copyPasteButtonScript);
        
        _renameAreaScript.Initialization(nameSuggestion,grantData.Name, grantData.Alias);

        _renameAreaScript.ConfirmedInputHandle += InputNewName;
    }

    public override void Close()
    {
        base.Close();

        _renameAreaScript.ConfirmedInputHandle -= InputNewName;
    }

    public void OnCancelButton()
    {
        _copyPasteButtonScript.ReceivingActionNotifications_Window3( Operation.OnCancelButton,null);

    }

    public void OnOverwriteButton()
    {
        _copyPasteButtonScript.ReceivingActionNotifications_Window3( Operation.OnOverwriteButton,null);

    }

    public void OnRenameRetryButton()
    {
        _copyPasteButtonScript.ReceivingActionNotifications_Window3( Operation.OnRenameRetryButton, null);
    }

    public void InputNewName(string newName)
    {
        _fileFolderNewName = newName;

        _copyPasteButtonScript.ReceivingActionNotifications_Window3( Operation.InputNewName, _fileFolderNewName);
    }

}
