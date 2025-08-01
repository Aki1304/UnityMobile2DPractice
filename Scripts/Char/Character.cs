using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;


public abstract class Character : MonoBehaviour
{
    [Header("ĳ���� ���̺�")]
    [SerializeField] protected CharacterTable _charTable;

    [Header("ĳ���� ��ų ���̺�")]
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

        charStats.SetUnitCode(_charTable.unitCode);                     // ĳ���� ���� �ڵ� ����
        charStats.GetBaseStats.SetBaseSetting(_charTable);              // ĳ���� ����

        _charBuffManager = new CharacterBuffManager();                  // ���� �Ŵ��� ����
        _charBuffManager.unit = this;

        Passive();                                                      // �нú� �� �� ����
    }


    /// <summary>
    /// 0�� => �÷��̾� ��������Ʈ
    /// 1�� => �÷��̾� �ñر� ������
    /// 2�� => �÷��̾� ���� ������
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
        Debug.Log($"[TakeDamage] {this.gameObject.name}, ������: {dmg}");
        charStats.GetCurrentHp -= dmg;

        if (charStats.GetCurrentHp <= 0) IsDie();

        OnTakeDamage?.Invoke();
    }

    public void PlusUlt(int amount)
    {
        charStats.GetCurrentUlt += amount;
        OnUltChange?.Invoke();
    }

    public void InvokeTurnEnd()     // ���� �����ٰ� �˸�
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
        OnDie?.Invoke(GetStats);       // �̺�Ʈ ����

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
