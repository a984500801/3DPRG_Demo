using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��չ��̬��
public static class ExtensionMethod
{
    //����
    private const float dotThreshold = 0.5f;

    //�ж������Ƿ���Թ���Ŀ��
    public static bool IsFacingTarget(this Transform transform,Transform target)
    {
        //������������
        var vectorToTarget = target.position - transform.position;
        //�õ���������
        vectorToTarget.Normalize();
        //��üн�����ֵ
        float dot = Vector3.Dot(transform.forward, vectorToTarget);
        //dot�Ƿ����0.5
        return dot >= dotThreshold;
    }
}
