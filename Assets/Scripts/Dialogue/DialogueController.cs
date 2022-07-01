using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;     //引用任务数据
    bool canTalk = false;                   //对话为false

    //角色碰撞触发可以对话
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }
    }

    //角色离开对话关闭界面
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
        //可以聊天同时按下G键执行对话
        if (canTalk && Input.GetKeyDown(KeyCode.G))
        {
            OpenDialogue();
        }
    }

    //打开UI面板
    void OpenDialogue()
    {
        //传输对话内容
        DialogueUI.Instance.UpdateDialogueData(currentData);
        //打开对话面板显示第一条对话
        DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }
}
