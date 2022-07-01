using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;


public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    //角色prefab
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    bool fadeFinished;

    //角色对象
    GameObject player;
    //角色agent
    NavMeshAgent playerAgent;

    //加载一开始不要销毁SceneController
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

    //场景传送
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
                //同场景传送
            case TransitionPoint.TransitionType.SameScene:
                //传递当前场景名字和目标点的标签到协程
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
                //异场景传送
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    //传送协程
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        //切换场景时保存数据
        SaveManager.Instance.SavePlayerData();
        InventoryManager.Instance.SaveData();
        QuestManager.Instance.SaveQuestManager();

        //当前场景名字和目标点场景名不同则是异场景传送
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneFader fade = Instantiate(sceneFaderPrefab);
            yield return StartCoroutine(fade.FadeOut(1.2f));
            //异步加载场景
            yield return SceneManager.LoadSceneAsync(sceneName);
            //生成角色，将目标点参数给到角色
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //切换场景生成角色后读取数据
            SaveManager.Instance.LoadPlayerData();
            yield return StartCoroutine(fade.FadeIn(1.2f));
            yield break;
        }
        else
        {
            //获取角色对象
            player = GameManager.Instance.playerStats.gameObject;
            //停用角色agent
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            //将目标点参数给到角色
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //传送完恢复agent
            playerAgent.enabled = true;
            yield return null;
        }
    }

    //获取目标点
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        //获取所有目标点
        var entrances = FindObjectsOfType<TransitionDestination>();

        //遍历找到目标点标签，返回目标点
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }
        return null;
    }

    //回到菜单
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    //继续游戏加载
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    //加载第一个场景
    public void TransitionToFirstLevel()
    { 
        StartCoroutine(LoadLevel("Room"));
    }

    //加载场景
    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2f));
            //加载场景
            yield return SceneManager.LoadSceneAsync(scene);
            //拿到目标点位置，生成角色
            yield return player = Instantiate(playerPrefab,GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            //保存数据
            SaveManager.Instance.SavePlayerData();
            InventoryManager.Instance.SaveData();
            yield return StartCoroutine(fade.FadeIn(2f));
            yield break;
        }
    }

    //加载主菜单场景
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
