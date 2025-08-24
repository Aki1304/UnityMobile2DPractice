using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

public class LancerBuff : IBuffInfo, OwnerTurnBasedDuration
{
    public Character castUnit;
    public string InfoID => "Lancer Buff";

    public string InfoViewId => "���� ����";

    public Sprite buffSprite => castUnit.ReturnPlayerSprite(2);

    public BuffDurationType durationType => BuffDurationType.OwnerTurnBased;

    public SkillActionType actionType => SkillActionType.Buff;

    public void OnBuffAdd()
    {
        var duration = this as OwnerTurnBasedDuration;

        // ���� �ʱ�ȭ
        duration.OnOwnerTurnStart();

        foreach (var instance in Helper.DualManager._instanceParty)
        {
            if (instance is null) continue;

            Character unit = instance.GetComponent<Character>();
            unit._charBuffManager.AddBuff(this);
        }
    }

    public string GetBuffID => InfoID;

    private int _currentTurn = 0; // ���� �� ��
    private int _maxTurn = 2;     // ���� ���� �� ��

    public int CurrentTurn { get => _currentTurn; set => _currentTurn = value; }
    public int MaxTurn { get => _maxTurn; }
    Character IBuffInfo.castUnit { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }




    public CharStats.buffStats GetBuffStats()
    {
        return new CharStats.buffStats
        {
            AtkBonus = castUnit.GetStats.GetBaseStats.baseDef * 0.25f
        };
    }

    public Sprite ReturnSprite() => buffSprite;


}
