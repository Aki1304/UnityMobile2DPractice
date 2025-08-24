using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class TurnManager : MonoBehaviour
{
    private EncounterDual _dual { get { return _context._encounter._encounterDual; } }
    private GameSceneUI _gameSceneUI {  get { return _context._encounter._gameSceneUI; } }
    private bool GetCurEndTurn { get { return _dual.GetCurEndTurn; } set { _dual.GetCurEndTurn = value; } }
    private Character GetCurUnit { get { return _dual.GetCurUnit; } set { _dual.GetCurUnit = value; } }
    private GameObject[] GetPartyInfo { get { return _dual._currentParty; } }
    private GameObject[] GetEnemyInfo { get { return _dual._currentWaveEnemy; } }

    // 현재 파티에 있는 캐릭터들 속도를 한 곳에 정리
    public List<CharStats> allCharStats = new List<CharStats>();

    // 추가 행동 턴 큐
    public List<(CharStats charStat, SkillType type)> _extraTurn = new List<(CharStats, SkillType)>();

    private EncounterContext _context;

    public Action OnAddTurn;
    public Action OnExtraTurn;

    public void InitTurnManager(EncounterContext ct)
    {
        _context = ct;
    }


    /// <summary>
    /// 전체 한 턴 마다 실행
    /// </summary>
    /// <returns></returns>
    public IEnumerator RoutineActTurn()
    {
        while(true)
        {
            SortStatsList();
            OnAddTurn?.Invoke();

            yield return StartCoroutine(RunOneTurn());

            if(_dual.HasNextEnemyWave())        // 그만 하는지 안하는지 체크
            {
                break;  
            }

            _context._dual._currentTurnRound++;
        }
    }

    // 한 턴(모든 캐릭터 순회) 실행
    private IEnumerator RunOneTurn()
    {
        while (allCharStats.Count > 0)
        {
            if (_dual.LoopStats()) break;

            var stat = allCharStats[0];
            GetCurUnit = stat.GetChar;

            yield return StartCoroutine(SelectTurnUnit(stat.GetUnitCode));

            if(_extraTurn.Count != 0)
            {
                // 먼저 추가 공격 리스트 있는지 확인
                yield return StartCoroutine(ExtraTurnUnit());
                _context._dual._buttonSelectType = SkillType.normal;
            }

            allCharStats.RemoveAt(0);                                    // 항상 첫 번째를 처리하므로 index 안전
            OnAddTurn?.Invoke();

            yield return null;
        }
    }

    public IEnumerator ExtraTurnUnit()
    {
        while(_extraTurn.Count != 0)
        {   
            if (_dual.LoopStats()) break;

            var extra = _extraTurn[0];
            GetCurUnit = extra.charStat.GetChar;            // 현재 턴 캐릭터로 표시
            SkillType type = extra.type;                    // 현재 턴 타입 값 표시

            _context._dual._buttonSelectType = type;

            // ui 첫 번째로 변경을 위해..
            allCharStats.RemoveAt(0);                       // 추가 턴이기 때문에 기본 턴 맨 위에 지워주기
            allCharStats.Insert(0, extra.charStat);         // 추가 턴 상태를 기본 턴 상태에 집어넣어주기
            _extraTurn.RemoveAt(0);                         // 추가 턴 맨 앞 지워주기

            // ui 정리
            OnAddTurn?.Invoke();
            OnExtraTurn?.Invoke();

            yield return null;

            yield return StartCoroutine(SelectTurnUnit(extra.charStat.GetUnitCode));
        }
    }


    public IEnumerator SelectTurnUnit(int code)
    {
        switch (code)
        {
            case 0:         // 플레이어
                {
                    yield return StartCoroutine(PlayerTurn());
                    break;
                }
            default:        // 1: 일반 적 2 : 보스
                {
                    yield return StartCoroutine(EnemyTurn());
                    break;
                }
        }
    }

    public void SortStatsList()                         // 턴 로직
    {
        allCharStats.Clear();                           //초기화

        // 1. 플레이어 유닛 추가
        foreach (var unit in GetPartyInfo)
        {
            if (unit == null) continue;                      // 빈 공간이면 넘기기

            CharStats stats = unit.GetComponent<Character>().GetStats;

            if (stats.IsDie()) continue;
            allCharStats.Add(stats);
        }

        // 2. 적 유닛 추가
        foreach (var unit in GetEnemyInfo)
        {
            if (unit == null) continue;                      // 빈 공간이면 넘기기

            Enemy enemy = unit.GetComponent<Enemy>();
            CharStats stats = enemy.GetStats;

            if (stats.IsDie()) continue;
            allCharStats.Add(stats);
        }

        // 3. 속도 기준 내림차순 정렬
        allCharStats = allCharStats.OrderByDescending(stat => stat.GetBaseStats.baseSpeed).ToList();

        // 4. 속도 기준 정렬 이후 한 번더 계산해주기 150 넘으면 한 번더 행동 가능
        int counts = allCharStats.Count;

        for (int i = 0; i < counts; i++)
        {
            if (allCharStats[i].GetBaseStats.baseSpeed >= 150)
                allCharStats.Add(allCharStats[i]);
        }

        // 5. 이벤트 다 구독시켜놓기.
        foreach (var unit in allCharStats)
        {
            unit.GetChar.OnDie -= OnPopCharTurn;
            unit.GetChar.OnDie += OnPopCharTurn;
        }
    }

    public void OnPushExtraTurn(CharStats current, SkillType type)
    {
        var extra = (current, type);

        // Extra에 넣어버리기
        _extraTurn.Add(extra);
        OnExtraTurn?.Invoke();
    }

    public void OnPopCharTurn(CharStats deadChar)
    {
        allCharStats.RemoveAll(stat => stat == deadChar);
        Character unit = deadChar.GetChar;
        unit.OnDie -= OnPopCharTurn;
    }

    public IEnumerator PlayerTurn()
    {
        Debug.Log("플레이어 턴 " + GetCurUnit.GetStats.GetBaseStats.charName + " " + Time.deltaTime + " ");
        PlayerRoutine();                                        // 플레이어 턴 세팅

        yield return new WaitUntil(() => GetCurEndTurn);      // True가 되면 넘어감
        yield return null;

        yield return StartCoroutine(ThisCharacterEndTurn());
    }

    public IEnumerator EnemyTurn()
    {
        Debug.Log("적 턴 " + GetCurUnit.GetStats.GetBaseStats.charName);
        Character unit = GetCurUnit;

        StartTun();
        yield return null;

        yield return new WaitUntil(() => GetCurEndTurn);          // 적이 행동을 끝낼 때까지 대기
        yield return null;

        yield return StartCoroutine(ThisCharacterEndTurn());

        void StartTun()
        {
            _context._encounter._camMove.CurrentCameraTurn(unit);
            _gameSceneUI._encounterUI.OnActiveRouTineUI(false);

            GetCurEndTurn = false;                                // 턴 대기
            GetCurUnit.OnTurnEnd += EndTurn;                      // 턴 종료 이벤트 구독
            unit.GetStats.UpdateFinalStats();                       // 스탯 업데이트
            unit.Skill(unit.ReturnTargetObjects(GetPartyInfo));    // 스킬 선택해서 사용
        }

        void EndTurn()
        {
            GetCurEndTurn = true;
            unit.OnTurnEnd -= EndTurn;
        }
    }

    public void PlayerRoutine()
    {
        EssentialRoutine();
        TypeCheck();

        void EssentialRoutine()
        {
            Helper.KeySet.GetEncounterTouch = true;               // 인카운터 터치 활성화   
            GetCurEndTurn = false;                                // 턴 대기
            _context._encounter._camMove.CurrentCameraTurn(GetCurUnit); // 카메라 설정
            _context._encounterUI.OnActiveRouTineUI(true);        // 루틴 활용한 ui
            GetCurUnit.MyTurn();                                  // 자기 턴에 할 일
        }

        void TypeCheck()
        {
            var curType = _context._dual._buttonSelectType;

            switch (curType)
            {
                case SkillType.ult:
                    {
                        _dual.IndicateTurnChar();                             // 인디케이터들 세팅
                        _context._encounter.GetButtonActione = false;
                        _context._encounterUI.ActiveAllButton(false);
                        _context._encounterUI.ActiveButton(true, 1);
                        break;
                    }

                case SkillType.extra:
                    {
                        _context._targetSelecter.ExtraTurn();
                        break;
                    }

                default:
                    {
                        _context._dual._buttonSelectType = SkillType.normal;
                        _dual.IndicateTurnChar();                             // 인디케이터들 세팅
                        _context._encounter.GetButtonActione = false;                                  // 버튼 액션 비활성화
                        _context._encounterUI.OnFocusButton();                                         // 버튼 포커스 활성화
                        break;
                    }
            }
        }
    }

    public IEnumerator ThisCharacterEndTurn()
    {
        yield return new WaitForSeconds(0.5f);

        Helper.KeySet.GetEncounterTouch = false;               // 인카운터 터치 활성화

        // 추가 공격이나 궁극기에는 버프 차감 X
        if(_context._dual._buttonSelectType == SkillType.normal || _context._dual._buttonSelectType == SkillType.skill)
        {
            GetCurUnit._charBuffManager.CheckTurnBasedBuff();  // 턴 기반 버프 체크
        }

        _context._dual._buttonSelectType = SkillType.normal;   // 기본 타입으로 초기화
        _gameSceneUI.ResetUI();                                // UI 초기화
        _dual.CharTurnEnd();
        _context._stage.ResetEnemyPos();                       // 적 위치 초기화

        yield return new WaitForSeconds(0.5f);
    }

}
