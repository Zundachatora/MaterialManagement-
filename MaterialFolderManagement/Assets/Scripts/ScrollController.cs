using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController<D,S> : MonoBehaviour where S : MonoBehaviour
{
    //自分で用意するデータ Content(サイズ変更) scrollbar(バー位置のリセット) scrollRect(移動情報) 空白(隙間) 上と下の余白
    //相手から欲しいデータ ScrollGameObject ScrollData 縦のSize 表示される際の処理(デリゲート)

    public delegate S DisplayItemWhenRedisplayed(S displayObject, int index,ref D data);
    public delegate S DisplayObjectBeforeHiding(S displayObject, int index,ref D data);
    public DisplayItemWhenRedisplayed DisplayObjectWhenRedisplayedDelegate = null;
    public DisplayObjectBeforeHiding DisplayObjectBeforeHidingDelegate = null;

    /// <summary>SetScrollData関数を実行時にバーを上に戻すかどうか</summary>
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

    /// <summary>スクロール時に呼び出す</summary>
    /// <param name="SRNedPosition"></param>
    public void ScrollOnValueChanged(Vector2 SRNedPosition)//ScrollRect.normalizedPosition
    {
        if (_scrollDatas.Count == 0) return;

        //位置計算
        //スクロールされた量(現在描画位置)
        float scrollPosition = (_contentArea.rect.height - _scrollRect.viewport.rect.height) * (1 - _scrollRect.normalizedPosition.y);

        //現在描画されている最も高い位置にあるインデックス
        int topIndex = 0;
        float remainingHeight = scrollPosition - _margin;
        foreach (var item in _scrollDatas)
        {

            remainingHeight -= item.verticalSize + _space;

            if (remainingHeight <= 0) break;

            topIndex++;
        }
        if (topIndex >= _scrollDatas.Count) topIndex = (_scrollDatas.Count - 1);

        //DisplayObjectがtopIndexを下回るとき末尾に新しいDisplayObjectを追加
        while (true)
        {
            if (_scrollObject.First.Value.scrollDatasIndex < topIndex)
            {
                LinkedListNode<(S displayObject, int scrollDatasIndex)> displayItem = _scrollObject.First;

                //非表示時の処理
                if (DisplayObjectBeforeHidingDelegate != null)
                {
                    var refData = _scrollDatas[displayItem.Value.scrollDatasIndex].data;
                    DisplayObjectBeforeHidingDelegate(
                        displayItem.Value.displayObject,
                        displayItem.Value.scrollDatasIndex,
                        ref refData
                            );

                    //帰って来たdataの反映
                    _scrollDatas[displayItem.Value.scrollDatasIndex] = (
                        refData,
                        _scrollDatas[displayItem.Value.scrollDatasIndex].displayGObjectIndex,
                        _scrollDatas[displayItem.Value.scrollDatasIndex].verticalSize
                        );
                }

                //DisplayObject内のデータを更新。
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

                    //帰って来たdataの反映
                    _scrollDatas[addIndex] = (
                        refData,
                        _scrollDatas[addIndex].displayGObjectIndex,
                        _scrollDatas[addIndex].verticalSize
                        );

                    displayItem.Value.displayObject.gameObject.SetActive(true);
                }
                else
                {//DataのIndex外の場合は処理しない。
                    break;
                }

                //表示位置
                Vector2 vector2 = _instantiateDisplayGameObjects[0].anchoredPosition;//後で修正
                vector2.y = GetElementPosition_y(displayItem.Value.scrollDatasIndex);
                displayItem.Value.displayObject.gameObject.GetComponent<RectTransform>().anchoredPosition = vector2;

                //末尾に移動。
                _scrollObject.RemoveFirst();
                _scrollObject.AddLast(displayItem);

            }
            else
            {
                break;
            }
        }


        //topIndex+(最大描画数-1)を上回る場合は先頭にDisplayObjectを追加
        while (true)
        {
            if (_scrollObject.Last.Value.scrollDatasIndex > topIndex + (_maxNumOfDisplay - 1))
            {
                LinkedListNode<(S displayObject, int scrollDatasIndex)> displayItem = _scrollObject.Last;

                //非表示時の処理
                if (DisplayObjectBeforeHidingDelegate != null)
                {
                    var refData = _scrollDatas[displayItem.Value.scrollDatasIndex].data;
                    DisplayObjectBeforeHidingDelegate(
                        displayItem.Value.displayObject,
                        displayItem.Value.scrollDatasIndex,
                        ref refData
                            );

                    //帰って来たdataの反映
                    _scrollDatas[displayItem.Value.scrollDatasIndex] = (
                        refData,
                        _scrollDatas[displayItem.Value.scrollDatasIndex].displayGObjectIndex,
                        _scrollDatas[displayItem.Value.scrollDatasIndex].verticalSize
                        );
                }

                //DisplayObject内のデータを更新。
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

                    //帰って来たdataの反映
                    _scrollDatas[addIndex] = (
                        refData,
                        _scrollDatas[addIndex].displayGObjectIndex,
                        _scrollDatas[addIndex].verticalSize
                        );

                    displayItem.Value.displayObject.gameObject.SetActive(true);
                }
                else
                {//DataのIndex外の場合は処理しない。
                    break;
                }

                //表示位置
                Vector2 vector2 = _instantiateDisplayGameObjects[0].anchoredPosition;//後で修正
                vector2.y = GetElementPosition_y(displayItem.Value.scrollDatasIndex);
                displayItem.Value.displayObject.gameObject.GetComponent<RectTransform>().anchoredPosition = vector2;

                //先頭に移動
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
    /// 変更する際は既存の配列と同じ長さである必要がある。
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
    /// 引数:Action(S, index, D)
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

            //現在表示しているオブジェクト達に反映させる。
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
    /// GameObjectを含む全ての要素を削除する。
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
    /// base.Update();で呼び出してね。
    /// </summary>
    protected virtual void Update()
    {
        //サイズ更新直後だと上手く反映されないので数フレーム待つ。
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

        //contentの高さは最低限ビューポートと同程度ないと表示アイテムの位置が不自然(中央より)になるので対策。
        if (verticalSize < viewportHeight) verticalSize = viewportHeight;
        _contentArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalSize);

        //DisplayGameObjectの最大描画数。
        float minimumSize =int.MaxValue;
        foreach (var item in _scrollDatas)
        {
            if (item.verticalSize < minimumSize) minimumSize = item.verticalSize;
        }
        float elementSize = minimumSize + _space;
        if (elementSize <= 0)//雑な対処なので後で修正。
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

        //前のGameObjectを全て削除
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

        //初期設定。
        foreach (var item in _instantiateDisplayGameObjects)
        {
            _stockpileGameObjects.Add(new Stack<S>());
        }

        //DisplayObjectの生成とその初期設定。
        for (int i = 0; i <= _maxNumOfDisplay; i++)
        {
            if (i >= _scrollDatas.Count) break;

            S s = Instantiate(_instantiateDisplayGameObjects[_scrollDatas[i].displayGObjectIndex], _contentArea).GetComponent<S>();

            //GameObjectの初期設定
            s.gameObject.SetActive(true);

            var refData = _scrollDatas[i].data;

            DisplayObjectWhenRedisplayedDelegate(s, i, ref refData);

            //帰って来たdataの反映
            _scrollDatas[i] = (
                refData,
                _scrollDatas[i].displayGObjectIndex,
                _scrollDatas[i].verticalSize
                );

            //位置
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
