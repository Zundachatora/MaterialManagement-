using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController<D,S> : MonoBehaviour where S : MonoBehaviour
{
    //�����ŗp�ӂ���f�[�^ Content(�T�C�Y�ύX) scrollbar(�o�[�ʒu�̃��Z�b�g) scrollRect(�ړ����) ��(����) ��Ɖ��̗]��
    //���肩��~�����f�[�^ ScrollGameObject ScrollData �c��Size �\�������ۂ̏���(�f���Q�[�g)

    public delegate S DisplayItemWhenRedisplayed(S displayObject, int index,ref D data);
    public delegate S DisplayObjectBeforeHiding(S displayObject, int index,ref D data);
    public DisplayItemWhenRedisplayed DisplayObjectWhenRedisplayedDelegate = null;
    public DisplayObjectBeforeHiding DisplayObjectBeforeHidingDelegate = null;

    /// <summary>SetScrollData�֐������s���Ƀo�[����ɖ߂����ǂ���</summary>
    [SerializeField] protected bool ScrollBarReset { get; set; }= true;

    [SerializeField] private RectTransform _contentArea;
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private float _space;
    [SerializeField] private float _margin;

    private List<(D data, int displayGObjectIndex, float verticalSize)> _scrollDatas;
    private List<Stack<S>> _stockpileGameObjects = new List<Stack<S>>();
    private RectTransform[] _instantiateDisplayGameObjects = new RectTransform[0];
    private LinkedList<(S displayObject, int scrollDatasIndex)> _scrollObject = new LinkedList<(S displayObject, int scrollDatasIndex)>();
    private int _maxNumOfDisplay;
    private int _barResetCount;

    /// <summary>�X�N���[�����ɌĂяo��</summary>
    /// <param name="SRNedPosition"></param>
    public void ScrollOnValueChanged(Vector2 SRNedPosition)//ScrollRect.normalizedPosition
    {
        if (_scrollDatas.Count == 0) return;

        //�ʒu�v�Z
        //�X�N���[�����ꂽ��(���ݕ`��ʒu)
        float scrollPosition = (_contentArea.rect.height - _scrollRect.viewport.rect.height) * (1 - _scrollRect.normalizedPosition.y);

        //���ݕ`�悳��Ă���ł������ʒu�ɂ���C���f�b�N�X
        int topIndex = 0;
        float remainingHeight = scrollPosition - _margin;
        foreach (var item in _scrollDatas)
        {

            remainingHeight -= item.verticalSize + _space;

            if (remainingHeight <= 0) break;

            topIndex++;
        }
        if (topIndex >= _scrollDatas.Count) topIndex = (_scrollDatas.Count - 1);

        //DisplayObject��topIndex�������Ƃ������ɐV����DisplayObject��ǉ�
        while (true)
        {
            if (_scrollObject.First.Value.scrollDatasIndex < topIndex)
            {
                LinkedListNode<(S displayObject, int scrollDatasIndex)> displayItem = _scrollObject.First;

                //��\�����̏���
                if (DisplayObjectBeforeHidingDelegate != null)
                {
                    var refData = _scrollDatas[displayItem.Value.scrollDatasIndex].data;
                    DisplayObjectBeforeHidingDelegate(
                        displayItem.Value.displayObject,
                        displayItem.Value.scrollDatasIndex,
                        ref refData
                            );

                    //�A���ė���data�̔��f
                    _scrollDatas[displayItem.Value.scrollDatasIndex] = (
                        refData,
                        _scrollDatas[displayItem.Value.scrollDatasIndex].displayGObjectIndex,
                        _scrollDatas[displayItem.Value.scrollDatasIndex].verticalSize
                        );
                }

                //DisplayObject���̃f�[�^���X�V�B
                int addIndex = _scrollObject.Last.Value.scrollDatasIndex + 1;
                if ((addIndex >= 0) && (addIndex < _scrollDatas.Count))
                {
                    displayItem.Value.displayObject.gameObject.SetActive(false);
                    _stockpileGameObjects[_scrollDatas[addIndex].displayGObjectIndex].Push(displayItem.Value.displayObject);

                    var refData = _scrollDatas[addIndex].data;

                    displayItem.Value = (DisplayObjectWhenRedisplayedDelegate(
                        (_stockpileGameObjects[_scrollDatas[addIndex].displayGObjectIndex].Count==0)? 
                        Instantiate(_instantiateDisplayGameObjects[_scrollDatas[addIndex].displayGObjectIndex], _contentArea).GetComponent<S>():
                        _stockpileGameObjects[_scrollDatas[addIndex].displayGObjectIndex].Pop(),
                        addIndex,
                        ref refData),
                        addIndex);

                    //�A���ė���data�̔��f
                    _scrollDatas[addIndex] = (
                        refData,
                        _scrollDatas[addIndex].displayGObjectIndex,
                        _scrollDatas[addIndex].verticalSize
                        );

                    displayItem.Value.displayObject.gameObject.SetActive(true);
                }
                else
                {//Data��Index�O�̏ꍇ�͏������Ȃ��B
                    break;
                }

                //�\���ʒu
                Vector2 vector2 = _instantiateDisplayGameObjects[0].anchoredPosition;//��ŏC��
                vector2.y = GetElementPosition_y(displayItem.Value.scrollDatasIndex);
                displayItem.Value.displayObject.gameObject.GetComponent<RectTransform>().anchoredPosition = vector2;

                //�����Ɉړ��B
                _scrollObject.RemoveFirst();
                _scrollObject.AddLast(displayItem);

            }
            else
            {
                break;
            }
        }


        //topIndex+(�ő�`�搔-1)������ꍇ�͐擪��DisplayObject��ǉ�
        while (true)
        {
            if (_scrollObject.Last.Value.scrollDatasIndex > topIndex + (_maxNumOfDisplay - 1))
            {
                LinkedListNode<(S displayObject, int scrollDatasIndex)> displayItem = _scrollObject.Last;

                //��\�����̏���
                if (DisplayObjectBeforeHidingDelegate != null)
                {
                    var refData = _scrollDatas[displayItem.Value.scrollDatasIndex].data;
                    DisplayObjectBeforeHidingDelegate(
                        displayItem.Value.displayObject,
                        displayItem.Value.scrollDatasIndex,
                        ref refData
                            );

                    //�A���ė���data�̔��f
                    _scrollDatas[displayItem.Value.scrollDatasIndex] = (
                        refData,
                        _scrollDatas[displayItem.Value.scrollDatasIndex].displayGObjectIndex,
                        _scrollDatas[displayItem.Value.scrollDatasIndex].verticalSize
                        );
                }

                //DisplayObject���̃f�[�^���X�V�B
                int addIndex = _scrollObject.First.Value.scrollDatasIndex - 1;
                if ((addIndex < _scrollDatas.Count) && (addIndex >= 0))
                {
                    displayItem.Value.displayObject.gameObject.SetActive(false);
                    _stockpileGameObjects[_scrollDatas[addIndex].displayGObjectIndex].Push(displayItem.Value.displayObject);

                    var refData = _scrollDatas[addIndex].data;

                    displayItem.Value = (DisplayObjectWhenRedisplayedDelegate(
                        (_stockpileGameObjects[_scrollDatas[addIndex].displayGObjectIndex].Count == 0) ?
                        Instantiate(_instantiateDisplayGameObjects[_scrollDatas[addIndex].displayGObjectIndex], _contentArea).GetComponent<S>() :
                        _stockpileGameObjects[_scrollDatas[addIndex].displayGObjectIndex].Pop(),
                        addIndex,
                        ref refData),
                        addIndex);

                    //�A���ė���data�̔��f
                    _scrollDatas[addIndex] = (
                        refData,
                        _scrollDatas[addIndex].displayGObjectIndex,
                        _scrollDatas[addIndex].verticalSize
                        );

                    displayItem.Value.displayObject.gameObject.SetActive(true);
                }
                else
                {//Data��Index�O�̏ꍇ�͏������Ȃ��B
                    break;
                }

                //�\���ʒu
                Vector2 vector2 = _instantiateDisplayGameObjects[0].anchoredPosition;//��ŏC��
                vector2.y = GetElementPosition_y(displayItem.Value.scrollDatasIndex);
                displayItem.Value.displayObject.gameObject.GetComponent<RectTransform>().anchoredPosition = vector2;

                //�擪�Ɉړ�
                _scrollObject.RemoveLast();
                _scrollObject.AddFirst(displayItem);

            }
            else
            {
                break;
            }
        }

    }

    protected void SetScrollData((D data, int displayGObjectIndex, float verticalSize)[] scrollDatas,
       RectTransform[] instantiateGOs,
       DisplayItemWhenRedisplayed displayObjectDelegate,
       DisplayObjectBeforeHiding displayObjectBeforeHidingDelegate=null)
    {
        _instantiateDisplayGameObjects = instantiateGOs.ToArray();

        _scrollDatas = new List<(D data, int displayGObjectIndex, float verticalSize)>(scrollDatas);

        DisplayObjectWhenRedisplayedDelegate = displayObjectDelegate;

        DisplayObjectBeforeHidingDelegate = displayObjectBeforeHidingDelegate;

        ContentAreaReckoning();

        DisplayGameObjectsInitialSettingOrReset();

        if (ScrollBarReset) _barResetCount = 1;
    }

    /// <summary>
    /// �ύX����ۂ͊����̔z��Ɠ��������ł���K�v������B
    /// </summary>
    /// <param name="instantiateGOs"></param>
    /// <returns></returns>
    protected bool ChangeInstantiateDisplayGameObjects(RectTransform[] instantiateGOs)
    {
        if (_instantiateDisplayGameObjects.Length == instantiateGOs.Length) return false;

        _instantiateDisplayGameObjects = instantiateGOs.ToArray();
    
        return true;
    }

    protected RectTransform[] GetInstantiateDisplayGameObjects()
    {
        return _instantiateDisplayGameObjects.ToArray();
    }
    /// <summary>
    /// ����:Action(S, index, D)
    /// </summary>
    /// <param name="action"></param>
    protected void DisplayObjectAccess(Action<S, int, D> action)
    {
        foreach (var item in _scrollObject)
        {
            if ((item.scrollDatasIndex >= 0) && (item.scrollDatasIndex < _scrollDatas.Count))
            {
                action(item.displayObject, item.scrollDatasIndex, _scrollDatas[item.scrollDatasIndex].data);
            }
        }
    }

    protected bool VerticalSizeResize(int index,float size)
    {
        if ((index >= 0) && (index < _scrollDatas.Count))
        {
            _scrollDatas[index] = (
                _scrollDatas[index].data,
                _scrollDatas[index].displayGObjectIndex,
                size);

            ContentAreaReckoning();

            //���ݕ\�����Ă���I�u�W�F�N�g�B�ɔ��f������B
            DisplayObjectAccess((S displayObject, int index, D data) =>
            {
                Vector2 vector2 = _instantiateDisplayGameObjects[_scrollDatas[index].displayGObjectIndex].anchoredPosition;
                vector2.y = GetElementPosition_y(index);
                displayObject.gameObject.GetComponent<RectTransform>().anchoredPosition = vector2;
            });
            
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// GameObject���܂ޑS�Ă̗v�f���폜����B
    /// </summary>
    protected void ScrollClear()
    {
        _instantiateDisplayGameObjects = new RectTransform[0];

        _scrollDatas = new List<(D data, int displayGObjectIndex, float verticalSize)>();

        DisplayObjectWhenRedisplayedDelegate = default;

        DisplayObjectBeforeHidingDelegate = default;

        ContentAreaReckoning();

        DisplayGameObjectsInitialSettingOrReset();
    }

    /// <summary>
    /// base.Update();�ŌĂяo���ĂˁB
    /// </summary>
    protected virtual void Update()
    {
        //�T�C�Y�X�V���ゾ�Ə�肭���f����Ȃ��̂Ő��t���[���҂B
        if (_barResetCount == 0)
        {
            _scrollbar.value = 1;
            _barResetCount = -1;
        }
        if (_barResetCount > 0) _barResetCount--;
    }

    private void ContentAreaReckoning()
    {
        float viewportHeight = _scrollRect.viewport.rect.height;

        float verticalSize = GetContentSize();

        //content�̍����͍Œ���r���[�|�[�g�Ɠ����x�Ȃ��ƕ\���A�C�e���̈ʒu���s���R(�������)�ɂȂ�̂ő΍�B
        if (verticalSize < viewportHeight) verticalSize = viewportHeight;
        _contentArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalSize);

        //DisplayGameObject�̍ő�`�搔�B
        float minimumSize =int.MaxValue;
        foreach (var item in _scrollDatas)
        {
            if (item.verticalSize < minimumSize) minimumSize = item.verticalSize;
        }
        float elementSize = minimumSize + _space;
        if (elementSize <= 0)//�G�ȑΏ��Ȃ̂Ō�ŏC���B
        {
            _maxNumOfDisplay = 0;
        }
        else
        {
          _maxNumOfDisplay = (int)Math.Ceiling(viewportHeight / elementSize) + 2;
        }

        float GetContentSize()
        {
            float returnSize = 0;

            returnSize += _margin * 2;

            returnSize += _space * (_scrollDatas.Count - 1);

            foreach (var item in _scrollDatas)
            {
                returnSize += item.verticalSize;
            }

            return (returnSize > 0)? returnSize:0;
        }
    }

    private void DisplayGameObjectsInitialSettingOrReset()
    {
        if (_maxNumOfDisplay < 0) _maxNumOfDisplay = 0;

        //�O��GameObject��S�č폜
        foreach (var item in _scrollObject)
        {
            Destroy(item.displayObject.gameObject);
        }
        _scrollObject.Clear();
        foreach (var stackList in _stockpileGameObjects)
        {
            foreach (var go in stackList)
            {
                Destroy(go.gameObject);
            }
        }
        _stockpileGameObjects.Clear();

        //�����ݒ�B
        foreach (var item in _instantiateDisplayGameObjects)
        {
            _stockpileGameObjects.Add(new Stack<S>());
        }

        //DisplayObject�̐����Ƃ��̏����ݒ�B
        for (int i = 0; i <= _maxNumOfDisplay; i++)
        {
            if (i >= _scrollDatas.Count) break;

            S s = Instantiate(_instantiateDisplayGameObjects[_scrollDatas[i].displayGObjectIndex], _contentArea).GetComponent<S>();

            //GameObject�̏����ݒ�
            s.gameObject.SetActive(true);

            var refData = _scrollDatas[i].data;

            DisplayObjectWhenRedisplayedDelegate(s, i, ref refData);

            //�A���ė���data�̔��f
            _scrollDatas[i] = (
                refData,
                _scrollDatas[i].displayGObjectIndex,
                _scrollDatas[i].verticalSize
                );

            //�ʒu
            Vector2 vector2 = _instantiateDisplayGameObjects[_scrollDatas[i].displayGObjectIndex].anchoredPosition;
            vector2.y = GetElementPosition_y(i);
            s.gameObject.GetComponent<RectTransform>().anchoredPosition = vector2;

            _scrollObject.AddLast((s,i));
        }
    }
    private float GetElementPosition_y(int index)
    {
        float returnPosition = _margin;

        for (int i = 0; i < index; i++)
        {
            if(i >= _scrollDatas.Count)return -returnPosition;

            returnPosition += (_scrollDatas[i].verticalSize + _space);
        }

            return -returnPosition;
    }

}
