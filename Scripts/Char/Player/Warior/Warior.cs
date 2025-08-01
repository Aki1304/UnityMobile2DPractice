using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warior : Player
{
    WariorBuff wariorBuff = new WariorBuff();

    public override void NormalAttack(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.normalAttack));
    }

    public override void Passive()
    {
        // 버프 넣어주기
        wariorBuff.castUnit = this;
        _charBuffManager.AddBuff(wariorBuff);
    }

    public override void Skill(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.skill));
    }

    public override void Ultimate(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.ultimate));
    }
}
