using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����˵�
[CreateAssetMenu(fileName ="New Inventory",menuName ="Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    //����������Ŀ�б�
    public List<InventoryItem> items = new List<InventoryItem>();

    //�����Ʒ����
    public void AddItem(ItemData_SO newItemData,int amount)
    {
        //�ҵ�ͬ����Ʒ����ֵ
        bool found = false;

        //�Ƿ�ɶѵ���Ʒ
        if (newItemData.stackable)
        {
            //�����б�
            foreach (var item in items)
            {
                //��Ʒ���
                if (item.itemData = newItemData)
                {
                    //��������
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }

        
        for (int i = 0; i < items.Count; i++)
        {
            //�����ǿյģ�����û���ҵ���ͬ��Ʒ
            if (items[i].itemData == null && !found)
            {
                //��������
                items[i].itemData = newItemData;
                //��������
                items[i].amount = amount;
                break;
            }
        }
    }
}

//����������
[System.Serializable]
public class InventoryItem
{
    //��������
    public ItemData_SO itemData;
    //����
    public int amount;
}
