using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class WariorBuff : IBuffInfo, WhileAliveDuration
{
    public Character castUnit { get; set; }

    public string InfoID => "Warior Buff";
    public string GetBuffID => InfoID;

    public string InfoViewId => "워리어 버프";
    public Sprite buffSprite => castUnit.ReturnPlayerSprite(2);

    public BuffDurationType durationType => BuffDurationType.WhileAlive;

    public SkillActionType actionType => SkillActionType.Buff;

    public void OnBuffAdd()
    {
        var duration = this as WhileAliveDuration;

        foreach (var instance in Helper.DualManager._instanceParty)
        {
            if (instance is null) continue;

            Character unit = instance.GetComponent<Character>();
            unit._charBuffManager.AddBuff(this);
        }

        duration.OnWhileAliveAdd(castUnit);
    }


    public CharStats.buffStats GetBuffStats()
    {
        return new CharStats.buffStats
        {
            AtkBonus = castUnit.GetStats.GetBaseStats.baseATk * 0.2f
        };
    }

    public Sprite ReturnSprite()
    {
       return buffSprite;
    }
}
