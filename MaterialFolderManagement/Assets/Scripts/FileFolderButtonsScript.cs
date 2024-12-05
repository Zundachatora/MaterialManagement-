using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;

public class FileFolderButtonsScript : MonoBehaviour
{
    /// <summary>LoadingAndSavingFileFoldersScriptのFileFoldersGrantDatas配列のインデックス</summary>
    public int IndexNumber { get;private set; }

    [SerializeField] private Color32 _lockColor;
    [SerializeField] private Color32 _isSelectedColor;
    [SerializeField] private Color32 _backgroundDefaultColor;
    [SerializeField] private Color32 _unLockColor;
    [SerializeField] private Image _lockIconImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _mainIcon;
    [SerializeField] private TMP_Text _buttonNameTMP;
    [SerializeField] private float _doubleClickWaitingTime = 0.5f;
   
    private int _clickCount;
    private int _clickCount_Lock;

    private FileFolderDisplayAreaScript _fileFolderDisplayAreaScript=null;

    public void InitialSetting(FileFoldersGrantData fileFoldersGrantData,int indexOfFileFoldersGrantData, FileFolderDisplayAreaScript fileFolderDisplayAreaScript)
    {
        //重複部分はこちらで処理する
        InitialSetting(indexOfFileFoldersGrantData, fileFolderDisplayAreaScript);

        _backgroundImage.color = fileFoldersGrantData.IsSelected ? _isSelectedColor : _backgroundDefaultColor;

        _lockIconImage.color = fileFoldersGrantData.IsLocked ? _lockColor : _unLockColor;

        _buttonNameTMP.SetText((fileFoldersGrantData.Name != null)?fileFoldersGrantData.Name:"");

    }

    public void InitialSetting(int indexOfFileFoldersGrantData, FileFolderDisplayAreaScript fileFolderDisplayAreaScript)
    {
        _fileFolderDisplayAreaScript= fileFolderDisplayAreaScript;

        IndexNumber = indexOfFileFoldersGrantData;

        _clickCount = 0;

        _clickCount_Lock = 0;

        _backgroundImage.color =_backgroundDefaultColor;

        _lockIconImage.color = _unLockColor;

        _buttonNameTMP.SetText("");

    }

    public void IconSetting(Sprite sprite)
    {
        _mainIcon.sprite = sprite;
    }

    public void IsLocked(bool isLock)
    {
        if (isLock)
        {
            _lockIconImage.color = _lockColor;
        }
        else
        {
            _lockIconImage.color = _unLockColor;
        }
    }

    public void IsSelected(bool isSelected)
    {

        if (isSelected)
        {
            _backgroundImage.color = _isSelectedColor;
        }
        else
        {
            _backgroundImage.color = _backgroundDefaultColor;
        }
    }

    public void FileFolderOnClick()
    {
        if (_fileFolderDisplayAreaScript == null) return;

        _clickCount++;

        if (_clickCount == 1) StartCoroutine(ClickHandling(_doubleClickWaitingTime));

    }

    public void LockOnClick()
    {
        if (_fileFolderDisplayAreaScript == null) return;

        _clickCount_Lock++;

        if (_clickCount_Lock == 1) StartCoroutine(ClickHandling_Lock(_doubleClickWaitingTime));
    }
    private IEnumerator ClickHandling_Lock(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        if (_clickCount_Lock == 1)
        {//シングルクリック
            _fileFolderDisplayAreaScript.AreaButtonClick(IndexNumber, false, true);
        }
        else
        {//ダブルクリック
            _fileFolderDisplayAreaScript.AreaButtonClick(IndexNumber, true, true);
        }

        _clickCount_Lock = 0;

        yield break;
    }
    private IEnumerator ClickHandling(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        if (_clickCount == 1)
        {//シングルクリック
            _fileFolderDisplayAreaScript.AreaButtonClick(IndexNumber,false, false);
        }
        else
        {//ダブルクリック
            _fileFolderDisplayAreaScript.AreaButtonClick(IndexNumber,true, false);
        }

        _clickCount = 0;

        yield break;
    }

}
