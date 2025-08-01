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
    Attack,       // 적 공격
    Heal,         // 회복
    Buff,         // 강화
    Debuff,       // 약화
    Utility,      // 텔레포트, 감지 등 특수
    Passive       // 발동형
}

// 이펙트 위치 타입을 구분하는 enum 추가
public enum EffectPositionType
{
    PerTarget,   // 각 타겟별로 이펙트
    SingleCenter // 전체 타겟의 중앙(혹은 첫 타겟 등) 하나에 이펙트
}

// 버프 지속 타입 구분
public enum BuffDurationType
{
    WhileAlive,          // 캐릭터가 살아있는 동안 지속
    TurnBased,           // 전체 턴 수 기준 지속 (예: 3턴 동안)
    OwnerTurnBased       // 버프 소유자의 턴 수 기준 지속 (예: 2회 행동 후 사라짐)
}


