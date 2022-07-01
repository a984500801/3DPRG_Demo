using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    public string text;         //选项内容
    public string targetID;     //目标对话条ID
    public bool takeQuest;      //是否有任务
}
