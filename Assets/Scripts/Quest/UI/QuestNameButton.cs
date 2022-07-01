using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNameButton : MonoBehaviour
{
    public Text questNameText;
    public QuestData_SO currentData;
    public Text questContentText;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }

    void UpdateQuestContent()
    {
        questContentText.text = currentData.description;
        QuestUI.Instance.SetUpRequireList(currentData);

        //先销毁所有奖励的物品
        foreach (Transform item in QuestUI.Instance.rewardTransform)
        {
            Destroy(item.gameObject);
        }
        //遍历任务的奖励列表物品
        foreach (var item in currentData.rewards)
        {
            QuestUI.Instance.SetupRewardItem(item.itemData, item.amount);
        }
    }

    public void SetUpNameButton(QuestData_SO questData)
    {
        currentData = questData;

        if (questData.isComplete)
            questNameText.text = questData.questName + "(完成)";
        else
            questNameText.text = questData.questName;
    }
}
