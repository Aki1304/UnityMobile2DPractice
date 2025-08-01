using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class WariorBuff : IBuffInfo
{
    public Character castUnit { get; set; }

    public string InfoID => "Warior Buff";
    public string InfoViewId => "������ ����";
    public Sprite buffSprite => castUnit.ReturnPlayerSprite(2);

    public BuffDurationType durationType => BuffDurationType.WhileAlive;

    public SkillActionType actionType => SkillActionType.Buff;

    public void BuffRoutine()
    {
        castUnit.OnDie += EndBuff;
    }

    public void EndBuff(CharStats stats)
    {
        GameObject[] temp = Helper.DualManager._instanceParty;

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] is null) continue;

            Character ch = temp[i].GetComponent<Character>();
            ch._charBuffManager.RemoveBuff(InfoID);
        }

        castUnit.OnDie -= EndBuff;
    }

    public CharStats.buffStats GetBuffStats()
    {
        return new CharStats.buffStats
        {
            AtkBonus = castUnit.GetStats.GetBaseStats.baseATk * 0.1f
        };
    }

    public Sprite ReturnSprite()
    {
       return buffSprite;
    }
}
