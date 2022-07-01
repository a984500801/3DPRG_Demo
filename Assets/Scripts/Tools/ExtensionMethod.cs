using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//拓展静态类
public static class ExtensionMethod
{
    //常量
    private const float dotThreshold = 0.5f;

    //判断人物是否面对攻击目标
    public static bool IsFacingTarget(this Transform transform,Transform target)
    {
        //方向坐标向量
        var vectorToTarget = target.position - transform.position;
        //得到向量方向
        vectorToTarget.Normalize();
        //获得夹角余弦值
        float dot = Vector3.Dot(transform.forward, vectorToTarget);
        //dot是否大于0.5
        return dot >= dotThreshold;
    }
}
