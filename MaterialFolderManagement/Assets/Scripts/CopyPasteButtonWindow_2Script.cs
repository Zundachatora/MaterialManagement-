using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPasteButtonWindow_2Script : CopyPasteButtonWindowCommonScript
{
    public enum Operation : int
    {
        OnCompletionButton,
    }

    public void OnCompletionButton()
    {
        _copyPasteButtonScript.ReceivingActionNotifications_Window2( Operation.OnCompletionButton);
    }
}
