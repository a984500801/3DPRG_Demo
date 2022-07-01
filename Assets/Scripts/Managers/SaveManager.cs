using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    //场景名
    string sceneName = "";
    //获取数据里的场景名
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        //跨场景，不销毁
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
            InventoryManager.Instance.SaveData();
            QuestManager.Instance.SaveQuestManager();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMain();
        }
    }

    //保存玩家数据方法
    public void SavePlayerData()
    {
        //传入玩家数据和键值进行保存
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void LoadPlayerData()
    {
        //传玩家数据和键值进行加载
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    //存储方法，传入data和键值
    public void Save(Object data,string key)
    {
        //临时变量，转为json数据
        var jsonData = JsonUtility.ToJson(data, true);
        //PlayerPrefs根据键值保存到系统磁盘
        PlayerPrefs.SetString(key, jsonData);
        //保存
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        //保存
        PlayerPrefs.Save();
    }

    //加载方法
    public void Load(Object data, string key)
    {
        //关键值是否有数值
        if (PlayerPrefs.HasKey(key))
        {
            //获取之前键值的json数值写回到data中
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
