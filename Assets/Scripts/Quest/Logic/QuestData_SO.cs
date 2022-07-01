using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ��������
/// </summary>
[CreateAssetMenu(fileName="New Quest",menuName ="Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    //������ҪĿ��
    public class QuestRequire
    {
        public string name;//Ŀ����(��Ʒ�����߹�����)
        public int requireAmount;//Ŀ������
        public int currentAmount;//��ǰ����
    }

    public string questName;//������
    [TextArea]
    public string description;//��������

    //����״̬
    public bool isStarted;//��ʼ����
    public bool isComplete;//����Ŀ�����
    public bool isFinished;//��������һ�ý���

    //����Ŀ���б�
    public List<QuestRequire> questRequires = new List<QuestRequire>();
    public List<InventoryItem> rewards = new List<InventoryItem>();

    //�����������Ƿ���������
    public void CheckQuestProgress()
    {
        var finishRequires = questRequires.Where(r => r.requireAmount <= r.currentAmount);
        isComplete = finishRequires.Count() == questRequires.Count;

        if (isComplete)
            Debug.Log("�������");
    }

    //��ǰ������Ҫ  �ռ�/���� ��Ŀ������
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();
        foreach (var require in questRequires)
        {
            targetNameList.Add(require.name);
        }
        return targetNameList;
    }

    //������ɸ��轱��
    public void GiveRewards()
    {
        foreach (var reward in rewards)
        {
            if (reward.amount < 0)//��Ҫ�Ͻ�������Ʒ
            {
                int requireCount = Mathf.Abs(reward.amount);

                //�жϱ������Ƿ�������������Ʒ
                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null)
                {
                    //������Ʒ����������
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)
                    {
                        //���������պú�����һ����ֱ�ӿ۳���������������������۳�������Ʒ��������ӿ�����۳�
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;
                        //��������У��۳�ʣ����Ҫ����Ʒ��
                        if (InventoryManager.Instance.QuestItemInAction(reward.itemData) != null)
                        {
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                        }
                    }
                    else
                    {
                        //�����������������������ֱ�Ӵӱ����۳���Ӧ��������Ʒ
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }
                else
                {
                    //�������ǿյģ�û��������Ʒ��ֱ�Ӵӿ�����۳�
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                }
            }
            else
            {
                //������Ʒ����
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData, reward.amount);
            }

            //���±�����action
            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
    }
}
