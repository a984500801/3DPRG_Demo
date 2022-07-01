using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Base Elements")]       //����Ԫ��
    public Image icon;              //ͼƬ
    public Text mainText;           //�ı�
    public Button nextButton;       //��ťѡ��
    public GameObject dialoguePanel;//�Ի����

    [Header("Options")]
    public RectTransform optionPanel;   //ѡ�����
    public OptionUI optionPrefab;       //ѡ��Ԥ����

    [Header("Data")]
    public DialogueData_SO currentData;     //���öԻ�����
    int currentIndex = 0;                   //�Ի����ݵĶԻ������

    protected override void Awake()
    {
        base.Awake();
        nextButton.onClick.AddListener(ContinueDialogue);   //����һ����ť��ӵ���¼�
    }

    //�����Ի�����
    void ContinueDialogue()
    {
        if (currentIndex < currentData.dialoguePieces.Count)                //��ǰ���С�ڶԻ���������
            UpdateMainDialogue(currentData.dialoguePieces[currentIndex]);   //���÷���������һ���Ի�
        else
            dialoguePanel.SetActive(false);                                 //����رնԻ����
    }

    //���¶Ի������ݣ���0��ſ�ʼ���ŶԻ�
    public void UpdateDialogueData(DialogueData_SO data)
    {
        currentData = data;
        currentIndex = 0;
    }

    //�����Ի���巽��
    public void UpdateMainDialogue(DialoguePiece piece)
    {
        dialoguePanel.SetActive(true);      //����Ի����
        currentIndex++;                     //�Ի�����������Լ�

        if (piece.image != null)            //��ͼƬ������ͼƬ������ͼƬ
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
        {
            icon.enabled = false;           //���򣬲���ʾͼƬ
        }

        mainText.text = "";                 //����նԻ�����
        //mainText.text = piece.text;
        mainText.DOText(piece.text, 1f);    //DOTweenʵ�ִ���Ч������ʾ�Ի�

        //�Ի�û��ѡ���ҶԻ����ݰ��������Ի���������һ����ť
        if (piece.options.Count == 0 && currentData.dialoguePieces.Count > 0)
        {
            nextButton.interactable = true;                                 //�ɵ㰴Ϊtrue
            nextButton.gameObject.SetActive(true);                          //������һ����ť
            nextButton.transform.GetChild(0).gameObject.SetActive(true);    //��ʾ�ı�
        }
        else
        {
            //nextButton.gameObject.SetActive(false);
            nextButton.interactable = false;                                //�ɵ㰴Ϊfalse
            nextButton.transform.GetChild(0).gameObject.SetActive(false);   //�Ӽ���������Ϊfalse
        }

        //����ѡ��
        CreateOptions(piece);
    }

    //����ѡ�ť����
    void CreateOptions(DialoguePiece piece)
    {
        if (optionPanel.childCount > 0) //ѡ������������壬������������
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < piece.options.Count; i++)               //�Ի������е�ѡ������
        {
            var option = Instantiate(optionPrefab, optionPanel);    //�ڸ�����ѡ��������ɶ�Ӧ�����İ�ť
            option.UpdateOption(piece, piece.options[i]);           //���ݶԻ������ݺ�ÿһ��ѡ��
        }
    }
}
