using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public struct SkillMetaData
{
    public int hits;
    public int range;
    public float damage;
    public SkillType skillType;                 // 스킬 타입 추가
    public AttackType type;                     // 어택 타입 추가
    public SkillActionType actionType;          // 액션 타입 추가
    public EffectPositionType effectType;       // 이펙트 타입 추가

    [TextArea]
    public string info;                         // 스킬 인포
}

[CreateAssetMenu(fileName = "SkillTable", menuName = "SkillInfo/Table")]
public class SkillTable : ScriptableObject
{
    [Header("해당 캐릭터 스킬 인포 칸")]
    public RuntimeAnimatorController _animCon;

    [Header("스킬 메타 정보")]
    public SkillMetaData normalAttack;
    public SkillMetaData skill;
    public SkillMetaData ultimate;
    public SkillMetaData passive;
}
