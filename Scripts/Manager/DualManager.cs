using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UIElements;

public class DualManager : MonoBehaviour
{
    // ���� ���� �帧 ����
    public const int MaxRound = 25;         // �� ��ü �������� ���� ��
    public int _currentRound = 0;           // ��ü �������� �� ���� ����

    public Action OnChangeSP;

    // �̺�Ʈ ������ ���� Encounter ����
    public EncounterContext _context { get; set; }
    public GameObject[] _instanceParty { get; set; }    


    #region ��ų ����Ʈ ����
    private int _skillPoint;
    public int GetSP { get { return _skillPoint; } }
    public void InitSkill(int use) => _skillPoint = use;
    public void UpdateSkillPoijnt(int use)
    {
        if (_skillPoint + use > 5) _skillPoint = 5;
        if (_skillPoint + use < 0) _skillPoint = 0;
        else _skillPoint += use;

        OnChangeSP?.Invoke(); 
    }

    public bool CanSkillPoint(int use)
    {
        bool answer;

        if (_skillPoint - use < 0) answer = false;
        else answer = true;

        return answer;
    }
    #endregion


    private Vector3 _enemySkillEffectPos;    // �� ��ų ����Ʈ ��ġ
    public Vector3 EnemySkillEffectPos
    {
        get { return _enemySkillEffectPos; }
        set { _enemySkillEffectPos = value; }
    }

    // �Ҽ��� ��� ĳ��
    public float ReturnFormulaNumber(float value) => Mathf.Round(value * 10f) / 10f;

    public void GetInitDual(GameObject[] instanceParty)
    {
        _instanceParty = instanceParty;     // �ν��Ͻ� ��Ƽ�� ����
        OnSubsPassive(_instanceParty);
    }

    // ��� �Ŵ������� �̺�Ʈ ����
    public void OnSubsPassive(GameObject[] unitObjects)
    {
        // �߰� ����
        foreach(var obj in unitObjects)
        {
            Character unit = obj.GetComponent<Character>();
            IPassiveExtraAttack extra = unit as IPassiveExtraAttack;

            if (unit is null || extra is null) continue;

            _context._encounterUI.OnStackRoutineUI(unit);
        }

        List<IBuffInfo> tempBuffList = new List<IBuffInfo>();

        // ���� �нú� �ӽ� ����Ʈ�� �ֱ�
        foreach(var obj in unitObjects)
        {
            Character unit = obj.GetComponent<Character>();
            List<IBuffInfo> listBuff = unit._charBuffManager.GetActiveBuffs;

            // ĳ���Ͱ� ���� �ִ� ���� �� ��������� ����Ǵ� ������ �ִٸ�
            foreach(var buff in listBuff)
            {
                if(buff.durationType == BuffDurationType.WhileAlive)
                {
                    Debug.Log(buff);
                    tempBuffList.Add(buff);
                }
            }
        }

        // ���� ����
        foreach(var obj in unitObjects)
        {
            Character unit = obj.GetComponent<Character>();
            foreach(var buff in tempBuffList)
            {
                unit._charBuffManager.AddBuff(buff);
            }
        }

    }

    public void BrokerPassiveType(List<GameObject> target)
    {
        // ���� Ÿ�� Ȯ�� �� �нú꿡 �´ٸ� ���� ��Ű��
        // �׳� ��� ĳ���� ���� �˸��°ž� ~ �� �� ������� �������ִ� ��ų�� ����~ ���.
        foreach(var obj in _instanceParty)
        {
            if(obj is null) continue;

            Character unit = obj.GetComponent<Character>();
            if(unit is IPassiveExtraAttack extra)
            {
                extra.OnExtraAttack(target);
            }
        }
    }

    public void BrokerExtraTurnAdd(Character unit)
    {
        // ���Ŀ�� ���� Turnmanager�� ���������ϴµ�
        _context._turnManager.OnPushExtraTurn(unit.GetStats, SkillType.extra);
    }
}
