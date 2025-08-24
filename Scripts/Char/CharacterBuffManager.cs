using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using UnityEngine;

public class CharacterBuffManager
{
    public Character unit;
    private CharStats.buffStats unitBuffStats { get { return unit.GetStats.GetBuffStats; }  set { unit.GetStats.GetBuffStats = value; } }

    private List<IBuffInfo> _activeBuffs = new List<IBuffInfo>();
    public List<IBuffInfo> GetActiveBuffs { get { return _activeBuffs; } }

    public Action<List<IBuffInfo>> OnUpdateBuff;

    public void AddBuff(IBuffInfo buff)
    {
        if (unit is null) return;

        // 중복 검사
        foreach (var item in _activeBuffs)
        {
            if (item.InfoID == buff.InfoID) return;
        }

        _activeBuffs.Add(buff);
        unitBuffStats = CalculateTotalBuff();
    }

    public void RemoveBuff(string buffID)
    {
        _activeBuffs.RemoveAll(b => b.InfoID == buffID);
        unitBuffStats = CalculateTotalBuff();
    }

    public CharStats.buffStats CalculateTotalBuff()
    {
        CharStats.buffStats total = new CharStats.buffStats();

        foreach (var buff in _activeBuffs) total.AddBuffs(buff.GetBuffStats());

        OnUpdateBuff?.Invoke(_activeBuffs);
        return total;
    }

    public void ClearedBuff()
    {
        _activeBuffs.Clear();
        OnUpdateBuff?.Invoke(_activeBuffs);
    }

    public void CheckTurnBasedBuff()
    {
        foreach(var buff in _activeBuffs)
        {
            if (buff is TurnBasedDuration duration) duration.OnTurnBaseRoutine(unit);
        }
    }

}