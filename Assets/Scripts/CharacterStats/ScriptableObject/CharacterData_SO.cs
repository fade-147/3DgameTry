using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO :ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;   //击杀不同的敌人会获得不同的经验值，敌人用，角色不用填

    [Header("Level")]    //给角色等级，敌人不用填
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;   //等级加成，影响角色的属性成长
    public float LevelMultiplier
    {
        get
        {
                       return 1 + (currentLevel - 1) * levelBuff;   //等级加成的计算公式，当前等级越高，属性成长越快
        }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;
        if(currentExp>=baseExp)
        {
            currentExp -= baseExp;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        //所有你想提升属性的方法
        currentLevel=Mathf.Clamp(currentLevel+1,0,maxLevel);     //每次升级都把当前等级加1，并且限制在0和最大等级之间
        baseExp+=(int)(baseExp*LevelMultiplier);   //每次升级都把升级所需的经验值增加，增加的量根据等级加成来计算

        maxHealth=(int)(maxHealth*LevelMultiplier);   //每次升级都把最大血量增加，增加的量根据等级加成来计算
        currentHealth = maxHealth;
    }
}
