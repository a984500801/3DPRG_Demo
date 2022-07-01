using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    //����Stats
    public CharacterStats playerStats;
    //���
    private CinemachineFreeLook followCamera;

    //�����б��ռ����м̳н�����Ϸ�۲��߽ӿڵĵ�����
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    //ע�ᱻ�۲���
    public void RegisterPlayer(CharacterStats player)
    {
        //��ȡ��ɫ�������
        playerStats = player;
        //�ҵ����
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        //�����Ϊ�գ�ʵ���������
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    //���뵽�б�
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    //���б����Ƴ�
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //�㲥
    public void NotifyObservers()
    {
        //���б����й۲��߹㲥
        foreach (var observer in endGameObservers)
        {
            //ִ�нӿڵĺ���
            observer.EndNotify();
        }
    }

    //��ȡ���Ŀ���λ��
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
