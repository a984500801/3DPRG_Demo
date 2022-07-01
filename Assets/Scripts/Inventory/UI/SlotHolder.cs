using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//������
public enum SlotType
{
    BAG,WEAPON,SHIELD,ACTION
}

public class SlotHolder : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    //����
    public SlotType slotType;
    //������
    public ItemUI itemUI;

    //������ӿ�
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)//˫��
        {
            UseItem();
        }
    }

    //ʹ����Ʒ
    public void UseItem()
    {
        if (itemUI.GetItem() != null)//��������Ʒ���ݲ�Ϊ�տ�ִ��ʹ����Ʒ
        {
            if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.Bag.items[itemUI.Index].amount > 0)//�ж���Ʒ���ͺͶ�Ӧ��Ʒ�ڱ��������Ʒ��������0
            {
                GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableData.healthPoint);//������Ʒ���ܷ���
                itemUI.Bag.items[itemUI.Index].amount -= 1;//������������

                //���������Ʒ���½���
                QuestManager.Instance.UpdateQuestProgress(itemUI.GetItem().itemName, -1);
            }
        }
        //������Ʒ
        UpdateItem();
    }

    //������Ʒ����
    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;//�������ݿ�
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;//װ�������ݿ�
                //װ���������л�����
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
                itemUI.Bag = InventoryManager.Instance.actionData;//�������������ݿ�
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];//��ȡ��Ӧ�������ݶ�Ӧ��ŵ���Ʒ
        itemUI.SetupItemUI(item.itemData, item.amount);//������Ʒ���ݺ���Ʒ����
    }

    //�����ͣ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())//��Ʒ����
        {
            InventoryManager.Instance.tooltip.SetupTooltip(itemUI.GetItem());//������ʾ��Ϣ����
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);    //��ʾ��ʾ���
        }
    }

    //����˳���ͣ
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);        //�ر���ʾ���
    }

    void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);        //�����رս�����ʾ���ر�
    }
}
