using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//创建菜单
[CreateAssetMenu(fileName ="New Inventory",menuName ="Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    //创建背包项目列表
    public List<InventoryItem> items = new List<InventoryItem>();

    //添加物品方法
    public void AddItem(ItemData_SO newItemData,int amount)
    {
        //找到同类物品布尔值
        bool found = false;

        //是否可堆叠物品
        if (newItemData.stackable)
        {
            //遍历列表
            foreach (var item in items)
            {
                //物品相等
                if (item.itemData = newItemData)
                {
                    //更新数量
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }

        
        for (int i = 0; i < items.Count; i++)
        {
            //格子是空的，并且没有找到相同物品
            if (items[i].itemData == null && !found)
            {
                //加入数据
                items[i].itemData = newItemData;
                //更新数量
                items[i].amount = amount;
                break;
            }
        }
    }
}

//背包物体类
[System.Serializable]
public class InventoryItem
{
    //物体数据
    public ItemData_SO itemData;
    //数量
    public int amount;
}
