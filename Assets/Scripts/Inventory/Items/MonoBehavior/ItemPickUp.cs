using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    //������Ʒ����
    public ItemData_SO itemData;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.inventoryData.AddItem(itemData,itemData.itemAmount);//����Ʒ��ӵ�����
            InventoryManager.Instance.inventoryUI.RefreshUI();//����Ʒˢ�±���UI

            //װ������
            //GameManager.Instance.playerStats.EquipWeapon(itemData);
            //GameManager.Instance.playerStats.EquipShield(itemData);

            //����Ƿ�������
            QuestManager.Instance.UpdateQuestProgress(itemData.itemName, itemData.itemAmount);

            Destroy(gameObject);
        }
    }
}
