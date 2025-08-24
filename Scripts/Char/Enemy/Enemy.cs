using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    public EnemyType _enemyType;            // 인스펙터에서 설정

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

        yield return PlayEffect(targets, data);   // 이펙트 실행

        Helper.DualManager.UpdateSkillPoijnt(+1); // or -1 based on usage

        InvokeTurnEnd();
    }


    public override List<GameObject> ReturnTargetObjects(GameObject[] players)
    {
        List<GameObject> tmpList = new List<GameObject>();
        int centerIndex;                                                                

        while (true)
        {
            int randNum = UnityEngine.Random.Range(0, players.Length);                      // 플레이어 중 랜덤으로 하나 선택
            if (players[randNum] != null)
            {
                centerIndex = randNum;
                break;                                                                      // 추가 후 루프 탈출
            }   
        }

        SkillMetaData act = this.ReturnAct(SkillType.skill);

        // 적군이 한 명일 경우
        if (act.range == 1) { tmpList.Add(players[centerIndex]); }

        // 만약 적군 전체 공격이라면
        if (act.range == 5)
        {
            // 살아있는 적 모두 추가
            foreach (var enemy in players) if (enemy != null) tmpList.Add(enemy);
        }

        if (act.range == 3)
        {
            tmpList.Add(players[centerIndex]);                                     // 기준 오브젝트 추가
            int leftindex = centerIndex - 1;                                                    // 왼쪽 인덱스
            int rightindex = centerIndex + 1;                                                   // 오른쪽 인덱스

            if (leftindex >= 0 && players[leftindex] != null)                         // 인덱스 배열 범위 먼저 체크
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

        var effectObject = GetEffectObject(caster.CompareTag("Player"), data);      // 이펙트 오브젝트 가져오기

        var positions = GetEffectPositions(targets, data.effectType, data.range);   // 이펙트 위치 계산
        List<Animator> curAnimCon = new List<Animator>();                           // 애니메이터 저장용도

        Animator checkAnim;                                                         // 애니메이터 체크용도

        bool IsAnimEnd = false;

        // 애니메이터 실행 및 오브젝트 위치 설정
        for (int i = 0; i < effectObject.Count && i < targets.Count; i++)
        {
            var obj = effectObject[i];
            obj.transform.position = positions[i];

            // 이펙트 작업
            EffectDamage effect = obj.GetComponent<EffectDamage>();             // 공격해야 할 대상 전해주기
            effect.ResetEffectAnimator();                                       // 이펙트 애니메이터 초기화
            effect.OnPassAnimEvent(caster, targets, data, i);                   // 이펙트가 애니메이션 이벤트를 통해 공격을 전달할 수 있도록 설정

            // 애니메이터 작업
            Animator animator = obj.GetComponent<Animator>();
            animator.runtimeAnimatorController = _skillTable._animCon;                      // 현재 스킬 애니메이터 컨트롤러 가져오기

            curAnimCon.Add(animator);                                           // 애니메이터 컨트롤러를 저장 (필요시 사용 가능)
        }

        yield return new WaitForSeconds(0.1f);                                  // 애니메이션 시작 전 잠시 대기 (애니메이터가 준비될 때까지)

        for (int i = 0; i < curAnimCon.Count; i++)
        {
            string skillName = (data.skillType == SkillType.normal) ? "Normal" :
                               (data.skillType == SkillType.skill) ? "Skill" : "Ult";

            curAnimCon[i].SetTrigger(skillName);

            if (i == 0)                                                  // 애니메이터 대기를 위해 사용
            {
                checkAnim = curAnimCon[i];                               // 첫 번째 애니메이터를 체크용으로 저장
                StartCoroutine(DelayAnimator(checkAnim));             // 애니메이터 코루틴 시작
            }
        }

        yield return new WaitUntil(() => IsAnimEnd);             // 애니메이션 대기
        yield return null;

        // 이펙트 풀 리턴 해주기
        Helper.PoolManager.ReturnEffectPool(effectObject);

        IEnumerator DelayAnimator(Animator anim)
        {
            // 첫 프레임 대기 (애니메이터가 준비될 때까지)
            yield return null;

            while (true)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    IsAnimEnd = true;
                    yield break; // 애니메이션이 끝나면 코루틴 종료
                }
                yield return null; // 다음 프레임까지 대기
            }
        }
    }

    /// <summary>
    /// 타겟 센터 즉, 목표 하나에 이펙트 가운데 위치를 사용하고 싶을때 사용
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
        if (col.CompareTag("Player")) return Helper.DualManager.EnemySkillEffectPos;        // 플레이어일 경우, 적 스킬 이펙트 위치 사용

        float centerOffsetY = col.bounds.center.y - target.transform.position.y;
        Vector3 centerPos = target.transform.position + new Vector3(0, centerOffsetY, 0);
        Vector3 centerPlusPos = new Vector3(0, 2f, 0);

        return centerPos + centerPlusPos;
    }

    // 유연한 이펙트 위치 계산 메서드 추가
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

            if (range == 5)                          // 5일 경우 재 설정
            {
                // 타겟 중심 위치 배열
                enemyCenter = targets.Count - 1;
            }

            // 타겟의 중심에만 이펙트
            return new List<Vector3> { GetScreenCenter(targets[index[enemyCenter]]) };
        }
    }

    protected List<GameObject> GetEffectObject(bool isPlayer, SkillMetaData data)
    {
        List<GameObject> tmp = new List<GameObject>();

        if (data.effectType == EffectPositionType.PerTarget)
        {
            tmp = Helper.PoolManager.GetEffectPool(data.range);  // range 만큼 풀 사이즈 설정
        }
        else
        {
            tmp = Helper.PoolManager.GetEffectPool(1);  // single 타겟의 경우 1개로 설정

        }

        // 사이즈 설정
        Vector3 scale = isPlayer ? new Vector3(3.5f, 3.5f, 3.5f) : new Vector3(2f, 2f, 2f); // 시전자가 플레이어면 3.5, 아니면 1.2로 설정

        foreach (var obj in tmp)
        {
            obj.transform.localScale = scale;  // 이펙트 오브젝트의 스케일 설정
        }

        return tmp;
    }


}
