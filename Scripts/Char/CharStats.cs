using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using static CharStats;

public class CharStats
{
    public Character GetChar {  get; set; }
    public baseStats GetBaseStats { get; set; }
    public buffStats GetBuffStats { get; set; }
    public FinalStats GetFinalStats { get; set; }

    private int _unitCode = 0;                          // 코드 0 << 플레이어 ||  코드 1 << 노말  || 코드 2 << 보스
    public int GetUnitCode { get { return _unitCode; } }

    public bool IsDie() => GetBaseStats.currentHP <= 0;
    public void InitCharStat(Character info)
    {
        GetBaseStats = new baseStats();
        GetBuffStats = new buffStats();
        GetFinalStats = new FinalStats();

        GetChar = info;
    }

    public void SetUnitCode(int code) => _unitCode = code;
    public int GetCurrentHp
    {
        get => GetBaseStats.currentHP;
        set
        {
            GetBaseStats.currentHP = Mathf.Clamp(value, 0, GetBaseStats.baseMaxHP);
        }
    }

    public int GetCurrentUlt
    {
        get => GetBaseStats.currentUlt;
        set
        {
            GetBaseStats.currentUlt = Mathf.Clamp(value, 0, GetBaseStats.baseMaxUlt);
        }
    }

    public void UpdateFinalStats()
    {
        GetFinalStats.Calculate(GetBaseStats,GetBuffStats);
    }

    /// <summary>
    /// 0 -> HP
    /// 1 -> Ult
    /// </summary>
    /// <returns></returns>
    public float Amount(int count)
    {
        return count switch
        {
            0 => (float)GetCurrentHp / GetBaseStats.baseMaxHP,
            1 => (float)GetCurrentUlt / GetBaseStats.baseMaxUlt,
            _ => 0
        };
    }

    #region 스탯 묶어두기
    public class baseStats
    {
        public string charName { get; set; }            // 캐릭터 이름
        public float baseATk {  get; set; }             // 기본 공격력
        public float baseDef { get; set; }              // 기본 방어력
        public float baseCri { get; set; }              // 기본 크리티컬
        public float baseCriDmg { get; set; }           // 기본 크리티컬 데미지
        public int baseMaxHP { get; set; }              // 기본 최대체력
        public int currentHP { get; set; }              // 현재 체력
        public int baseMaxUlt { get; set; }           // 기본 최대 궁게이지
        public int currentUlt { get; set; }           // 현재 궁게이지
        public float baseSpeed { get; set; }            // 기본 현재 속도

        public void SetBaseSetting(CharacterTable thisTable)
        {
            charName = thisTable.dataName;
            baseATk = thisTable.dataATk;
            baseDef = thisTable.dataDef;
            baseCri = thisTable.dataCri;
            baseCriDmg = thisTable.dataCriDmg;
            baseMaxHP = (int)thisTable.dataHP;
            currentHP = baseMaxHP;
            baseMaxUlt = thisTable.dataUlt;
            baseSpeed = thisTable.dataSpeed;
        }
    }

    public class buffStats
    {
        public float AtkBonus, DefBonus, CriBonus, CriDmgBonus, SpeedBonus;
        public int HpBonus;
        public float UltBonus;

        public void Reset()
        {
            AtkBonus = DefBonus = CriBonus = CriDmgBonus = SpeedBonus = UltBonus  = 0;
            HpBonus = 0;
        }

        public void AddBuffs(buffStats stats)
        {
            AtkBonus += stats.AtkBonus;
            DefBonus += stats.DefBonus;
            CriBonus += stats.CriBonus;
            CriDmgBonus += stats.CriDmgBonus;
            SpeedBonus += stats.SpeedBonus;
            HpBonus += stats.HpBonus;
            UltBonus += stats.UltBonus;
        }
    }

    public class FinalStats
    {
        public float Atk, Def, Cri, CriDmg, Speed;
        public int MaxHP;
        public float MaxUlt;

        public void Calculate(baseStats baseStats, buffStats buffStats)
        {
            Atk = baseStats.baseATk + buffStats.AtkBonus;
            Def = baseStats.baseDef + buffStats.DefBonus;
            Cri = baseStats.baseCri + buffStats.CriBonus;
            CriDmg = baseStats.baseCriDmg + buffStats.CriDmgBonus;
            MaxHP = baseStats.baseMaxHP + buffStats.HpBonus;
            MaxUlt = baseStats.baseMaxUlt + buffStats.UltBonus;
            Speed = baseStats.baseSpeed + buffStats.SpeedBonus;
        }
    }
    #endregion
}
