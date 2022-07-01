using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject item;     //物品组件
        [Range(0,1)]                           
        public float weight;        //掉落概率，范围0-1
    }
    public LootItem[] lootItems;    //物品数组

    //掉落物品生成方法
    public void SpawnLoot()
    {
        float currentValue = Random.value;  //产生随机值

        for (int i = 0; i < lootItems.Length; i++)
        {
            if (currentValue <= lootItems[i].weight)    //在概率内掉落物品
            {
                GameObject obj = Instantiate(lootItems[i].item);                //生成物品
                obj.transform.position = transform.position + Vector3.up * 2;   //设置坐标位置
                break;
            }
        }
    }
}
