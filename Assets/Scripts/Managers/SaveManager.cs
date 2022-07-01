using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    //������
    string sceneName = "";
    //��ȡ������ĳ�����
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        //�糡����������
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

    //����������ݷ���
    public void SavePlayerData()
    {
        //����������ݺͼ�ֵ���б���
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void LoadPlayerData()
    {
        //��������ݺͼ�ֵ���м���
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    //�洢����������data�ͼ�ֵ
    public void Save(Object data,string key)
    {
        //��ʱ������תΪjson����
        var jsonData = JsonUtility.ToJson(data, true);
        //PlayerPrefs���ݼ�ֵ���浽ϵͳ����
        PlayerPrefs.SetString(key, jsonData);
        //����
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        //����
        PlayerPrefs.Save();
    }

    //���ط���
    public void Load(Object data, string key)
    {
        //�ؼ�ֵ�Ƿ�����ֵ
        if (PlayerPrefs.HasKey(key))
        {
            //��ȡ֮ǰ��ֵ��json��ֵд�ص�data��
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
