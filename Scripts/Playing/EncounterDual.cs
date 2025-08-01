using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EncounterDual : MonoBehaviour
{
    [Header("���� �� ĳ���� ǥ�� ������Ʈ")]
    [SerializeField] public GameObject _indicaterTurn;

    [HideInInspector] public int[] _waveEnemyCount;               // 0 index => 1 ���� 1 index -> 2 ���� 2 ... 
    [HideInInspector] public int _currentWaveCount;
    [HideInInspector] public int _currentTurnRound;               // �� �������� ǥ�� �뵵..

    // ���� ���̺� �� �� ����
    [HideInInspector] public GameObject[] _currentWaveEnemy;
    [HideInInspector] public GameObject[] _currentParty;

    [SerializeField] private Character _currentChar;
    public Character GetCurUnit { get { return _currentChar; } set { _currentChar = value; } }

    private BattleState _currenteState = BattleState.None;
    public BattleState GetBattleState { get { return _currenteState; } }

    public SkillType _buttonSelectType { get; set; }

    private bool _currentEndTurn;
    public bool GetCurEndTurn { get { return _currentEndTurn; } set{ _currentEndTurn = value; } }

    private List<GameObject> targetList = new List<GameObject>();               // ���� ��� ����Ʈ
    public List<GameObject> GetTargetList { get { return targetList; } }        // ���� ��� ����Ʈ ��ȯ

    // context
    private EncounterContext _context;


    public void InitDual(EncounterContext ct)
    {
        _context = ct;

        _waveEnemyCount = new int[3];           // �� ��ī���� �� �ִ� ����� 3

        _currentWaveEnemy = new GameObject[5];
        _currentParty = new GameObject[4];
    }

    public void ResetEncounterDual()
    {
        _currentWaveCount = 0;
        _context._encounterUI._allDamageUI.ResetDamage();     // �� ������ ����
        Array.Clear(_currentWaveEnemy, 0, _currentWaveEnemy.Length);
    }

    public void StartEncounterDual()
    {
        int curRound = Helper.DualManager._currentRound;                     // ���̺� ���� ���� �����
        _waveEnemyCount = WaveData.GetEnemyCount(curRound);                  // ���忡 �´� wave ���� ����
        _currentTurnRound = 0;                                               // 0���� �ʱⰪ

        Debug.Log($"{_waveEnemyCount[0]} {_waveEnemyCount[1]} {_waveEnemyCount[2]}");
    }

    /// <summary>
    /// ���̺� �����ϱ�
    /// </summary>
    public void SetEncounterWave()
    {
        _currentEndTurn = false;                                                            // ������ ���� üũ �� �� 

        int curSkillPoint = (_currentWaveCount == 0) ? 2 : Helper.DualManager.GetSP;       // 0�� ���� ���� �� �־��ֱ�
        Helper.DualManager.InitSkill(curSkillPoint);                                       // �־��ֱ� ��ų����Ʈ

        EnemyType currentType = (Helper.DualManager._currentRound % 5 == 0) ? EnemyType.Boss : EnemyType.Normal;   // �� Ÿ�� 
        List<GameObject> enemyObject = Helper.CharacterManager.ReturnTypeEnemy(currentType);

        for(int i = 0; i < _waveEnemyCount[_currentWaveCount]; i++)
        {
            int ranomPick = UnityEngine.Random.Range(0, enemyObject.Count);     // ���� �� �ϱ�
            _currentWaveEnemy[i] = enemyObject[ranomPick];                      // ���� �� �ֱ�
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

        // ��Ƽ �Ŵ��� �� ���� �迭 ����ȭ
        Helper.PartyManager.EndUpdateParty(_currentParty);

        // ������Ʈ�� �迭 �ʱ�ȭ
        Array.Clear(_currentParty, 0, _currentParty.Length);                // ���� ��Ƽ
        Array.Clear(_currentWaveEnemy, 0, _currentWaveEnemy.Length);        // ���� �� 
    }


    public void IndicateTurnChar()
    {
        int[] indexs = new int[5] { 0, 1, 1, 2, 2 };                                         // ���� ���� ���� ��� ��ġ
        int counts = _currentWaveEnemy.Count(obj => obj != null);                            // ���� ���̺꿡 �ִ� �� ��
        int pos = indexs[counts - 1];                                                        // ���� �� ���� ���� ��� ��ġ
        Debug.Log($"���� �� �� : {counts} ���� ��� ��ġ : {pos}");
        Character target = _currentWaveEnemy[pos].GetComponent<Character>();                 // �׻� ����� ����


        _context._sceneUI.GetInputSelect.SetFirstChar(target);

        // �÷��̾� ��ġ �����
        Collider col = _currentChar.gameObject.GetComponent<Collider>();

        Vector3 charPos = _currentChar.gameObject.transform.position;
        Vector3 uiPos = new Vector3(0, col.bounds.size.y + 0.2f, 0);

        _indicaterTurn.transform.position = charPos + uiPos;

        _indicaterTurn.gameObject.SetActive(true);
    }

    public void CharTurnEnd()
    {
        targetList.Clear();                                         // Ÿ�� ����Ʈ �ʱ�ȭ
        _indicaterTurn.gameObject.SetActive(false);
    }


    #region     TurnManager

    public bool HasNextEnemyWave()
    {
        if (!LoopStats()) return false;

        int nextWaveIndex = _currentWaveCount + 1;
        bool isEndOfWaves = nextWaveIndex >= _waveEnemyCount.Length;

        // ���� ���̺꿡 ���� ���� ������ ���尡 �����ٸ�
        if(isEndOfWaves)
        {
            _currenteState = BattleState.Win;
            return true;
        }

        if (_waveEnemyCount[nextWaveIndex] > 0)          // ���� ���̺갡 �����Ѵٸ� !
        {
            _currentWaveCount += 1;
            return true;
        }
        else                                             // ���� ���̺갡 �������� �ʴ´ٸ�
        {
            _currenteState = BattleState.Win;
            return true;
        }

    }

    public bool LoopStats()
    {
        if (CanMoveUnitPlayerCount() == 0)
        {
            Debug.Log("�÷��̾� ��� ���");
            _currenteState = BattleState.Lose;
            return true;
        }

        if (CanMoveUnityEnemyCount() == 0)
        {
            Debug.Log("�� ���� ���� - ���̺� ���� ���� Ȯ��");
            return true;
        }

        return false;
    }

    int CanMoveUnitPlayerCount()
    {
        int count = 0;

        // �Ʊ� ��Ƽ
        foreach (var unit in _currentParty)
        {
            if (unit != null) count++;            // �׾��ٸ� 0 �ƴϸ� ���ϱ� 1
        }

        return count;
    }
    int CanMoveUnityEnemyCount()
    {
        int count = 0;

        // �Ʊ� ��Ƽ
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
        yield return null; // 1 ������ ���

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


