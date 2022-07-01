using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Dialogue",menuName ="Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();                              //�Ի�����б�
    public Dictionary<string, DialoguePiece> dialogueIndex = new Dictionary<string, DialoguePiece>();   //�ý�ID��Ի����������

#if UNITY_EDITOR
    void OnValidate()   //��Unity�༭�޸�ʱ���и���
    {
        dialogueIndex.Clear();                          //�޸�ʱ����ֵ�
        foreach (var piece in dialoguePieces)           //�����Ի������б�
        {
            if (!dialogueIndex.ContainsKey(piece.ID))   //�ֵ�δ������ӦID
                dialogueIndex.Add(piece.ID, piece);     //����Ӧ��ID��Ի����ݴ����ֵ�
        }
    }
#else
    void Awake()
    {
        dialogueIndex.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!dialogueIndex.ContainsKey(piece.ID))
                dialogueIndex.Add(piece.ID, piece);
        }
    }
#endif

    public QuestData_SO GetQuest()//ȡ����������
    {
        QuestData_SO currentQuest = null;
        foreach (var piece in dialoguePieces)//ѭ����ǰ�Ի���ÿһ�仰���ж��Ƿ�������
        {
            if (piece.quest != null)
                currentQuest = piece.quest;
        }
        return currentQuest;
    }

}
