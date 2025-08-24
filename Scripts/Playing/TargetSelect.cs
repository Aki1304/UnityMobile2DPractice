using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class TargetSelector : MonoBehaviour
{
    private EncounterContext _context;

    private Character GetCurUnit { get { return _context._dual.GetCurUnit; } } 
    private List<GameObject> targetList { get { return _context._dual.GetTargetList; } }
    private GameObject[] GetPartyInfo { get { return _context._dual._currentParty; } } 
    private GameObject[] GetEnemyInfo { get {  return _context._dual._currentWaveEnemy; } }

    public void InitTargetSelector(EncounterContext ct)
    {
        _context = ct;

        // 이벤트 구독
        _context._sceneUI.GetInputSelect.OnChangeCursor += OnSelectEvent;        // 커서 이동 이벤트 구독
    }

    public void EndDual()
    {

        // 이벤트 구독 해제
        _context._sceneUI.GetInputSelect.OnChangeCursor -= OnSelectEvent;  // 커서 이동 이벤트 구독 해제
    }

    public void OnSelectEvent()
    {
        // 스킬 타입이 아군인지 적인지 구분
        SkillMetaData data = GetCurUnit.ReturnAct(_context._dual._buttonSelectType);
        SkillActionType type = data.actionType;


        if(type == SkillActionType.Attack)         // 스킬 타입이 공격이라면
        {
            _context._sceneUI.GetInputSelect.CanSelectPlayer = false;    // 플레이어 선택 불가
            _context._encounter._camMove.CurrentCameraTurn(GetCurUnit);  // 카메라 설정
        }
        else                                        // 스킬 타입이 공격이 아니라면
        {
            _context._sceneUI.GetInputSelect.CanSelectPlayer = true;    // 플레이어 선택 가능
            _context._encounter._camMove.OnHealAndBuffCamera();  // 힐 및 버프 카메라 설정
        }

        SetTargetList(type);                                                                        // 타겟 리스트 설정
        OnCursorChange();
    }

    public void SelectButton(int chooseType)
    {
        Character target = _context._encounter.GetSelectUnit();
        Character unit = GetCurUnit;

        SkillType selectType = (SkillType)chooseType;
        SkillMetaData data = unit.ReturnAct(selectType);

        if (_context._encounter.GetButtonActione) return;                          // 버튼 액션이 활성화 되어 있으면 중단

        if (_context._dual._buttonSelectType != selectType)
        {
            _context._dual._buttonSelectType = selectType;                          // 버튼 타입 변경
            OnSelectEvent();                                                        // 타겟 리스트 설정
            _context._encounterUI.OnFocusButton();                                  // 버튼 포커스 활성화
            return;                                                                 // 버튼 타입이 다르면 리턴
        }

        if (_context._dual._buttonSelectType == selectType)
        {
            Helper.GM.GetTouchKeySet.GetEncounterTouch = false;
            _context._encounterUI.ActiveCursor(false);                              // 커서 비활성화
            _context._encounterUI.ActiveAllButton(false);                           // 버튼 비활성화
            _context._encounterUI._allDamageUI.ResetDamage();                       // 데미지 리셋
            ActButton(data);                                                            // 스킬 범위 설정 및 타겟 전달
        }

    }

    public void UltimateButton(int idx)
    {
        GameObject objects = _context._dual._currentParty[idx];
        Character unit = objects.GetComponent<Character>();
        CharStats stat = unit.GetStats;

        // 궁극기 게이지 체크
        if (stat.GetCurrentUlt != stat.GetBaseStats.baseMaxUlt) { Debug.Log(  $" {unit.name}궁극기 게이지 다 안참"); return; }

        // 궁극기 버튼 턴 매니저에게 끼워넣기로 해달라하기.
        _context._turnManager.OnPushExtraTurn(stat, SkillType.ult);
    }

    public void ExtraTurn()
    {
        _context._encounterUI._allDamageUI.ResetDamage();                       // 데미지 리셋

        GetCurUnit.OnTurnEnd += EndTurn;
        IPassiveExtraAttack extra = GetCurUnit as IPassiveExtraAttack;
        if (extra == null) 
        {
            GetCurUnit.OnTurnEnd -= EndTurn;
            return;
        }

        extra.ActAttack(_context._dual._currentWaveEnemy);
    }

    public void ActButton(SkillMetaData data)
    {
        GetCurUnit.OnTurnEnd += EndTurn;

        // 어택 타입인지 체크 후 실행
        if (data.actionType == SkillActionType.Attack)
        {
            Helper.DualManager.BrokerPassiveType(targetList);
        }

        // 실제 공격 실행
        if (_context._dual._buttonSelectType == SkillType.normal) GetCurUnit.NormalAttack(targetList);
        if (_context._dual._buttonSelectType == SkillType.skill) GetCurUnit.Skill(targetList);
        if (_context._dual._buttonSelectType == SkillType.ult) GetCurUnit.Ultimate(targetList);
    }

    // 현재 턴이 끝난다면
    public void EndTurn()
    {
        _context._dual.GetCurEndTurn = true;
        GetCurUnit.OnTurnEnd -= EndTurn;
    }

    public void SetTargetList(SkillActionType type)
    {
        targetList.Clear();                                                                     // 타겟 리스트 초기화

        Character target = _context._encounter.GetSelectUnit();
        Character unit = GetCurUnit;

        SkillMetaData act = unit.ReturnAct(_context._dual._buttonSelectType);
        int centerIndex = (type == SkillActionType.Attack) 
            ? Array.IndexOf(GetEnemyInfo, target.gameObject) : Array.IndexOf(GetPartyInfo, target.gameObject);

        switch (act.range)
        {
            case 1:
                { 
                    if(type == SkillActionType.Attack) targetList.Add(GetEnemyInfo[centerIndex]);
                    else targetList.Add(GetPartyInfo[centerIndex]);

                    break;
                }

            case 3:
                {
                    targetList.Add(GetEnemyInfo[centerIndex]);                                     // 기준 오브젝트 추가
                    int leftindex = centerIndex - 1;                                               // 왼쪽 인덱스
                    int rightindex = centerIndex + 1;                                              // 오른쪽 인덱스

                    if (leftindex >= 0 && GetEnemyInfo[leftindex] != null)                         // 인덱스 배열 범위 먼저 체크
                    {
                        targetList.Add(GetEnemyInfo[leftindex]);
                    }

                    if (rightindex < GetEnemyInfo.Length && GetEnemyInfo[rightindex] != null)
                    {
                        targetList.Add(GetEnemyInfo[rightindex]);
                    }

                    break;
                }

            case 4:     // 4인은 예외 없이 플레이어 기준
                { foreach (var party in GetPartyInfo) if (party != null) targetList.Add(party); break; }

            case 5:
                { foreach (var enemy in GetEnemyInfo) if (enemy != null) targetList.Add(enemy); break; }

            default : throw new ArgumentOutOfRangeException(nameof(act.range), "기존 정해진 스킬 범위를 벗어났습니다.");
        }
    }

    public void OnCursorChange()
    {
        _context._encounterUI.OnCursorMove();        // 커서 변경
    }
}
