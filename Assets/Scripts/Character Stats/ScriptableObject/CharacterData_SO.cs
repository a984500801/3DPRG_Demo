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

    //等级参数
    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;

    //获得升级的增量
    public float LevelMutiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    //更新经验
    public void UpdateExp(int point)
    {
        //击杀怪物添加经验
        currentExp += point;

        //经验值满了执行升级
        if (currentExp > baseExp)
            LevelUp();
    }

    //升级方法
    private void LevelUp()
    {
        //提升的数据方法
        //计算方法限制最大等级，升级不会超过最大等级
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        //属性基础数值随升级而提升
        baseExp += (int)(baseExp * LevelMutiplier);
        maxHealth = (int)(maxHealth * LevelMutiplier);
        currentHealth = maxHealth;

        Debug.Log("Level UP!" + currentLevel + "Max Health:" + maxHealth);
    }
}
