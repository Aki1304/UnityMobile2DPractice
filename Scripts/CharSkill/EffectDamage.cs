using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDamage : MonoBehaviour
{
    private Animator _animator;                 // 애니메이터 컴포넌트

    private Character caster;                   // 임시 플레이어 캐릭터
    private List<GameObject> tempTargets;       // 임시 타겟 리스트
    SkillMetaData tempData;                     // 임시 스킬 메타 데이터

    int tmpCount = 0;
    public void ResetEffectAnimator()
    {
        _animator = GetComponent<Animator>();           // 애니메이터 컴포넌트 초기화
        _animator.runtimeAnimatorController = null;     // 애니메이터 컨트롤러 초기화
    }

    public void OnPassAnimEvent(GameObject casterObject, List<GameObject> targets, SkillMetaData data)
    {
        caster = casterObject.GetComponent<Character>();
        tempTargets = targets;
        tempData = data;

        tmpCount = data.hits;
    }

    public void GetTakeDamage()
    {
        float atkDamage = tempData.damage * caster.GetStats.GetFinalStats.Atk;

        foreach (var target in tempTargets)
        {
            if (target == null) continue; // 이 라인으로 null 또는 파괴된 객체까지 거름

            Character unit = target.GetComponent<Character>();
            if (unit == null) continue; // 혹시 Character 컴포넌트가 빠졌을 경우 방어

            GameObject font = Helper.PoolManager.GetFontPool();                     // 폰트 풀에서 폰트 가져오기

            EffectFont effectFont = font.GetComponent<EffectFont>();
            Vector2 effectPosition = GetEffectPosition(target, font);

            bool isCri;
            int finlaDamage = DamageFormula.CalculateDamage(caster.GetStats, unit.GetStats, out isCri, atkDamage);
            unit.TakeDamage(finlaDamage);

            effectFont.OnActiveFontEffect(finlaDamage, isCri);

            Helper.UM.GetAllDMGUI.totalDamage += finlaDamage;          // 전체 데미지에 누적
        }

        if (caster.GetComponent<Player>() is Player)
        {
            tmpCount--;
            if (tmpCount == 0) { Helper.UM.GetAllDMGUI.OnAllDamgeUI(); }
        }

    }

    public Vector2 GetEffectPosition(GameObject target, GameObject effect)
    {
        Transform trans = target.transform;                                 // 타겟의 트랜스폼
        RectTransform rect = effect.GetComponent<RectTransform>();          // 이펙트의 RectTransform 컴포넌트

        Collider col = target.GetComponent<Collider>();
        Vector3 boundCenter = new Vector3(col.bounds.center.x, col.bounds.center.y, 0f);

        Vector3 position = Camera.main.WorldToScreenPoint(trans.position);
        rect.position = position;                                         // UI 요소의 위치를 월드 좌표로 설정
        return position + boundCenter;
    }
}
