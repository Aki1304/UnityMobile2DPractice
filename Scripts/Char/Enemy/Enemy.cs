using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    public EnemyType _enemyType;            // �ν����Ϳ��� ����

    public override void NormalAttack(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.normalAttack));
    }

    public override void Passive()
    {

    }

    public override void Skill(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.skill));
    }

    public override void Ultimate(List<GameObject> targets)
    {
        StartCoroutine(WaitMotion(targets, _skillTable.ultimate));
    }

    public override void MyTurn()
    {
        throw new System.NotImplementedException();
    }

    protected IEnumerator WaitMotion(List<GameObject> targets, SkillMetaData data)
    {
        SkillType type = data.skillType;

        yield return PlayEffect(targets, data);   // ����Ʈ ����

        Helper.DualManager.UpdateSkillPoijnt(+1); // or -1 based on usage

        InvokeTurnEnd();
    }


    public override List<GameObject> ReturnTargetObjects(GameObject[] players)
    {
        List<GameObject> tmpList = new List<GameObject>();
        int centerIndex;                                                                

        while (true)
        {
            int randNum = UnityEngine.Random.Range(0, players.Length);                      // �÷��̾� �� �������� �ϳ� ����
            if (players[randNum] != null)
            {
                centerIndex = randNum;
                break;                                                                      // �߰� �� ���� Ż��
            }   
        }

        SkillMetaData act = this.ReturnAct(SkillType.skill);

        // ������ �� ���� ���
        if (act.range == 1) { tmpList.Add(players[centerIndex]); }

        // ���� ���� ��ü �����̶��
        if (act.range == 5)
        {
            // ����ִ� �� ��� �߰�
            foreach (var enemy in players) if (enemy != null) tmpList.Add(enemy);
        }

        if (act.range == 3)
        {
            tmpList.Add(players[centerIndex]);                                     // ���� ������Ʈ �߰�
            int leftindex = centerIndex - 1;                                                    // ���� �ε���
            int rightindex = centerIndex + 1;                                                   // ������ �ε���

            if (leftindex >= 0 && players[leftindex] != null)                         // �ε��� �迭 ���� ���� üũ
            {
                tmpList.Add(players[leftindex]);
            }

            if (rightindex < players.Length && players[rightindex] != null)
            {
                tmpList.Add(players[rightindex]);
            }
        }

        return tmpList;
    }

    protected IEnumerator PlayEffect(List<GameObject> targets, SkillMetaData data)
    {
        GameObject caster = this.gameObject;
        Character unit = caster.GetComponent<Character>();

        yield return null;

        var effectObject = GetEffectObject(caster.CompareTag("Player"), data);      // ����Ʈ ������Ʈ ��������

        var positions = GetEffectPositions(targets, data.effectType, data.range);   // ����Ʈ ��ġ ���
        List<Animator> curAnimCon = new List<Animator>();                           // �ִϸ����� ����뵵

        Animator checkAnim;                                                         // �ִϸ����� üũ�뵵

        bool IsAnimEnd = false;

        // �ִϸ����� ���� �� ������Ʈ ��ġ ����
        for (int i = 0; i < effectObject.Count && i < targets.Count; i++)
        {
            var obj = effectObject[i];
            obj.transform.position = positions[i];

            // ����Ʈ �۾�
            EffectDamage effect = obj.GetComponent<EffectDamage>();             // �����ؾ� �� ��� �����ֱ�
            effect.ResetEffectAnimator();                                       // ����Ʈ �ִϸ����� �ʱ�ȭ
            effect.OnPassAnimEvent(caster, targets, data, i);                   // ����Ʈ�� �ִϸ��̼� �̺�Ʈ�� ���� ������ ������ �� �ֵ��� ����

            // �ִϸ����� �۾�
            Animator animator = obj.GetComponent<Animator>();
            animator.runtimeAnimatorController = _skillTable._animCon;                      // ���� ��ų �ִϸ����� ��Ʈ�ѷ� ��������

            curAnimCon.Add(animator);                                           // �ִϸ����� ��Ʈ�ѷ��� ���� (�ʿ�� ��� ����)
        }

        yield return new WaitForSeconds(0.1f);                                  // �ִϸ��̼� ���� �� ��� ��� (�ִϸ����Ͱ� �غ�� ������)

        for (int i = 0; i < curAnimCon.Count; i++)
        {
            string skillName = (data.skillType == SkillType.normal) ? "Normal" :
                               (data.skillType == SkillType.skill) ? "Skill" : "Ult";

            curAnimCon[i].SetTrigger(skillName);

            if (i == 0)                                                  // �ִϸ����� ��⸦ ���� ���
            {
                checkAnim = curAnimCon[i];                               // ù ��° �ִϸ����͸� üũ������ ����
                StartCoroutine(DelayAnimator(checkAnim));             // �ִϸ����� �ڷ�ƾ ����
            }
        }

        yield return new WaitUntil(() => IsAnimEnd);             // �ִϸ��̼� ���
        yield return null;

        // ����Ʈ Ǯ ���� ���ֱ�
        Helper.PoolManager.ReturnEffectPool(effectObject);

        IEnumerator DelayAnimator(Animator anim)
        {
            // ù ������ ��� (�ִϸ����Ͱ� �غ�� ������)
            yield return null;

            while (true)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    IsAnimEnd = true;
                    yield break; // �ִϸ��̼��� ������ �ڷ�ƾ ����
                }
                yield return null; // ���� �����ӱ��� ���
            }
        }
    }

    /// <summary>
    /// Ÿ�� ���� ��, ��ǥ �ϳ��� ����Ʈ ��� ��ġ�� ����ϰ� ������ ���
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
    protected List<Vector3> GetTargetCenters(List<GameObject> targets)
    {
        List<Vector3> centers = new List<Vector3>();

        foreach (var select in targets)
        {
            Collider col = select.GetComponent<Collider>();
            float centerOffsetY = col.bounds.center.y - select.transform.position.y;
            centers.Add(select.transform.position + new Vector3(0, centerOffsetY, 0));
        }

        return centers;
    }

    protected Vector3 GetScreenCenter(GameObject target)
    {
        Collider col = target.GetComponent<Collider>();
        if (col.CompareTag("Player")) return Helper.DualManager.EnemySkillEffectPos;        // �÷��̾��� ���, �� ��ų ����Ʈ ��ġ ���

        float centerOffsetY = col.bounds.center.y - target.transform.position.y;
        Vector3 centerPos = target.transform.position + new Vector3(0, centerOffsetY, 0);
        Vector3 centerPlusPos = new Vector3(0, 2f, 0);

        return centerPos + centerPlusPos;
    }

    // ������ ����Ʈ ��ġ ��� �޼��� �߰�
    protected List<Vector3> GetEffectPositions(List<GameObject> targets, EffectPositionType posType, int range)
    {
        if (posType == EffectPositionType.PerTarget)
        {
            return GetTargetCenters(targets);
        }
        else // SingleCenter
        {
            int enemyCenter = 0;
            int[] index = { 0, 1, 1, 2, 2 };

            if (range == 5)                          // 5�� ��� �� ����
            {
                // Ÿ�� �߽� ��ġ �迭
                enemyCenter = targets.Count - 1;
            }

            // Ÿ���� �߽ɿ��� ����Ʈ
            return new List<Vector3> { GetScreenCenter(targets[index[enemyCenter]]) };
        }
    }

    protected List<GameObject> GetEffectObject(bool isPlayer, SkillMetaData data)
    {
        List<GameObject> tmp = new List<GameObject>();

        if (data.effectType == EffectPositionType.PerTarget)
        {
            tmp = Helper.PoolManager.GetEffectPool(data.range);  // range ��ŭ Ǯ ������ ����
        }
        else
        {
            tmp = Helper.PoolManager.GetEffectPool(1);  // single Ÿ���� ��� 1���� ����

        }

        // ������ ����
        Vector3 scale = isPlayer ? new Vector3(3.5f, 3.5f, 3.5f) : new Vector3(2f, 2f, 2f); // �����ڰ� �÷��̾�� 3.5, �ƴϸ� 1.2�� ����

        foreach (var obj in tmp)
        {
            obj.transform.localScale = scale;  // ����Ʈ ������Ʈ�� ������ ����
        }

        return tmp;
    }


}
