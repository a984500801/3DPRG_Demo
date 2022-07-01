using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class QuestGiver : MonoBehaviour
{
    DialogueController controller;//对话控制组件
    QuestData_SO currentQuest;//当前任务Data

    public DialogueData_SO startDialogue;//开始对话
    public DialogueData_SO progressDialogue;//任务进行中对话
    public DialogueData_SO completeDialogue;//任务完成时对话
    public DialogueData_SO finishDialogue;//任务已完成后的对话

    #region 获得任务状态
    public bool IsStarted//判断任务是否开始
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetTask(currentQuest).IsStarted;
            }
            else return false;
        }
    }

    public bool IsComplete//判断任务是否完成
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetTask(currentQuest).IsComplete;
            }
            else return false;
        }
    }

    public bool IsFinished//判断任务是否完成并提交
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetTask(currentQuest).IsFinished;
            }
            else return false;
        }
    }
    #endregion

    void Awake()
    {
        controller = GetComponent<DialogueController>();
    }

    void Start()
    {
        controller.currentData = startDialogue;
        currentQuest = controller.currentData.GetQuest();
    }

    void Update()
    {
        if (IsStarted)//任务开始了
        {
            if (IsComplete)//任务是否完成
            {
                controller.currentData = completeDialogue;
            }
            else
            {
                controller.currentData = progressDialogue;
            }  
        }

        if (IsFinished)//任务是否完成并提交过
        {
            controller.currentData = finishDialogue;
        }
    }
}
