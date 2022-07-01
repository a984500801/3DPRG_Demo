using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Base Elements")]       //基本元素
    public Image icon;              //图片
    public Text mainText;           //文本
    public Button nextButton;       //按钮选项
    public GameObject dialoguePanel;//对话面板

    [Header("Options")]
    public RectTransform optionPanel;   //选项面板
    public OptionUI optionPrefab;       //选项预制体

    [Header("Data")]
    public DialogueData_SO currentData;     //引用对话数据
    int currentIndex = 0;                   //对话数据的对话条序号

    protected override void Awake()
    {
        base.Awake();
        nextButton.onClick.AddListener(ContinueDialogue);   //给下一条按钮添加点击事件
    }

    //继续对话方法
    void ContinueDialogue()
    {
        if (currentIndex < currentData.dialoguePieces.Count)                //当前序号小于对话条的数量
            UpdateMainDialogue(currentData.dialoguePieces[currentIndex]);   //调用方法播放下一条对话
        else
            dialoguePanel.SetActive(false);                                 //否则关闭对话面板
    }

    //更新对话的数据，从0序号开始播放对话
    public void UpdateDialogueData(DialogueData_SO data)
    {
        currentData = data;
        currentIndex = 0;
    }

    //打开主对话面板方法
    public void UpdateMainDialogue(DialoguePiece piece)
    {
        dialoguePanel.SetActive(true);      //激活对话面板
        currentIndex++;                     //对话条播放序号自加

        if (piece.image != null)            //有图片，激活图片并传递图片
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
        {
            icon.enabled = false;           //否则，不显示图片
        }

        mainText.text = "";                 //先清空对话内容
        //mainText.text = piece.text;
        mainText.DOText(piece.text, 1f);    //DOTween实现打字效果来显示对话

        //对话没有选项且对话数据包含多条对话，激活下一条按钮
        if (piece.options.Count == 0 && currentData.dialoguePieces.Count > 0)
        {
            nextButton.interactable = true;                                 //可点按为true
            nextButton.gameObject.SetActive(true);                          //激活下一条按钮
            nextButton.transform.GetChild(0).gameObject.SetActive(true);    //显示文本
        }
        else
        {
            //nextButton.gameObject.SetActive(false);
            nextButton.interactable = false;                                //可点按为false
            nextButton.transform.GetChild(0).gameObject.SetActive(false);   //子集物体设置为false
        }

        //创建选项
        CreateOptions(piece);
    }

    //创建选项按钮方法
    void CreateOptions(DialoguePiece piece)
    {
        if (optionPanel.childCount > 0) //选项面板有子物体，遍历进行销毁
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < piece.options.Count; i++)               //对话条已有的选项数量
        {
            var option = Instantiate(optionPrefab, optionPanel);    //在父级的选项面板生成对应数量的按钮
            option.UpdateOption(piece, piece.options[i]);           //传递对话条数据和每一个选项
        }
    }
}
