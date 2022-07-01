using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    //Button的主要元素
    public Text optionText;
    private Button thisButton;
    private DialoguePiece currentPiece;     //选项所匹配的对话条
    private bool takeQuest;                 //接受任务布尔值

    private string nextPieceID;             //目标对话ID

    void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);    //给选项按钮添加点击事件
    }

    //显示选项按钮内容
    public void UpdateOption(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece;                  //传入对应的对话条数据
        optionText.text = option.text;         //显示选项文本
        nextPieceID = option.targetID;         //获取到选项的目标ID
        takeQuest = option.takeQuest;
    }

    //点击方法
    public void OnOptionClicked()
    {
        //判断当前对话是否有任务
        if (currentPiece.quest != null)
        {
            var newTask = new QuestManager.QuestTask();
            newTask.questData = Instantiate(currentPiece.quest);

            if (takeQuest)
            {
                //判断是否已有任务
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    //判断任务是否完成给予奖励
                    if (QuestManager.Instance.GetTask(newTask.questData).IsComplete)
                    {
                        //扣除物品，给予奖励,任务已完成
                        newTask.questData.GiveRewards();
                        QuestManager.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                else
                {
                    //添加新任务进列表,接受新任务
                    QuestManager.Instance.tasks.Add(newTask);
                    //任务开始
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true;

                    foreach (var requireItem in newTask.questData.RequireTargetName())
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem);
                    }
                }
            }
        }

        if (nextPieceID == "") //目标ID为空
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);  //关闭对话窗口
            return;
        }
        else
        {
            //调用对话显示方法，在字典中查找对应ID的对话条数据传递
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceID]);
        }
    }
}
