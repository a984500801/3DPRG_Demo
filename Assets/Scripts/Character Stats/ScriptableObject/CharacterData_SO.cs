using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;

    //�ȼ�����
    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;

    //�������������
    public float LevelMutiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    //���¾���
    public void UpdateExp(int point)
    {
        //��ɱ������Ӿ���
        currentExp += point;

        //����ֵ����ִ������
        if (currentExp > baseExp)
            LevelUp();
    }

    //��������
    private void LevelUp()
    {
        //���������ݷ���
        //���㷽���������ȼ����������ᳬ�����ȼ�
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        //���Ի�����ֵ������������
        baseExp += (int)(baseExp * LevelMutiplier);
        maxHealth = (int)(maxHealth * LevelMutiplier);
        currentHealth = maxHealth;

        Debug.Log("Level UP!" + currentLevel + "Max Health:" + maxHealth);
    }
}
