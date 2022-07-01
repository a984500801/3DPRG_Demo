using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler,IPointerDownHandler
{
    RectTransform rectTransform;    //坐标
    Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();              //获取当前坐标
        canvas = InventoryManager.Instance.GetComponent<Canvas>();  //获取Canvas组件
    }

    //拖拽接口
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; //实现鼠标拖拽面板，同时要适配画布
        //Debug.Log(rectTransform.GetSiblingIndex());
    }

    //点击接口
    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetSiblingIndex(2);//设置同级索引，点按将面板显示在最前
    }
}
