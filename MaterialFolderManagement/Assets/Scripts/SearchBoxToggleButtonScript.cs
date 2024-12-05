using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchBoxToggleButtonScript : SearchBoxButtonCommon
{
    [SerializeField] private RectTransform _toggleRT;
    [SerializeField] private TextMeshProUGUI _leftText;
    [SerializeField] private TextMeshProUGUI _rightText;

    override public void Initialization(SearchBoxScript searchBoxScript)
    {
        base.Initialization(searchBoxScript);

        if(CheckSheetOn)
        {
            ToggleRight();
        }
        else
        {
            ToggleLeft();
        }

        OnButtonNotification(GetComponent<SearchBoxButtonCommon>());
    }

    public void OnToggleButton()
    {
        if (CheckSheetOn)
        {
            ToggleLeft();
        }
        else
        {
            ToggleRight();
        }

        OnButtonNotification(GetComponent<SearchBoxButtonCommon>());
    }

    private void ToggleRight()
    {
        OnButton();

        _toggleRT.localEulerAngles = Vector3.zero;
        
        _leftText.color = new Color32(0,0,0,100);
        _rightText.color = new Color32(0,0,0,255);
    }

    private void ToggleLeft()
    {
        OffButton();

        _toggleRT.localEulerAngles = new Vector3(0,0,180);

        _leftText.color = new Color32(0, 0, 0, 255);
        _rightText.color = new Color32(0, 0, 0, 100);
    }


}
