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
        //�J�[�\���̈�̃Z�b�g
        _cursorInformation = cursorInformationScript;
        _cursorInformation.RegionSet(_cursorRegionRT, CursorInformationScript.region.searchBoxMenu, 1);

        //�{�^���B�̏�����
        SearchBoxButtonCommon searchBoxButtonCommon;
        foreach (GameObject buttonGo in _buttons)
        {
            searchBoxButtonCommon = buttonGo.GetComponent<SearchBoxButtonCommon>();

            searchBoxButtonCommon.Initialization(searchBoxScript);
        }
    }
}
