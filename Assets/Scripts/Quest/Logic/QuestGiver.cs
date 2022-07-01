using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class QuestGiver : MonoBehaviour
{
    DialogueController controller;//�Ի��������
    QuestData_SO currentQuest;//��ǰ����Data

    public DialogueData_SO startDialogue;//��ʼ�Ի�
    public DialogueData_SO progressDialogue;//��������жԻ�
    public DialogueData_SO completeDialogue;//�������ʱ�Ի�
    public DialogueData_SO finishDialogue;//��������ɺ�ĶԻ�

    #region �������״̬
    public bool IsStarted//�ж������Ƿ�ʼ
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

    public bool IsComplete//�ж������Ƿ����
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

    public bool IsFinished//�ж������Ƿ���ɲ��ύ
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
        if (IsStarted)//����ʼ��
        {
            if (IsComplete)//�����Ƿ����
            {
                controller.currentData = completeDialogue;
            }
            else
            {
                controller.currentData = progressDialogue;
            }  
        }

        if (IsFinished)//�����Ƿ���ɲ��ύ��
        {
            controller.currentData = finishDialogue;
        }
    }
}
