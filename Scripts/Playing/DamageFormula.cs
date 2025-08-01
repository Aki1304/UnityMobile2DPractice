using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageFormula
{
    private const float BalanceScale = 0.5f; // 전반적인 데미지 조정 (0.5 = 50% 감소)

    public static int CalculateDamage(CharStats unit, CharStats target, out bool isCri ,float atkDmg = 0)
    {
        float atk = atkDmg;
        float def = target.GetFinalStats.Def;
        float critRate = unit.GetFinalStats.Cri;
        float critDmg = unit.GetFinalStats.CriDmg;

        isCri = UnityEngine.Random.Range(0f, 100f) < critRate;
        float critMultiplier = isCri ? (1f + (critDmg / 100f)) : 1f;

        // 선형 방어율 공식
        float defenseReduction = 1f - (def / (def + 100f));
        float baseDamage = atk * defenseReduction;

        float finalDamage = (baseDamage * critMultiplier) * BalanceScale;
        finalDamage = Mathf.Max(1f, finalDamage);
        int last = Mathf.RoundToInt(finalDamage);

        Debug.Log($"[{(isCri ? "크리티컬!" : "일반")}] 데미지: {finalDamage:F2} (공:{atk}, 방:{def}, 감소율:{defenseReduction:F2})");

        return last;
    }

}
