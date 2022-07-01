using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���͵���ģʽ��
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T:Singleton<T>
{
    //��̬����instance
    private static T instance;

    //�ⲿ���ʱ���
    public static T Instance
    {
        //�ɶ�����˽�б���
        get { return instance; }
    }

    //�̳���ɷ��ʵĿ���д���鷽��
    protected virtual void Awake()
    {
        //��Ϊ������
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else//������ڵ�ǰ���������
        {
            instance = (T)this;
        }
    }

    //���ص�ǰ�����Ƿ�����
    public static bool IsInitialized
    {
        //��Ϊ���Ѿ����ɷ���true
        get { return instance != null; }
    }

    //����ʵ��
    protected virtual void OnDestroy()
    {
        //��ǰʵ������������Ϊ��
        if (instance == this)
            instance = null;
    }
}
