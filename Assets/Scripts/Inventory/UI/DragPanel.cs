using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler,IPointerDownHandler
{
    RectTransform rectTransform;    //����
    Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();              //��ȡ��ǰ����
        canvas = InventoryManager.Instance.GetComponent<Canvas>();  //��ȡCanvas���
    }

    //��ק�ӿ�
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; //ʵ�������ק��壬ͬʱҪ���仭��
        //Debug.Log(rectTransform.GetSiblingIndex());
    }

    //����ӿ�
    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetSiblingIndex(2);//����ͬ���������㰴�������ʾ����ǰ
    }
}
