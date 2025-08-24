using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gambler : Player
{

    public override void NormalAttack(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.normalAttack));
    }

    public override void Passive()
    {

    }

    public override void Skill(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.skill));
    }

    public override void Ultimate(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.ultimate));
        Helper.DualManager.UpdateSkillPoijnt(+1);
    }

}
