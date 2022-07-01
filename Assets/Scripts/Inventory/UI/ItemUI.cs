using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    //物品图标、数量、数据
    public Image icon = null;
    public Text amount = null;
    public ItemData_SO currentItemData;

    //背包属性，可读可写
    public InventoryData_SO Bag { get; set; }
    //背包数据序号
    public int Index { get; set; } = -1;

    //UI更新
    public void SetupItemUI(ItemData_SO item,int itemAmount)
    {
        if (itemAmount == 0)//物品数量为0
        {
            Bag.items[Index].itemData = null;//对应物品数据清空
            icon.gameObject.SetActive(false);//关闭物品图标
            return;
        }

        //任务面板里避免显示任务需求的物品
        if (itemAmount < 0)
            item = null;

        //不为空背包显示物品与数量
        if (item != null)
        {
            currentItemData = item;
            icon.sprite = item.itemIcon;        //图标
            amount.text = itemAmount.ToString();//数量UI
            icon.gameObject.SetActive(true);//物体激活
        }
        else
        {
            icon.gameObject.SetActive(false);//物品保持不激活
        }
    }

    public ItemData_SO GetItem()//获得物品数据
    {
        return Bag.items[Index].itemData;//返回当前UI在背包中对应索引的物品数据
    }
}
