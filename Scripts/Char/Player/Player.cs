using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;

public abstract class Player : Character
{
    public abstract override void NormalAttack(List<GameObject> targets);
    public abstract override void Skill(List<GameObject> targets);
    public abstract override void Ultimate(List<GameObject> targets);
    public abstract override void Passive();

    protected IEnumerator WaitMotion(List<GameObject> targets, SkillMetaData data)
    {
        SkillType type = data.skillType;

        if(type == SkillType.skill)
            Helper.DualManager.UpdateSkillPoijnt(-1); // or -1 based on usage
        else if (type == SkillType.normal)
            Helper.DualManager.UpdateSkillPoijnt(+1); // or -1 based on usage

        yield return PlayEffect(targets, data);   // 이펙트 실행


        InvokeTurnEnd();
    }
    // 재사용할 공통 로직
    // PlayEffect에 위치 타입 파라미터 추가
    protected IEnumerator PlayEffect(List<GameObject> targets, SkillMetaData data)
    {
        GameObject caster = this.gameObject;
        Character unit = caster.GetComponent<Character>();

        yield return null;

        unit.PlusUlt(CallbackUltPercent(data.skillType));                           // 캐릭터의 현재 궁극기 게이지 증가

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
            effect.OnPassAnimEvent(caster, targets, data);                      // 이펙트가 애니메이션 이벤트를 통해 공격을 전달할 수 있도록 설정

            // 애니메이터 작업
            Animator animator = obj.GetComponent<Animator>();
            animator.runtimeAnimatorController = _skillTable._animCon;                      // 현재 스킬 애니메이터 컨트롤러 가져오기

            curAnimCon.Add(animator);                                           // 애니메이터 컨트롤러를 저장 (필요시 사용 가능)
        }

        yield return new WaitForSeconds(0.1f);                                  // 애니메이션 시작 전 잠시 대기 (애니메이터가 준비될 때까지)

        for (int i = 0; i < curAnimCon.Count; i++)
        {
            string skillName = (data.skillType == SkillType.normal) ? "Normal" :
                   (data.skillType == SkillType.skill) ? "Skill" :
                   (data.skillType == SkillType.passive) ? "Passive" :
                   "Ult";

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

    private int CallbackUltPercent(SkillType type)
    {
        int ultPercent = 0;

        // 궁극기 퍼센트 콜백
        if (type == SkillType.normal) ultPercent = 12;
        if (type == SkillType.skill) ultPercent = 20;
        if (type == SkillType.ult) ultPercent = -500;

        return ultPercent;
    }
}
