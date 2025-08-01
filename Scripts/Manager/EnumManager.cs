using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattleState
{
    None, PlayerTurn, EnemyTurn, Win, Lose
}
public enum AttackType { single, range }
public enum SkillType {normal, skill, ult, passive, extra }
public enum SkillActionType
{
    Attack,       // �� ����
    Heal,         // ȸ��
    Buff,         // ��ȭ
    Debuff,       // ��ȭ
    Utility,      // �ڷ���Ʈ, ���� �� Ư��
    Passive       // �ߵ���
}

// ����Ʈ ��ġ Ÿ���� �����ϴ� enum �߰�
public enum EffectPositionType
{
    PerTarget,   // �� Ÿ�ٺ��� ����Ʈ
    SingleCenter // ��ü Ÿ���� �߾�(Ȥ�� ù Ÿ�� ��) �ϳ��� ����Ʈ
}

// ���� ���� Ÿ�� ����
public enum BuffDurationType
{
    WhileAlive,          // ĳ���Ͱ� ����ִ� ���� ����
    TurnBased,           // ��ü �� �� ���� ���� (��: 3�� ����)
    OwnerTurnBased       // ���� �������� �� �� ���� ���� (��: 2ȸ �ൿ �� �����)
}


