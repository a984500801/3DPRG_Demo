using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    //����������
    public SlotHolder[] slotHolders;

    //ˢ��UI
    public void RefreshUI()
    {
        //����������
        for (int i = 0; i < slotHolders.Length; i++)
        {
            slotHolders[i].itemUI.Index = i;//��ȡ���
            slotHolders[i].UpdateItem();//������Ʒ
        }
    }
}