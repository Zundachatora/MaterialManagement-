using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public class InformationCommonUI : MonoBehaviour
{
    public bool Opening { get; private set; } = false;
    public int? ThisScrollIndex { get; set; } = null;

    /// <summary>�{�^���ɉ�����SetActive����Bclose�Ə�����Ă��邪�\��GameObject�����˂Ă���B</summary>
    [SerializeField] protected List<GameObject> _closeGameObjectList = new List<GameObject>();
    [SerializeField] protected FileFolderInformationAreaScript _fileFolderInformationAreaScript;
    [SerializeField] protected float _verticalSize_fold = 57;
    [SerializeField] protected float _verticalSize_expand = 0;

    [SerializeField] private GameObject _expandAndFoldIconGameObject;
    [SerializeField] private RectTransform _contentBackground;

    //private (float? expand, float? fold) _scrollSize;//fold�̒l�͏���������
    private (Sprite expand, Sprite fold) _iconSprites = new(null, null);
    private Image _expandAndFoldIcon;

    public void Open()
    {
        Opening = true;

        SetContentBackgroundSize_Expanded(_verticalSize_expand);

        if (_iconSprites.fold != null) _expandAndFoldIcon.sprite = _iconSprites.fold;

        foreach (GameObject go in _closeGameObjectList)
        {
            go.SetActive(true);
        }
    }

    public void Close()
    {
        Opening = false;

        SetContentBackgroundSize_Expanded(_verticalSize_fold);

        if (_iconSprites.expand != null) _expandAndFoldIcon.sprite = _iconSprites.expand;

        foreach (GameObject go in _closeGameObjectList)
        {
            go.SetActive(false);
        }
    }

    public void SetIcons((Sprite expand, Sprite fold) icons)
    {
        _iconSprites = icons;

        if(Opening)
        {
            if (_iconSprites.expand != null) _expandAndFoldIcon.sprite = _iconSprites.expand;
        }
        else
        {
            if (_iconSprites.fold != null) _expandAndFoldIcon.sprite = _iconSprites.fold;
        }

    }

    public void OpenOrCloseButtonClick()
    {
        Opening = !Opening;

        if (Opening)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    
    /// <summary>
    /// base.InitialSetting();�ŌĂяo���ĂˁB
    /// </summary>
    /// <param name="grantDatas"></param>
    /// <param name="icons"></param>
    public virtual void InitialSetting(bool opening, int scrollIndex, (Sprite expand, Sprite fold) icons)
    {
        ThisScrollIndex = scrollIndex;
        _iconSprites = icons;

        if(opening)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    /// <summary>
    /// base.Awake();�ŌĂяo���ĂˁB
    /// </summary>
    protected virtual void Awake()
    {
        _expandAndFoldIcon = _expandAndFoldIconGameObject.GetComponent<Image>();
    }

    /// <summary>
    /// �ʒu�͎����Œ��������BFileFolderInformationAreaScript�̃X�N���[����̃T�C�Y���ύX�����B
    /// </summary>
    /// <param name="setSize"></param>
    protected void SetContentBackgroundSize_Expanded(float setSize)
    {

        if(Opening)
        {
            _verticalSize_expand = setSize;
        }
        else
        {
            _verticalSize_fold = setSize;
        }

        float difference = setSize - _contentBackground.sizeDelta.y;

        Vector2 position = _contentBackground.anchoredPosition;

        position.y -= difference / 2;

        _contentBackground.anchoredPosition = position;
        _contentBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, setSize);

        //�X�N���[����ł̑z��T�C�Y���ύX
        if(Opening)
        {//�^�C�g�������ƃR���e���c���������邪���܂Ń^�C�g���������l�������Ƀv���O�����������Ă����̂ŋ����ɏC��//��ŏC��������
            if (ThisScrollIndex != null) _fileFolderInformationAreaScript.ScrollElementResize((int)ThisScrollIndex, setSize + _verticalSize_fold);

        }
        else
        {
            if (ThisScrollIndex != null) _fileFolderInformationAreaScript.ScrollElementResize((int)ThisScrollIndex, setSize);
        }
    }




}
