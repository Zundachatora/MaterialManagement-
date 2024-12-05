using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchBoxMenuScript : MonoBehaviour
{
    [SerializeField] private RectTransform _cursorRegionRT;
    [SerializeField] private List<GameObject> _buttons;

    private CursorInformationScript _cursorInformation;

    public void Initialization(CursorInformationScript cursorInformationScript, SearchBoxScript searchBoxScript)
    {
        //カーソル領域のセット
        _cursorInformation = cursorInformationScript;
        _cursorInformation.RegionSet(_cursorRegionRT, CursorInformationScript.region.searchBoxMenu, 1);

        //ボタン達の初期化
        SearchBoxButtonCommon searchBoxButtonCommon;
        foreach (GameObject buttonGo in _buttons)
        {
            searchBoxButtonCommon = buttonGo.GetComponent<SearchBoxButtonCommon>();

            searchBoxButtonCommon.Initialization(searchBoxScript);
        }
    }
}
