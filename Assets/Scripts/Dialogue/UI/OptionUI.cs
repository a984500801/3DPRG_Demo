using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    //Button����ҪԪ��
    public Text optionText;
    private Button thisButton;
    private DialoguePiece currentPiece;     //ѡ����ƥ��ĶԻ���
    private bool takeQuest;                 //�������񲼶�ֵ

    private string nextPieceID;             //Ŀ��Ի�ID

    void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);    //��ѡ�ť��ӵ���¼�
    }

    //��ʾѡ�ť����
    public void UpdateOption(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece;                  //�����Ӧ�ĶԻ�������
        optionText.text = option.text;         //��ʾѡ���ı�
        nextPieceID = option.targetID;         //��ȡ��ѡ���Ŀ��ID
        takeQuest = option.takeQuest;
    }

    //�������
    public void OnOptionClicked()
    {
        //�жϵ�ǰ�Ի��Ƿ�������
        if (currentPiece.quest != null)
        {
            var newTask = new QuestManager.QuestTask();
            newTask.questData = Instantiate(currentPiece.quest);

            if (takeQuest)
            {
                //�ж��Ƿ���������
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    //�ж������Ƿ���ɸ��轱��
                    if (QuestManager.Instance.GetTask(newTask.questData).IsComplete)
                    {
                        //�۳���Ʒ�����轱��,���������
                        newTask.questData.GiveRewards();
                        QuestManager.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                else
                {
                    //�����������б�,����������
                    QuestManager.Instance.tasks.Add(newTask);
                    //����ʼ
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true;

                    foreach (var requireItem in newTask.questData.RequireTargetName())
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem);
                    }
                }
            }
        }

        if (nextPieceID == "") //Ŀ��IDΪ��
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);  //�رնԻ�����
            return;
        }
        else
        {
            //���öԻ���ʾ���������ֵ��в��Ҷ�ӦID�ĶԻ������ݴ���
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceID]);
        }
    }
}
