using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialoguePiece
{
    
    public string ID;           //语句
    public Sprite image;        //对话头像
    [TextArea]
    public string text;         //对话内容，TextArea文本区域
    public QuestData_SO quest;  //

    public List<DialogueOption> options = new List<DialogueOption>();   //对话的选项
}
