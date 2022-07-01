using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialoguePiece
{
    
    public string ID;           //���
    public Sprite image;        //�Ի�ͷ��
    [TextArea]
    public string text;         //�Ի����ݣ�TextArea�ı�����
    public QuestData_SO quest;  //

    public List<DialogueOption> options = new List<DialogueOption>();   //�Ի���ѡ��
}
