using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    //背包格数组
    public SlotHolder[] slotHolders;

    //刷新UI
    public void RefreshUI()
    {
        //遍历背包格
        for (int i = 0; i < slotHolders.Length; i++)
        {
            slotHolders[i].itemUI.Index = i;//获取序号
            slotHolders[i].UpdateItem();//更新物品
        }
    }
}
