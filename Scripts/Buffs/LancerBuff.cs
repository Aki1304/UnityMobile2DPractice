using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LancerBuff : IBuffInfo
{
    public Character castUnit;
    public string InfoID => "Lancer Buff";

    public string InfoViewId => "랜서 버프";

    public Sprite buffSprite => castUnit.ReturnPlayerSprite(2);

    public BuffDurationType durationType => BuffDurationType.OwnerTurnBased;

    public SkillActionType actionType => SkillActionType.Buff;

    public void BuffRoutine()
    {
        
    }

    public CharStats.buffStats GetBuffStats()
    {
        return new CharStats.buffStats
        {
            AtkBonus = castUnit.GetStats.GetBaseStats.baseDef * 0.25f
        };
    }

    public Sprite ReturnSprite() => buffSprite;
}
