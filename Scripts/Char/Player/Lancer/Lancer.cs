using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lancer : Player, IStack
{
    LancerBuff buff = new LancerBuff();

    public int ReturnCurStack()
    {
        return buff.CurrentTurn;
    }

    public int ReturnMaxStack()
    {
        return buff.MaxTurn;
    }

    public override void MyTurn()
    {
        base.MyTurn();

        if(buff is OwnerTurnBasedDuration duration) duration.OnOwnerTurnRoutine(this);
    }

    public override void NormalAttack(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.normalAttack));
    }

    public override void Passive()
    {
        buff.castUnit = this;
    }

    public override void Skill(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.skill));
        buff.OnBuffAdd();
        Helper.DualManager._context._encounterUI.OnStackRoutineUI(this);
    }

    public override void Ultimate(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.ultimate));
        buff.OnBuffAdd();
        Helper.DualManager._context._encounterUI.OnStackRoutineUI(this);
    }

   
}
