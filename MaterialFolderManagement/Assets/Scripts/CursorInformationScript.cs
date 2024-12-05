using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorInformationScript : MonoBehaviour
{
    public region LastClickLocation { get;private set; } = region.others;

    public event OnClick OnClickEvent = delegate { };

    public delegate void OnClick();

    public enum region
    {
        fileFolderDisplayArea = 0,
        pinnedFolderArea,
        fileFolderInspectorArea,
        headerArea,
        searchBoxMenu,
        copyPasteButtonWindow,
        others,
    }

    [SerializeField] private RectTransform _canvasRT;
    [SerializeField] private InputActionProperty _inputAction;

    private List<(RectTransform rTransform,region regionName,int layer)> _region = new List<(RectTransform rTransform, region regionName, int layer)>();
    /// <summary>
    /// �̈�̍폜��RectTransform��j������Ύ����ō폜�����B
    /// </summary>
    /// <param name="rTransform"></param>
    /// <param name="regionName"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public bool RegionSet(RectTransform rTransform, region regionName, int layer)
    {
        if (rTransform == null) return false;
        
        //�j�������ׂ��̈�̍폜
        for (int i = 0; i < _region.Count; i++)
        {
            if ((_region[i].rTransform == null) || (_region[i].rTransform == rTransform))//Rect���d������ꍇ�͏㏑������
            {
                _region.RemoveAt(i);
                i--;
                continue;
            }
        }

        _region.Add((rTransform, regionName, layer));

        _region = _region
                   .OrderByDescending(item => item.layer)
                   .ToList();

        return true;
    }


    private void Awake()
    {
        _inputAction.action.performed += OnPress;
    }
    
    private void OnEnable()
    {
        _inputAction.action.Enable();
    }

    //�N���b�N��
    private void OnPress(InputAction.CallbackContext context)
    {
        Vector2 position = _inputAction.action.ReadValue<Vector2>();

        float magnification = _canvasRT.sizeDelta.x / Screen.width;
        position.x *= magnification;
        position.y *= magnification;

        LastClickLocation = ClickLocation(position);

        OnClickEvent();
    }

    /// <summary>
    /// ���C���[�̈قȂ�d������̈悪����ꍇ�傫�Ȓl�̃��C���[���D�悳���B��\��(GO�̔�A�N�e�B�u)���C���[�͖��������
    /// </summary>
    /// <param name="clickPosition"></param>
    /// <returns></returns>
    private region ClickLocation(Vector2 clickPosition)
    {

        for (int i = 0; i < _region.Count; i++)
        {
            if (_region[i].rTransform==null)
            {
                _region.RemoveAt(i);
                i--;
                continue;
            }

            float leftX = _region[i].rTransform.position.x;
            leftX *= _canvasRT.sizeDelta.x / Screen.width;
            leftX -= (_region[i].rTransform.rect.size.x / 2);

            float rightX = _region[i].rTransform.position.x;
            rightX *= _canvasRT.sizeDelta.x / Screen.width;
            rightX +=(_region[i].rTransform.rect.size.x / 2);

            if ((clickPosition.x >= leftX) && (clickPosition.x <= rightX))
            {
                float bottomY = _region[i].rTransform.position.y;
                bottomY *= _canvasRT.sizeDelta.y / Screen.height;
                bottomY -= (_region[i].rTransform.rect.size.y / 2);

                float topY = _region[i].rTransform.position.y;
                topY *= _canvasRT.sizeDelta.y / Screen.height;
                topY += (_region[i].rTransform.rect.size.y / 2);

                if ((clickPosition.y >= bottomY) && (clickPosition.y <= topY))
                {//�͈͓�
                    //���C���[��\���̏ꍇ
                    if (!_region[i].rTransform.gameObject.activeInHierarchy) continue;

                    return _region[i].regionName;
                }
            }
        }
        return region.others;
    }


}
