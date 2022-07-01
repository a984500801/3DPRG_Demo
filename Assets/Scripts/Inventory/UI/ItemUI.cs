using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    //��Ʒͼ�ꡢ����������
    public Image icon = null;
    public Text amount = null;
    public ItemData_SO currentItemData;

    //�������ԣ��ɶ���д
    public InventoryData_SO Bag { get; set; }
    //�����������
    public int Index { get; set; } = -1;

    //UI����
    public void SetupItemUI(ItemData_SO item,int itemAmount)
    {
        if (itemAmount == 0)//��Ʒ����Ϊ0
        {
            Bag.items[Index].itemData = null;//��Ӧ��Ʒ�������
            icon.gameObject.SetActive(false);//�ر���Ʒͼ��
            return;
        }

        //��������������ʾ�����������Ʒ
        if (itemAmount < 0)
            item = null;

        //��Ϊ�ձ�����ʾ��Ʒ������
        if (item != null)
        {
            currentItemData = item;
            icon.sprite = item.itemIcon;        //ͼ��
            amount.text = itemAmount.ToString();//����UI
            icon.gameObject.SetActive(true);//���弤��
        }
        else
        {
            icon.gameObject.SetActive(false);//��Ʒ���ֲ�����
        }
    }

    public ItemData_SO GetItem()//�����Ʒ����
    {
        return Bag.items[Index].itemData;//���ص�ǰUI�ڱ����ж�Ӧ��������Ʒ����
    }
}
