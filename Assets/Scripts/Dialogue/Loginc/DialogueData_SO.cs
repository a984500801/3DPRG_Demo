using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Dialogue",menuName ="Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();                              //对话语句列表
    public Dictionary<string, DialoguePiece> dialogueIndex = new Dictionary<string, DialoguePiece>();   //用将ID与对话条进行配对

#if UNITY_EDITOR
    void OnValidate()   //在Unity编辑修改时进行更新
    {
        dialogueIndex.Clear();                          //修改时清空字典
        foreach (var piece in dialoguePieces)           //遍历对话数据列表
        {
            if (!dialogueIndex.ContainsKey(piece.ID))   //字典未包含对应ID
                dialogueIndex.Add(piece.ID, piece);     //将相应的ID与对话数据存入字典
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

    public QuestData_SO GetQuest()//取得任务数据
    {
        QuestData_SO currentQuest = null;
        foreach (var piece in dialoguePieces)//循环当前对话的每一句话，判断是否有任务
        {
            if (piece.quest != null)
                currentQuest = piece.quest;
        }
        return currentQuest;
    }

}
