using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject item;     //��Ʒ���
        [Range(0,1)]                           
        public float weight;        //������ʣ���Χ0-1
    }
    public LootItem[] lootItems;    //��Ʒ����

    //������Ʒ���ɷ���
    public void SpawnLoot()
    {
        float currentValue = Random.value;  //�������ֵ

        for (int i = 0; i < lootItems.Length; i++)
        {
            if (currentValue <= lootItems[i].weight)    //�ڸ����ڵ�����Ʒ
            {
                GameObject obj = Instantiate(lootItems[i].item);                //������Ʒ
                obj.transform.position = transform.position + Vector3.up * 2;   //��������λ��
                break;
            }
        }
    }
}
