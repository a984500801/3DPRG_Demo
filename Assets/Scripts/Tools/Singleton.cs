using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 泛型单例模式类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T:Singleton<T>
{
    //静态变量instance
    private static T instance;

    //外部访问变量
    public static T Instance
    {
        //可读访问私有变量
        get { return instance; }
    }

    //继承类可访问的可重写的虚方法
    protected virtual void Awake()
    {
        //不为空销毁
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else//否则等于当前类归属物体
        {
            instance = (T)this;
        }
    }

    //返回当前单例是否生成
    public static bool IsInitialized
    {
        //不为空已经生成返回true
        get { return instance != null; }
    }

    //销毁实例
    protected virtual void OnDestroy()
    {
        //当前实例被销毁设置为空
        if (instance == this)
            instance = null;
    }
}
