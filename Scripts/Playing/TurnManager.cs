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

    // ���� ��Ƽ�� �ִ� ĳ���͵� �ӵ��� �� ���� ����
    public List<CharStats> allCharStats = new List<CharStats>();

    // �߰� �ൿ �� ť
    public List<(CharStats charStat, SkillType type)> _extraTurn = new List<(CharStats, SkillType)>();

    private EncounterContext _context;

    public Action OnAddTurn;
    public Action OnExtraTurn;

    public void InitTurnManager(EncounterContext ct)
    {
        _context = ct;
    }


    /// <summary>
    /// ��ü �� �� ���� ����
    /// </summary>
    /// <returns></returns>
    public IEnumerator RoutineActTurn()
    {
        while(true)
        {
            SortStatsList();
            OnAddTurn?.Invoke();

            yield return StartCoroutine(RunOneTurn());

            if(_dual.HasNextEnemyWave())        // �׸� �ϴ��� ���ϴ��� üũ
            {
                break;  
            }

            _context._dual._currentTurnRound++;
        }
    }

    // �� ��(��� ĳ���� ��ȸ) ����
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
                // ���� �߰� ���� ����Ʈ �ִ��� Ȯ��
                yield return StartCoroutine(ExtraTurnUnit());
                _context._dual._buttonSelectType = SkillType.normal;
            }

            allCharStats.RemoveAt(0);                                    // �׻� ù ��°�� ó���ϹǷ� index ����
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
            GetCurUnit = extra.charStat.GetChar;            // ���� �� ĳ���ͷ� ǥ��
            SkillType type = extra.type;                    // ���� �� Ÿ�� �� ǥ��

            _context._dual._buttonSelectType = type;

            // ui ù ��°�� ������ ����..
            allCharStats.RemoveAt(0);                       // �߰� ���̱� ������ �⺻ �� �� ���� �����ֱ�
            allCharStats.Insert(0, extra.charStat);         // �߰� �� ���¸� �⺻ �� ���¿� ����־��ֱ�
            _extraTurn.RemoveAt(0);                         // �߰� �� �� �� �����ֱ�

            // ui ����
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
            case 0:         // �÷��̾�
                {
                    yield return StartCoroutine(PlayerTurn());
                    break;
                }
            default:        // 1: �Ϲ� �� 2 : ����
                {
                    yield return StartCoroutine(EnemyTurn());
                    break;
                }
        }
    }

    public void SortStatsList()                         // �� ����
    {
        allCharStats.Clear();                           //�ʱ�ȭ

        // 1. �÷��̾� ���� �߰�
        foreach (var unit in GetPartyInfo)
        {
            if (unit == null) continue;                      // �� �����̸� �ѱ��

            CharStats stats = unit.GetComponent<Character>().GetStats;

            if (stats.IsDie()) continue;
            allCharStats.Add(stats);
        }

        // 2. �� ���� �߰�
        foreach (var unit in GetEnemyInfo)
        {
            if (unit == null) continue;                      // �� �����̸� �ѱ��

            Enemy enemy = unit.GetComponent<Enemy>();
            CharStats stats = enemy.GetStats;

            if (stats.IsDie()) continue;
            allCharStats.Add(stats);
        }

        // 3. �ӵ� ���� �������� ����
        allCharStats = allCharStats.OrderByDescending(stat => stat.GetBaseStats.baseSpeed).ToList();

        // 4. �ӵ� ���� ���� ���� �� ���� ������ֱ� 150 ������ �� ���� �ൿ ����
        int counts = allCharStats.Count;

        for (int i = 0; i < counts; i++)
        {
            if (allCharStats[i].GetBaseStats.baseSpeed >= 150)
                allCharStats.Add(allCharStats[i]);
        }

        // 5. �̺�Ʈ �� �������ѳ���.
        foreach (var unit in allCharStats)
        {
            unit.GetChar.OnDie -= OnPopCharTurn;
            unit.GetChar.OnDie += OnPopCharTurn;
        }
    }

    public void OnPushExtraTurn(CharStats current, SkillType type)
    {
        var extra = (current, type);

        // Extra�� �־������
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
        Debug.Log("�÷��̾� �� " + GetCurUnit.GetStats.GetBaseStats.charName + " " + Time.deltaTime + " ");
        PlayerRoutine();                                        // �÷��̾� �� ����

        yield return new WaitUntil(() => GetCurEndTurn);      // True�� �Ǹ� �Ѿ
        yield return null;

        yield return StartCoroutine(ThisCharacterEndTurn());
    }

    public IEnumerator EnemyTurn()
    {
        Debug.Log("�� �� " + GetCurUnit.GetStats.GetBaseStats.charName);
        Character unit = GetCurUnit;

        StartTun();
        yield return null;

        yield return new WaitUntil(() => GetCurEndTurn);          // ���� �ൿ�� ���� ������ ���
        yield return null;

        yield return StartCoroutine(ThisCharacterEndTurn());

        void StartTun()
        {
            _context._encounter._camMove.CurrentCameraTurn(unit);
            _gameSceneUI._encounterUI.OnActiveRouTineUI(false);

            GetCurEndTurn = false;                                // �� ���
            GetCurUnit.OnTurnEnd += EndTurn;                      // �� ���� �̺�Ʈ ����
            unit.GetStats.UpdateFinalStats();                       // ���� ������Ʈ
            unit.Skill(unit.ReturnTargetObjects(GetPartyInfo));    // ��ų �����ؼ� ���
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
            Helper.KeySet.GetEncounterTouch = true;               // ��ī���� ��ġ Ȱ��ȭ   
            GetCurEndTurn = false;                                // �� ���
            _context._encounter._camMove.CurrentCameraTurn(GetCurUnit); // ī�޶� ����
            _context._encounterUI.OnActiveRouTineUI(true);        // ��ƾ Ȱ���� ui
            GetCurUnit.MyTurn();                                  // �ڱ� �Ͽ� �� ��
        }

        void TypeCheck()
        {
            var curType = _context._dual._buttonSelectType;

            switch (curType)
            {
                case SkillType.ult:
                    {
                        _dual.IndicateTurnChar();                             // �ε������͵� ����
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
                        _dual.IndicateTurnChar();                             // �ε������͵� ����
                        _context._encounter.GetButtonActione = false;                                  // ��ư �׼� ��Ȱ��ȭ
                        _context._encounterUI.OnFocusButton();                                         // ��ư ��Ŀ�� Ȱ��ȭ
                        break;
                    }
            }
        }
    }

    public IEnumerator ThisCharacterEndTurn()
    {
        yield return new WaitForSeconds(0.5f);

        Helper.KeySet.GetEncounterTouch = false;               // ��ī���� ��ġ Ȱ��ȭ

        // �߰� �����̳� �ñر⿡�� ���� ���� X
        if(_context._dual._buttonSelectType == SkillType.normal || _context._dual._buttonSelectType == SkillType.skill)
        {
            GetCurUnit._charBuffManager.CheckTurnBasedBuff();  // �� ��� ���� üũ
        }

        _context._dual._buttonSelectType = SkillType.normal;   // �⺻ Ÿ������ �ʱ�ȭ
        _gameSceneUI.ResetUI();                                // UI �ʱ�ȭ
        _dual.CharTurnEnd();
        _context._stage.ResetEnemyPos();                       // �� ��ġ �ʱ�ȭ

        yield return new WaitForSeconds(0.5f);
    }

}
