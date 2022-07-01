using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 任务数据
/// </summary>
[CreateAssetMenu(fileName="New Quest",menuName ="Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    //任务需要目标
    public class QuestRequire
    {
        public string name;//目标名(物品名或者怪物名)
        public int requireAmount;//目标数量
        public int currentAmount;//当前数量
    }

    public string questName;//任务名
    [TextArea]
    public string description;//任务描述

    //任务状态
    public bool isStarted;//开始任务
    public bool isComplete;//任务目标完成
    public bool isFinished;//完成任务且获得奖励

    //任务目标列表
    public List<QuestRequire> questRequires = new List<QuestRequire>();
    public List<InventoryItem> rewards = new List<InventoryItem>();

    //检查任务进度是否满足需求
    public void CheckQuestProgress()
    {
        var finishRequires = questRequires.Where(r => r.requireAmount <= r.currentAmount);
        isComplete = finishRequires.Count() == questRequires.Count;

        if (isComplete)
            Debug.Log("任务完成");
    }

    //当前任务需要  收集/消灭 的目标名字
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();
        foreach (var require in questRequires)
        {
            targetNameList.Add(require.name);
        }
        return targetNameList;
    }

    //任务完成给予奖励
    public void GiveRewards()
    {
        foreach (var reward in rewards)
        {
            if (reward.amount < 0)//需要上交任务物品
            {
                int requireCount = Mathf.Abs(reward.amount);

                //判断背包里是否有任务需求物品
                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null)
                {
                    //背包物品数量够不够
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)
                    {
                        //背包数量刚好和需求一样，直接扣除，背包里的数量不够，扣除背包物品的数量后从快捷栏扣除
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;
                        //快捷栏里有，扣除剩下需要的物品数
                        if (InventoryManager.Instance.QuestItemInAction(reward.itemData) != null)
                        {
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                        }
                    }
                    else
                    {
                        //背包里的数量大于需求数，直接从背包扣除相应数量的物品
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }
                else
                {
                    //背包里是空的，没有任务物品，直接从快捷栏扣除
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                }
            }
            else
            {
                //给予物品奖励
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData, reward.amount);
            }

            //更新背包和action
            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
    }
}
