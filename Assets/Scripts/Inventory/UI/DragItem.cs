using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    ItemUI currentItemUI;//��Ʒ
    SlotHolder currentHolder;//������
    SlotHolder targetHolder;//Ŀ�걳����

    void Awake()
    {
        //��ȡ���
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();
    }

    //��ʼ��ק����¼ԭʼ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();                    //������ק������ʱ��¼
        InventoryManager.Instance.currentDrag.originalHolder = GetComponentInParent<SlotHolder>();  //��¼��ʼ������
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;     //��¼��ʼ���RectTransform
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);                  //���ø���Ϊ��ʱ������ʵ�������ϲ���ʾ
    }
    
    //��ק��
    public void OnDrag(PointerEventData eventData)
    {
        //�������λ���ƶ�
        transform.position = eventData.position;
    }

    //������Ʒ������ק����������
    public void OnEndDrag(PointerEventData eventData)
    {
        
        //�Ƿ�ָ��UI��Ʒ
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //�����ж��Ƿ��ڱ�����Χ��
            if (InventoryManager.Instance.CheckInActionUI(eventData.position) || InventoryManager.Instance.CheckInInventoryUI(eventData.position) || InventoryManager.Instance.CheckInEquipmentUI(eventData.position))
            {
                //ָ�����������Ƿ��б�����
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();            //���Ŀ�걳����
                else
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();    //��ȡ����������Ʒ�ĸ������õ�������
                

                //�ж�Ŀ��holder�Ƿ���ԭholder
                if (targetHolder != InventoryManager.Instance.currentDrag.originalHolder)
                {
                    switch (targetHolder.slotType)//�����������ж�
                    {
                        case SlotType.BAG:
                            SwapItem();
                            break;
                        case SlotType.WEAPON:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)//�ж���Ʒ����ʵ�ֲ�ͬ���������Ʒ��ק
                                SwapItem();
                            break;
                        case SlotType.SHIELD:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Shield)
                                SwapItem();
                            break;
                        case SlotType.ACTION:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                                SwapItem();
                            break;
                    }
                }
                //��ק��������������������
                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);//�Ѹ�������Ϊ�洢��ԭʼ��������

        //����RectTransform��offsetȷ��ͼƬ����ȷλ����ʾ
        RectTransform t = transform as RectTransform;
        t.offsetMax = -Vector2.one * 5;
        t.offsetMin = Vector2.one * 5;
    }

    //������Ʒ
    public void SwapItem()
    {
        var targetItem = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];//��ȡĿ�걳�����е���Ʒ���䱳���е����
        var tempItem = currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];//��ק��Ʒ���䱳���е����

        bool isSameItem = tempItem.itemData == targetItem.itemData;//����������Ʒ�Ƿ�����ͬ��Ʒ�Ĳ���ֵ

        //�ж���Ʒ�Ƿ���ͬ�ҿɶѵ�
        if (isSameItem && targetItem.itemData.stackable)
        {
            targetItem.amount += tempItem.amount;//�������
            tempItem.itemData = null;           //��ק��Ʒ���
            tempItem.amount = 0;                //��ק��Ʒ��������
        }
        else
        {
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItem;//Ŀ����Ʒ��ֵ��ԭʼ������
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;//��ק��Ʒ��ֵ��Ŀ�걳����
        }
    }
}
