using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPasteButtonWindowCommonScript : MonoBehaviour
{
    protected CopyPasteButtonScript _copyPasteButtonScript;

    [SerializeField] private RectTransform regionRT;


    /// <summary>
    /// base.Initialization()で呼び出してね。
    /// </summary>
    virtual public void Initialization(CursorInformationScript cursorInformationScript, CopyPasteButtonScript copyPasteButtonScript)
    {
        cursorInformationScript.RegionSet(regionRT, CursorInformationScript.region.copyPasteButtonWindow, 2);

        _copyPasteButtonScript = copyPasteButtonScript;
    }

    /// <summary>
    /// base.Close()で呼び出してね。
    /// </summary>
    virtual public void Close()
    {

    }
}
