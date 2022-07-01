using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    ItemUI currentItemUI;//物品
    SlotHolder currentHolder;//背包格
    SlotHolder targetHolder;//目标背包格

    void Awake()
    {
        //获取组件
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();
    }

    //开始拖拽，记录原始数据
    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();                    //创建拖拽数据临时记录
        InventoryManager.Instance.currentDrag.originalHolder = GetComponentInParent<SlotHolder>();  //记录初始背包格
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;     //记录初始面板RectTransform
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);                  //设置父级为临时画布，实现在最上层显示
    }
    
    //拖拽中
    public void OnDrag(PointerEventData eventData)
    {
        //跟随鼠标位置移动
        transform.position = eventData.position;
    }

    //放下物品结束拖拽，交换数据
    public void OnEndDrag(PointerEventData eventData)
    {
        
        //是否指向UI物品
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //调用判断是否在背包格范围内
            if (InventoryManager.Instance.CheckInActionUI(eventData.position) || InventoryManager.Instance.CheckInInventoryUI(eventData.position) || InventoryManager.Instance.CheckInEquipmentUI(eventData.position))
            {
                //指针进入的物体是否有背包格
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();            //获得目标背包格
                else
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();    //获取背包格中物品的父级，拿到背包格
                

                //判断目标holder是否是原holder
                if (targetHolder != InventoryManager.Instance.currentDrag.originalHolder)
                {
                    switch (targetHolder.slotType)//背包格类型判断
                    {
                        case SlotType.BAG:
                            SwapItem();
                            break;
                        case SlotType.WEAPON:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)//判断物品类型实现不同背包格的物品拖拽
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
                //拖拽后更新两个背包格的数据
                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);//把父级设置为存储的原始父级画布

        //调整RectTransform的offset确保图片在正确位置显示
        RectTransform t = transform as RectTransform;
        t.offsetMax = -Vector2.one * 5;
        t.offsetMin = Vector2.one * 5;
    }

    //交换物品
    public void SwapItem()
    {
        var targetItem = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];//获取目标背包格中的物品在其背包中的序号
        var tempItem = currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];//拖拽物品在其背包中的序号

        bool isSameItem = tempItem.itemData == targetItem.itemData;//返回两个物品是否是相同物品的布尔值

        //判断物品是否相同且可堆叠
        if (isSameItem && targetItem.itemData.stackable)
        {
            targetItem.amount += tempItem.amount;//数量添加
            tempItem.itemData = null;           //拖拽物品清空
            tempItem.amount = 0;                //拖拽物品数量清零
        }
        else
        {
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItem;//目标物品赋值给原始背包格
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;//拖拽物品赋值给目标背包格
        }
    }
}
