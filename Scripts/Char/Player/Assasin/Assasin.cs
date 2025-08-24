using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assasin : Player, IPassiveExtraAttack, IStack
{
    private int _stackExtra = 0;
    private const int _maxStackExtra = 3;

    private List<GameObject> extraList = new List<GameObject>();

    EncounterUI _encounterUI { get { return Helper.DualManager._context._encounterUI; } }

    public int ReturnCurStack()
    {
        return _stackExtra;
    }

    public int ReturnMaxStack()
    {
        return _maxStackExtra;
    }

    public void OnExtraAttack(List<GameObject> targets)
    {
        if (this is null) return;

        // 이미 3이면 ..
        if (_stackExtra >= _maxStackExtra)
        {
            return;
        }

        _stackExtra += 1;
        // UI 연결
        _encounterUI.OnStackRoutineUI(this);

        if(_stackExtra == _maxStackExtra)
        {
            Helper.DualManager.BrokerExtraTurnAdd(this);
            extraList.Add(targets[0]);
        }
    }

    public void ActAttack(GameObject[] targets)
    {
        bool isReturn = false;

        for(int i = 0; i < targets.Length; i++)
        {
            if(targets[i] == null) continue;

            if (targets[i] == extraList[0])
            {
                isReturn = true;
                Acting();
                break;
            }
        }

        if (isReturn) return;

        // 찾지 못했다면
        while(true)
        {
            int randIdx = UnityEngine.Random.Range(0, targets.Length);
            if (targets[randIdx] != null)
            {
                extraList[0] = targets[randIdx];
                Acting();
                break;
            }
        }
    }

    void Acting()
    {
        List<GameObject> targets = new List<GameObject>(extraList);
        StartCoroutine(WaitMotion(targets, _skillTable.passive));
        _stackExtra = 0;
        _encounterUI.OnStackRoutineUI(this);
        extraList.Clear();
    }

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
    }
}
