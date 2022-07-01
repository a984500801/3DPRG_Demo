using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;     //������������
    bool canTalk = false;                   //�Ի�Ϊfalse

    //��ɫ��ײ�������ԶԻ�
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }
    }

    //��ɫ�뿪�Ի��رս���
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            canTalk = false;
        }
    }

    void Update()
    {
        //��������ͬʱ����G��ִ�жԻ�
        if (canTalk && Input.GetKeyDown(KeyCode.G))
        {
            OpenDialogue();
        }
    }

    //��UI���
    void OpenDialogue()
    {
        //����Ի�����
        DialogueUI.Instance.UpdateDialogueData(currentData);
        //�򿪶Ի������ʾ��һ���Ի�
        DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }
}
