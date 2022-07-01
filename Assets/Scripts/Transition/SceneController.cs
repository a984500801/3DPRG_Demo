using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;


public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    //��ɫprefab
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    bool fadeFinished;

    //��ɫ����
    GameObject player;
    //��ɫagent
    NavMeshAgent playerAgent;

    //����һ��ʼ��Ҫ����SceneController
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    //��������
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
                //ͬ��������
            case TransitionPoint.TransitionType.SameScene:
                //���ݵ�ǰ�������ֺ�Ŀ���ı�ǩ��Э��
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
                //�쳡������
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    //����Э��
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        //�л�����ʱ��������
        SaveManager.Instance.SavePlayerData();
        InventoryManager.Instance.SaveData();
        QuestManager.Instance.SaveQuestManager();

        //��ǰ�������ֺ�Ŀ��㳡������ͬ�����쳡������
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneFader fade = Instantiate(sceneFaderPrefab);
            yield return StartCoroutine(fade.FadeOut(1.2f));
            //�첽���س���
            yield return SceneManager.LoadSceneAsync(sceneName);
            //���ɽ�ɫ����Ŀ������������ɫ
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //�л��������ɽ�ɫ���ȡ����
            SaveManager.Instance.LoadPlayerData();
            yield return StartCoroutine(fade.FadeIn(1.2f));
            yield break;
        }
        else
        {
            //��ȡ��ɫ����
            player = GameManager.Instance.playerStats.gameObject;
            //ͣ�ý�ɫagent
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            //��Ŀ������������ɫ
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //������ָ�agent
            playerAgent.enabled = true;
            yield return null;
        }
    }

    //��ȡĿ���
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        //��ȡ����Ŀ���
        var entrances = FindObjectsOfType<TransitionDestination>();

        //�����ҵ�Ŀ����ǩ������Ŀ���
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }
        return null;
    }

    //�ص��˵�
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    //������Ϸ����
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    //���ص�һ������
    public void TransitionToFirstLevel()
    { 
        StartCoroutine(LoadLevel("Room"));
    }

    //���س���
    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2f));
            //���س���
            yield return SceneManager.LoadSceneAsync(scene);
            //�õ�Ŀ���λ�ã����ɽ�ɫ
            yield return player = Instantiate(playerPrefab,GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            //��������
            SaveManager.Instance.SavePlayerData();
            InventoryManager.Instance.SaveData();
            yield return StartCoroutine(fade.FadeIn(2f));
            yield break;
        }
    }

    //�������˵�����
    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }
}
