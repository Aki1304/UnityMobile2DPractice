using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IBuffInfo
{
    Character castUnit { get; set; }// ������ ������ ĳ����
    string InfoID { get; }          //   ���� �ĺ���
    string InfoViewId { get; }      //   �ѱ��� ǥ��
    Sprite buffSprite { get; }      //   ���� ��������Ʈ
    BuffDurationType durationType { get; }  // ���� Ÿ�� ����
    SkillActionType actionType { get; }

    void OnBuffAdd(); // ���� ���� �� �۵�  

    /// <summary>
    /// ���� �� ���� ���� ��� �� ������ ����ٰ� ����
    /// </summary>
    /// <returns></returns>
    CharStats.buffStats GetBuffStats();

    Sprite ReturnSprite();              // ��������Ʈ �����ֱ�
}

public interface WhileAliveDuration 
{
    string GetBuffID { get; }          //   ���� �ĺ���

    // ������ �� ���� �� ���� ����
    public void OnWhileAliveAdd(Character owner) 
    {
        if (owner.gameObject is null) return;
        owner.OnDie += EndBuff;
    }

    public void EndBuff(CharStats stats)
    {
        GameObject[] party = Helper.DualManager._instanceParty;

        for (int i = 0; i < party.Length; i++)
        {
            if (party[i] is null) continue;

            Character unit = party[i].GetComponent<Character>();
            unit._charBuffManager.RemoveBuff(GetBuffID);

            unit.OnDie -= EndBuff; // ���� ���� �� �̺�Ʈ ����
        }
    }
}

public interface TurnBasedDuration
{
    string GetBuffID { get; }           //   ���� �ĺ���
    int CurrentTurn { get; set; }    // ���� �� �� 
    int MaxTurn { get; set; }        // �ִ� �� ��


    /// <summary>
    /// �������� ���� ���� �� �۵��� ������
    /// �ش� ������ �߰� ����, �ñر� ��� �������� ����.
    /// ��, �⺻ ���� �߰��� �ִ� ��� ����. EX) ������Ʈ �߰� �ൿ ��
    /// </summary>
    
    public void OnTurnBaseRoutine(Character unit)
    {
        // unit�� �� �ڽ��� ��Ī
        CurrentTurn -= 1;                               // �� �� ����

        if(CurrentTurn <= 0)                      //  ���� �� ���� 0 ���ϰ� �Ǹ�
            unit._charBuffManager.RemoveBuff(GetBuffID);   // ���� ����
    }
}

public interface OwnerTurnBasedDuration
{
    string GetBuffID { get; }           //   ���� �ĺ���
    int CurrentTurn { get; set; }    // ���� �� ��
    int MaxTurn { get; }        // �ִ� �� ��

    public void OnOwnerTurnStart()
    {
        CurrentTurn = MaxTurn;
    }

    public void OnOwnerTurnRoutine(Character owner)
    {
        if (CurrentTurn == 0) return; // ���� �� ���� 0�̶�� �˻� �� �ʿ� x

        CurrentTurn -= 1;                               // �� �� ����
        if(CurrentTurn <= 0) //  ���� �� ���� 0 ���ϰ� �Ǹ�
        {
            foreach(var instance in Helper.DualManager._instanceParty)
            {
                if (instance is null) continue;

                Character unit = instance.GetComponent<Character>();
                unit._charBuffManager.RemoveBuff(GetBuffID);   // ���� ����
            }    
        }
    }
}