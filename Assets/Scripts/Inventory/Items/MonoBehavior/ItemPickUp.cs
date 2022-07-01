using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    //附加物品数据
    public ItemData_SO itemData;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.inventoryData.AddItem(itemData,itemData.itemAmount);//将物品添加到背包
            InventoryManager.Instance.inventoryUI.RefreshUI();//捡到物品刷新背包UI

            //装备武器
            //GameManager.Instance.playerStats.EquipWeapon(itemData);
            //GameManager.Instance.playerStats.EquipShield(itemData);

            //检查是否有任务
            QuestManager.Instance.UpdateQuestProgress(itemData.itemName, itemData.itemAmount);

            Destroy(gameObject);
        }
    }
}
