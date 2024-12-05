using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchBoxDefaultButtonScript : SearchBoxButtonCommon
{
    [SerializeField] private Sprite _checkIconSprite;
    [SerializeField] private Sprite _noCheckIconSprite;
    [SerializeField] private GameObject _checkButtonImageGO;
    private Image _checkButtonImage;

    override public void Initialization(SearchBoxScript searchBoxScript)
    {
        base.Initialization(searchBoxScript);

        if (CheckSheetOn)
        {
            OnButton();
        }
        else
        {
            OffButton();
        }

        OnButtonNotification(GetComponent<SearchBoxButtonCommon>());
    }

    public void OnCheckSheetButton()
    {
        if (CheckSheetOn)
        {
            OffButton();
        }
        else
        {
            OnButton();
        }

        OnButtonNotification(GetComponent<SearchBoxButtonCommon>());
    }


    override protected void OnButton()
    {
        base.OnButton();

        _checkButtonImage.sprite = _checkIconSprite;
    }

    override protected void OffButton()
    {
        base.OffButton();

        _checkButtonImage.sprite = _noCheckIconSprite;

    }

    private void Awake()
    {
        _checkButtonImage = _checkButtonImageGO.GetComponent<Image>();
    }
}
