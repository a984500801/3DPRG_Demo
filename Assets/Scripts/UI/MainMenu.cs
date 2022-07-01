
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

        //添加按钮点按事件
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

    //新游戏
    void NewGame(PlayableDirector obj)
    {
        //清除所有游戏数据
        PlayerPrefs.DeleteAll();
        //转换场景
        SceneController.Instance.TransitionToFirstLevel();
    }

    //继续游戏
    void ContinueGame()
    {
        //转换场景，读取进度
        if(SaveManager.Instance.SceneName!=null)
            SceneController.Instance.TransitionToLoadGame();
    }

    //退出游戏
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("退出游戏");
    }
}
