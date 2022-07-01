using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//包类型
public enum SlotType
{
    BAG,WEAPON,SHIELD,ACTION
}

public class SlotHolder : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    //类型
    public SlotType slotType;
    //背包格
    public ItemUI itemUI;

    //鼠标点击接口
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)//双击
        {
            UseItem();
        }
    }

    //使用物品
    public void UseItem()
    {
        if (itemUI.GetItem() != null)//背包格物品数据不为空可执行使用物品
        {
            if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.Bag.items[itemUI.Index].amount > 0)//判断物品类型和对应物品在背包里的物品数量大于0
            {
                GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableData.healthPoint);//调用物品功能方法
                itemUI.Bag.items[itemUI.Index].amount -= 1;//背包数量更新

                //检查任务物品更新进度
                QuestManager.Instance.UpdateQuestProgress(itemUI.GetItem().itemName, -1);
            }
        }
        //更新物品
        UpdateItem();
    }

    //更新物品数据
    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;//背包数据库
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;//装备栏数据库
                //装备武器，切换武器
                if (itemUI.Bag.items[itemUI.Index].itemData != null)
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }
                break;
            case SlotType.SHIELD:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                if (itemUI.Bag.items[itemUI.Index].itemData != null)
                {
                    GameManager.Instance.playerStats.ChangeShield(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipShield();
                }
                break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;//操作栏背包数据库
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];//获取对应背包数据对应序号的物品
        itemUI.SetupItemUI(item.itemData, item.amount);//传入物品数据和物品数量
    }

    //鼠标悬停进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())//物品存在
        {
            InventoryManager.Instance.tooltip.SetupTooltip(itemUI.GetItem());//传入提示信息数据
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);    //显示提示面板
        }
    }

    //鼠标退出悬停
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);        //关闭提示面板
    }

    void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);        //背包关闭进行提示面板关闭
    }
}
