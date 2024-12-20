using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchBoxButtonCommon : MonoBehaviour
{
    public SearchBoxScript.ButtonNameEnum ButtonNameEnum { get; private set; }
    public bool CheckSheetOn { get; private set; } = false;
    public string[] SearchStrings { get; protected set; } = new string[0];
    
    [SerializeField]private SearchBoxScript.ButtonNameEnum _setButtonNameEnum = SearchBoxScript.ButtonNameEnum.others;

    private SearchBoxScript _searchBoxScript;
    /// <summary>
    /// base.Initialization();してね
    /// </summary>
    /// <param name="gameObjectWithSearchBoxScript"></param>
    /// <param name="buttonNameEnum"></param>
    virtual public void Initialization(SearchBoxScript searchBoxScript)
    {
        ButtonNameEnum = _setButtonNameEnum;
        CheckSheetOn = false;
        SearchStrings = new string[0];

        _searchBoxScript = searchBoxScript;
    }

    /// <summary>
    /// base.OnButton();で呼び出してね
    /// </summary>
    virtual protected void OnButton()
    {
        CheckSheetOn = true;
    }
    /// <summary>
    /// base.OffButton();で呼び出してね
    /// </summary>
    virtual protected void OffButton()
    {
        CheckSheetOn = false;
    }

    protected void OnButtonNotification(SearchBoxButtonCommon searchBoxButtonCommon)
    {
        if (_searchBoxScript == null) Debug.LogError("初期化忘れ、又はnullを引数に代入している");
        _searchBoxScript.OnSearchBoxMenuButton(searchBoxButtonCommon);
    }

    
}
