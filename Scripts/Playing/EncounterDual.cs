using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EncounterDual : MonoBehaviour
{
    [Header("현재 턴 캐릭터 표시 오브젝트")]
    [SerializeField] public GameObject _indicaterTurn;

    [HideInInspector] public int[] _waveEnemyCount;               // 0 index => 1 라운드 1 index -> 2 라운드 2 ... 
    [HideInInspector] public int _currentWaveCount;
    [HideInInspector] public int _currentTurnRound;               // 턴 지나간거 표시 용도..

    // 현재 웨이브 내 적 관리
    [HideInInspector] public GameObject[] _currentWaveEnemy;
    [HideInInspector] public GameObject[] _currentParty;

    [SerializeField] private Character _currentChar;
    public Character GetCurUnit { get { return _currentChar; } set { _currentChar = value; } }

    private BattleState _currenteState = BattleState.None;
    public BattleState GetBattleState { get { return _currenteState; } }

    public SkillType _buttonSelectType { get; set; }

    private bool _currentEndTurn;
    public bool GetCurEndTurn { get { return _currentEndTurn; } set{ _currentEndTurn = value; } }

    private List<GameObject> targetList = new List<GameObject>();               // 공격 대상 리스트
    public List<GameObject> GetTargetList { get { return targetList; } }        // 공격 대상 리스트 반환

    // context
    private EncounterContext _context;


    public void InitDual(EncounterContext ct)
    {
        _context = ct;

        _waveEnemyCount = new int[3];           // 한 인카운터 당 최대 라운드는 3

        _currentWaveEnemy = new GameObject[5];
        _currentParty = new GameObject[4];
    }

    public void ResetEncounterDual()
    {
        _currentWaveCount = 0;
        _context._encounterUI._allDamageUI.ResetDamage();     // 총 데미지 리셋
        Array.Clear(_currentWaveEnemy, 0, _currentWaveEnemy.Length);
    }

    public void StartEncounterDual()
    {
        int curRound = Helper.DualManager._currentRound;                     // 웨이브 세팅 현재 라운드로
        _waveEnemyCount = WaveData.GetEnemyCount(curRound);                  // 라운드에 맞는 wave 적들 생성
        _currentTurnRound = 0;                                               // 0으로 초기값

        Debug.Log($"{_waveEnemyCount[0]} {_waveEnemyCount[1]} {_waveEnemyCount[2]}");
    }

    /// <summary>
    /// 웨이브 구성하기
    /// </summary>
    public void SetEncounterWave()
    {
        _currentEndTurn = false;                                                            // 끝나는 시점 체크 불 값 

        int curSkillPoint = (_currentWaveCount == 0) ? 2 : Helper.DualManager.GetSP;       // 0일 때는 기초 값 넣어주기
        Helper.DualManager.InitSkill(curSkillPoint);                                       // 넣어주기 스킬포인트

        EnemyType currentType = (Helper.DualManager._currentRound % 5 == 0) ? EnemyType.Boss : EnemyType.Normal;   // 적 타입 
        List<GameObject> enemyObject = Helper.CharacterManager.ReturnTypeEnemy(currentType);

        for(int i = 0; i < _waveEnemyCount[_currentWaveCount]; i++)
        {
            int ranomPick = UnityEngine.Random.Range(0, enemyObject.Count);     // 랜덤 픽 하기
            _currentWaveEnemy[i] = enemyObject[ranomPick];                      // 랜덤 적 넣기
        }

        foreach (var obj in _currentParty)
        {
            if (obj is null) continue;
            Character unit = obj.GetComponent<Character>();
            unit.OnDie += OnDieUnits;
        }
    }

    public void EndDual()
    {
        _indicaterTurn.gameObject.SetActive(false);

        // 파티 매니저 및 기존 배열 동기화
        Helper.PartyManager.EndUpdateParty(_currentParty);

        // 오브젝트들 배열 초기화
        Array.Clear(_currentParty, 0, _currentParty.Length);                // 현재 파티
        Array.Clear(_currentWaveEnemy, 0, _currentWaveEnemy.Length);        // 현재 적 
    }


    public void IndicateTurnChar()
    {
        int[] indexs = new int[5] { 0, 1, 1, 2, 2 };                                         // 몬스터 수에 따른 가운데 위치
        int counts = _currentWaveEnemy.Count(obj => obj != null);                            // 현재 웨이브에 있는 적 수
        int pos = indexs[counts - 1];                                                        // 현재 적 수에 따른 가운데 위치
        Debug.Log($"현재 적 수 : {counts} 현재 가운데 위치 : {pos}");
        Character target = _currentWaveEnemy[pos].GetComponent<Character>();                 // 항상 가운데로 고정


        _context._sceneUI.GetInputSelect.SetFirstChar(target);

        // 플레이어 위치 찍어줌
        Collider col = _currentChar.gameObject.GetComponent<Collider>();

        Vector3 charPos = _currentChar.gameObject.transform.position;
        Vector3 uiPos = new Vector3(0, col.bounds.size.y + 0.2f, 0);

        _indicaterTurn.transform.position = charPos + uiPos;

        _indicaterTurn.gameObject.SetActive(true);
    }

    public void CharTurnEnd()
    {
        targetList.Clear();                                         // 타겟 리스트 초기화
        _indicaterTurn.gameObject.SetActive(false);
    }


    #region     TurnManager

    public bool HasNextEnemyWave()
    {
        if (!LoopStats()) return false;

        int nextWaveIndex = _currentWaveCount + 1;
        bool isEndOfWaves = nextWaveIndex >= _waveEnemyCount.Length;

        // 다음 웨이브에 적이 없는 마지막 라운드가 끝났다면
        if(isEndOfWaves)
        {
            _currenteState = BattleState.Win;
            return true;
        }

        if (_waveEnemyCount[nextWaveIndex] > 0)          // 다음 웨이브가 존재한다면 !
        {
            _currentWaveCount += 1;
            return true;
        }
        else                                             // 다음 웨이브가 존재하지 않는다면
        {
            _currenteState = BattleState.Win;
            return true;
        }

    }

    public bool LoopStats()
    {
        if (CanMoveUnitPlayerCount() == 0)
        {
            Debug.Log("플레이어 모두 사망");
            _currenteState = BattleState.Lose;
            return true;
        }

        if (CanMoveUnityEnemyCount() == 0)
        {
            Debug.Log("적 유닛 없음 - 웨이브 종료 여부 확인");
            return true;
        }

        return false;
    }

    int CanMoveUnitPlayerCount()
    {
        int count = 0;

        // 아군 파티
        foreach (var unit in _currentParty)
        {
            if (unit != null) count++;            // 죽었다면 0 아니면 더하기 1
        }

        return count;
    }
    int CanMoveUnityEnemyCount()
    {
        int count = 0;

        // 아군 파티
        foreach (var unit in _currentWaveEnemy)
        {
            if (unit != null) count++;
        }

        return count;
    }

    public void OnDieUnits(CharStats stats)
    {
        StartCoroutine(DelayRemoveFromParty(stats));
    }

    private IEnumerator DelayRemoveFromParty(CharStats stats)
    {
        yield return null; // 1 프레임 대기

        GameObject unit = stats.GetChar.gameObject;
        Character unitChar = stats.GetChar;

        for (int i = 0; i < _currentParty.Length; i++)
        {
            if (unit == _currentParty[i]) _currentParty[i] = null;
        }

        unitChar.OnDie -= OnDieUnits;
    }



    #endregion
}


