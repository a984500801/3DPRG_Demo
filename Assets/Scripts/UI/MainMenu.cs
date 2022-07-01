
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;

    PlayableDirector director;

    void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        //��Ӱ�ť�㰴�¼�
        newGameBtn.onClick.AddListener(PlayerTimeline);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }


    void PlayerTimeline()
    {
        director.Play();
    }

    //����Ϸ
    void NewGame(PlayableDirector obj)
    {
        //���������Ϸ����
        PlayerPrefs.DeleteAll();
        //ת������
        SceneController.Instance.TransitionToFirstLevel();
    }

    //������Ϸ
    void ContinueGame()
    {
        //ת����������ȡ����
        if(SaveManager.Instance.SceneName!=null)
            SceneController.Instance.TransitionToLoadGame();
    }

    //�˳���Ϸ
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("�˳���Ϸ");
    }
}
