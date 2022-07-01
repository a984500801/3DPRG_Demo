using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    //人物Stats
    public CharacterStats playerStats;
    //相机
    private CinemachineFreeLook followCamera;

    //创建列表收集所有继承结束游戏观察者接口的敌人类
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    //注册被观察者
    public void RegisterPlayer(CharacterStats player)
    {
        //获取角色数据组件
        playerStats = player;
        //找到相机
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        //相机不为空，实现相机跟随
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    //加入到列表
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    //从列表中移除
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //广播
    public void NotifyObservers()
    {
        //向列表所有观察者广播
        foreach (var observer in endGameObservers)
        {
            //执行接口的函数
            observer.EndNotify();
        }
    }

    //获取入口目标点位置
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.Room)
                return item.transform;
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
                return item.transform;
        }
        return null;
    }
}
