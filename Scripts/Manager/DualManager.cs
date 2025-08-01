using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UIElements;

public class DualManager : MonoBehaviour
{
    // 게임 현재 흐름 정리
    public const int MaxRound = 25;         // 총 전체 스테이지 라운드 수
    public int _currentRound = 0;           // 전체 스테이지 중 현재 라운드

    public Action OnChangeSP;

    // 이벤트 구독을 위해 Encounter 연결
    public EncounterContext _context { get; set; }
    public GameObject[] _instanceParty { get; set; }    


    #region 스킬 포인트 관련
    private int _skillPoint;
    public int GetSP { get { return _skillPoint; } }
    public void InitSkill(int use) => _skillPoint = use;
    public void UpdateSkillPoijnt(int use)
    {
        if (_skillPoint + use > 5) _skillPoint = 5;
        if (_skillPoint + use < 0) _skillPoint = 0;
        else _skillPoint += use;

        OnChangeSP?.Invoke(); 
    }

    public bool CanSkillPoint(int use)
    {
        bool answer;

        if (_skillPoint - use < 0) answer = false;
        else answer = true;

        return answer;
    }
    #endregion


    private Vector3 _enemySkillEffectPos;    // 적 스킬 이펙트 위치
    public Vector3 EnemySkillEffectPos
    {
        get { return _enemySkillEffectPos; }
        set { _enemySkillEffectPos = value; }
    }

    // 소수점 계산 캐싱
    public float ReturnFormulaNumber(float value) => Mathf.Round(value * 10f) / 10f;

    public void GetInitDual(GameObject[] instanceParty)
    {
        _instanceParty = instanceParty;     // 인스턴스 파티에 참조
        OnSubsPassive(_instanceParty);
    }

    // 듀얼 매니저에서 이벤트 관측
    public void OnSubsPassive(GameObject[] unitObjects)
    {
        // 추가 스택
        foreach(var obj in unitObjects)
        {
            Character unit = obj.GetComponent<Character>();
            IPassiveExtraAttack extra = unit as IPassiveExtraAttack;

            if (unit is null || extra is null) continue;

            _context._encounterUI.OnStackRoutineUI(unit);
        }

        List<IBuffInfo> tempBuffList = new List<IBuffInfo>();

        // 버프 패시브 임시 리스트에 넣기
        foreach(var obj in unitObjects)
        {
            Character unit = obj.GetComponent<Character>();
            List<IBuffInfo> listBuff = unit._charBuffManager.GetActiveBuffs;

            // 캐릭터가 갖고 있는 버프 중 살아있으면 적용되는 버프가 있다면
            foreach(var buff in listBuff)
            {
                if(buff.durationType == BuffDurationType.WhileAlive)
                {
                    Debug.Log(buff);
                    tempBuffList.Add(buff);
                }
            }
        }

        // 버프 적용
        foreach(var obj in unitObjects)
        {
            Character unit = obj.GetComponent<Character>();
            foreach(var buff in tempBuffList)
            {
                unit._charBuffManager.AddBuff(buff);
            }
        }

    }

    public void BrokerPassiveType(List<GameObject> target)
    {
        // 어택 타입 확인 후 패시브에 맞다면 실행 시키기
        // 그냥 모든 캐릭터 한테 알리는거야 ~ 어 나 상대한테 데미지주는 스킬을 썻어~ 라고.
        foreach(var obj in _instanceParty)
        {
            if(obj is null) continue;

            Character unit = obj.GetComponent<Character>();
            if(unit is IPassiveExtraAttack extra)
            {
                extra.OnExtraAttack(target);
            }
        }
    }

    public void BrokerExtraTurnAdd(Character unit)
    {
        // 브로커를 통해 Turnmanager에 넣으려고하는데
        _context._turnManager.OnPushExtraTurn(unit.GetStats, SkillType.extra);
    }
}
