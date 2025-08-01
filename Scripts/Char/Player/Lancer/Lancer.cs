using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lancer : Player, IStack
{
    LancerBuff buff = new LancerBuff();
    private int _stackExtra = 0;
    private const int _maxStackExtra = 2;

    public int ReturnCurStack()
    {
        return _stackExtra;
    }

    public int ReturnMaxStack()
    {
        return _maxStackExtra;
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
        AddandRemove(true);
        _stackExtra = _maxStackExtra;
        StackRoutine();
    }

    public override void Ultimate(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.ultimate));
    }

    public override void MyTurn()
    {
        base.MyTurn();

        if (_stackExtra - 1 < 0) _stackExtra = 0;
        else _stackExtra -= 1;

        StackRoutine();
    }

    private void StackRoutine()
    {
        if (_stackExtra == 0) AddandRemove(false);

        OnMyTurn?.Invoke();
    }

    /// <summary>
    /// true => Add
    /// false => Remove
    /// </summary>
    /// <param name="active"></param>
    private void AddandRemove(bool active)
    {
        foreach (var obj in Helper.DualManager._instanceParty)
        {
            if (obj is null) continue;

            Character unit = obj.GetComponent<Character>();
            if(active) unit._charBuffManager.AddBuff(buff);
            else unit._charBuffManager.RemoveBuff(buff.InfoID);
        }
    }
}
