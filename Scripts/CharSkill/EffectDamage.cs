using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDamage : MonoBehaviour
{
    private Animator _animator;                 // �ִϸ����� ������Ʈ

    private Character caster;                   // �ӽ� �÷��̾� ĳ����
    private List<GameObject> tempTargets;       // �ӽ� Ÿ�� ����Ʈ
    SkillMetaData tempData;                     // �ӽ� ��ų ��Ÿ ������

    int tmpCount = 0;
    public void ResetEffectAnimator()
    {
        _animator = GetComponent<Animator>();           // �ִϸ����� ������Ʈ �ʱ�ȭ
        _animator.runtimeAnimatorController = null;     // �ִϸ����� ��Ʈ�ѷ� �ʱ�ȭ
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
            if (target == null) continue; // �� �������� null �Ǵ� �ı��� ��ü���� �Ÿ�

            Character unit = target.GetComponent<Character>();
            if (unit == null) continue; // Ȥ�� Character ������Ʈ�� ������ ��� ���

            GameObject font = Helper.PoolManager.GetFontPool();                     // ��Ʈ Ǯ���� ��Ʈ ��������

            EffectFont effectFont = font.GetComponent<EffectFont>();
            Vector2 effectPosition = GetEffectPosition(target, font);

            bool isCri;
            int finlaDamage = DamageFormula.CalculateDamage(caster.GetStats, unit.GetStats, out isCri, atkDamage);
            unit.TakeDamage(finlaDamage);

            effectFont.OnActiveFontEffect(finlaDamage, isCri);

            Helper.UM.GetAllDMGUI.totalDamage += finlaDamage;          // ��ü �������� ����
        }

        if (caster.GetComponent<Player>() is Player)
        {
            tmpCount--;
            if (tmpCount == 0) { Helper.UM.GetAllDMGUI.OnAllDamgeUI(); }
        }

    }

    public Vector2 GetEffectPosition(GameObject target, GameObject effect)
    {
        Transform trans = target.transform;                                 // Ÿ���� Ʈ������
        RectTransform rect = effect.GetComponent<RectTransform>();          // ����Ʈ�� RectTransform ������Ʈ

        Collider col = target.GetComponent<Collider>();
        Vector3 boundCenter = new Vector3(col.bounds.center.x, col.bounds.center.y, 0f);

        Vector3 position = Camera.main.WorldToScreenPoint(trans.position);
        rect.position = position;                                         // UI ����� ��ġ�� ���� ��ǥ�� ����
        return position + boundCenter;
    }
}
