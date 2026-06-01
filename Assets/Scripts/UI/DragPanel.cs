using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler, IPointerDownHandler  //第一个接口用于拖拽UI窗口，第二个接口用于检测鼠标的点按
{
    RectTransform rectTransform;
    Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas=InventoryManager.Instance.GetComponent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition+=eventData.delta/canvas.scaleFactor;   //根据鼠标拖动的距离更新面板的位置(scaleFactor可以缩放整个画布，同时仍使其适合屏幕。防止UI元素位置偏移)

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetSiblingIndex(2);    //这个函数可以改变该物体的顺序序号（0，1，2，，），当点按stats或者inventory是可以把他放在对方的上面
    }
}
