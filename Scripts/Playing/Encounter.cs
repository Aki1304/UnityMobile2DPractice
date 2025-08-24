using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterContext
{
    public Encounter _encounter;
    public EncounterDual _dual;
    public EncounterStage _stage;
    public GameSceneUI _sceneUI;
    public EncounterUI _encounterUI;
    public TurnManager _turnManager;
    public TargetSelector _targetSelecter;
}


public class Encounter : MonoBehaviour
{
    // context ��ġ
    public EncounterDual _encounterDual;
    public EncounterStage _encounterStage;
    public TurnManager _turnManager;
    public TargetSelector _targetSelector;
    public GameSceneUI _gameSceneUI;

    private EncounterContext _context;

    // context �ƴ�
    public CameraMove _camMove;

    public GameObject[] GetPartyInfo { get { return _encounterDual._currentParty; } }
    public Character GetSelectUnit() => _gameSceneUI.GetSelectUnit;


    #region ��ư UI �ִϸ��̼� ���� �뵵
    public bool GetButtonActione { get; set; }
    #endregion

    void Awake()
    {
        InitEncounter();
    }

    void Start()
    {
        _encounterStage.SetInitCharPos();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Helper.DualManager._currentRound += 1;
            StartCoroutine(BattleEncounter());
        }
    }

    public void InitEncounter()
    {
        PassEncounter();
        OtherClassInit();
        PoolSetting();

    }

    void OtherClassInit()
    {
        _encounterDual.InitDual(_context);
        _encounterStage.InitStage(_context);
        _gameSceneUI.InitSceneUI(_context);
        _turnManager.InitTurnManager(_context);
        _targetSelector.InitTargetSelector(_context);

        // �̺�Ʈ ������ ���� ����
        Helper.DualManager._context = _context;
    }

    void PassEncounter()
    {
        _context = new EncounterContext();

        _context._encounter = this;
        _context._dual = _encounterDual;
        _context._stage = _encounterStage;
        _context._turnManager = _turnManager;
        _context._sceneUI = _gameSceneUI;
        _context._encounterUI = _gameSceneUI._encounterUI;
        _context._targetSelecter = _targetSelector;
    }

    void PoolSetting()
    {
        Helper.PoolManager.InitEncounterPool(_context._dual._poolEffectParent,_context._dual._poolFontParent);
    }

    public IEnumerator BattleEncounter()
    {
        Debug.Log("���� ����");
        EncounterInit();
        yield return null;

        while (CanEndDual())                             // ���� �� ����
        {
            EncounterWave();
            yield return StartCoroutine(_turnManager.RoutineActTurn());          // ���� �� ���̺꿡�� �� ���θ� �̷� ����

            Debug.Log(_encounterDual.GetBattleState);
        }

        EncounterEnd();
    }

    private void EncounterInit()
    {
        _encounterDual.ResetEncounterDual();              // ���� �� ���� ����
        _encounterDual.StartEncounterDual();              // ��ī���ͽ� ���̺� ����Ʈ �̸� �־�α�. 
        _encounterStage.SetEncounterPlayerPos();          // �÷��̾� ���� �� ��Ʋ ����
        _gameSceneUI.StartEncounter();                    // ��ī���� ui ����

        Helper.KeySet.GetEncounterTouch = true;           // ��ī��Ʈ�� ��ġ Ȱ��ȭ
        GetButtonActione = true;                          // ��ư �ִϸ��̼� �ʱ� ��
    }

    private void EncounterWave()
    {
        _encounterDual.SetEncounterWave();            // ���� ���̺��� �� �� ���� ����
        _encounterStage.SetEncounterEnemyPos();       // ���� ���̺꿡 �´� �� ����
    }

    bool CanEndDual()
    {
        var state = _encounterDual.GetBattleState;
        return state != BattleState.Win && state != BattleState.Lose;
    }

    private void EncounterEnd()
    {
        _encounterDual.EndDual();
        _targetSelector.EndDual();
    }

}
