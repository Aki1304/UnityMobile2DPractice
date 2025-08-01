using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;


public abstract class Character : MonoBehaviour
{
    [Header("캐릭터 테이블")]
    [SerializeField] protected CharacterTable _charTable;

    [Header("캐릭터 스킬 테이블")]
    [SerializeField] protected SkillTable _skillTable;

    public CharacterBuffManager _charBuffManager;

    protected CharStats charStats;
    public CharStats GetStats { get { return charStats; } }

    public Action OnTakeDamage;
    public Action OnUltChange;
    public Action<CharStats> OnDie;
    public Action OnMyTurn;
    public Action OnTurnEnd;

    public void InitCharacter()
    {
        charStats = new CharStats();
        charStats.InitCharStat(this);

        charStats.SetUnitCode(_charTable.unitCode);                     // 캐릭터 유닛 코드 세팅
        charStats.GetBaseStats.SetBaseSetting(_charTable);              // 캐릭터 세팅

        _charBuffManager = new CharacterBuffManager();                  // 버프 매니저 세팅
        _charBuffManager.unit = this;

        Passive();                                                      // 패시브 한 번 실행
    }


    /// <summary>
    /// 0번 => 플레이어 스프라이트
    /// 1번 => 플레이어 궁극기 아이콘
    /// 2번 => 플레이어 버프 아이콘
    /// </summary>
    /// <returns></returns>
    public Sprite ReturnPlayerSprite(int index)
    {
        return index switch
        {
            0 => _charTable._playerSprite,
            1 => _charTable._ultIconSprite,
            2 => _charTable._buffSprite,
            _ => throw new NotImplementedException()
        };
    }

    public abstract void NormalAttack(List<GameObject> targets);

    public abstract void Skill(List<GameObject> targets);

    public abstract void Ultimate(List<GameObject> targets);

    public abstract void Passive();

    public virtual void MyTurn()
    {
        GetStats.UpdateFinalStats();
    }
    

    public void TakeDamage(int dmg)
    {
        Debug.Log($"[TakeDamage] {this.gameObject.name}, 데미지: {dmg}");
        charStats.GetCurrentHp -= dmg;

        if (charStats.GetCurrentHp <= 0) IsDie();

        OnTakeDamage?.Invoke();
    }

    public void PlusUlt(int amount)
    {
        charStats.GetCurrentUlt += amount;
        OnUltChange?.Invoke();
    }

    public void InvokeTurnEnd()     // 턴이 끝났다고 알림
    {
        OnTurnEnd?.Invoke();
    }

    public virtual List<GameObject> ReturnTargetObjects(GameObject[] players)
    {
        return null;
    }

    public SkillMetaData ReturnAct(SkillType type)
    {
        return type switch
        {
            SkillType.normal => _skillTable.normalAttack,
            SkillType.skill => _skillTable.skill,
            SkillType.ult => _skillTable.ultimate,
            SkillType.passive => _skillTable.passive,
            _ => throw new Exception("Error")

        };
    }

    public virtual void IsDie()
    {
        OnDie?.Invoke(GetStats);       // 이벤트 먼저

        StartCoroutine(DestroyAfterFrame());
    }

    private IEnumerator DestroyAfterFrame()
    {
        yield return null;

        _charBuffManager.ClearedBuff();
        OnTakeDamage = null;
        OnUltChange = null;
        OnDie = null;
        OnMyTurn = null;
        OnTurnEnd = null;

        Destroy(gameObject);
    }

}
