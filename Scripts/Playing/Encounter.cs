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
    // context 위치
    public EncounterDual _encounterDual;
    public EncounterStage _encounterStage;
    public TurnManager _turnManager;
    public TargetSelector _targetSelector;
    public GameSceneUI _gameSceneUI;

    private EncounterContext _context;

    // context 아님
    public CameraMove _camMove;

    public GameObject[] GetPartyInfo { get { return _encounterDual._currentParty; } }
    public Character GetSelectUnit() => _gameSceneUI.GetSelectUnit;


    #region 버튼 UI 애니메이션 정리 용도
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

        // 이벤트 구독을 위한 연결
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
        Debug.Log("전투 진입");
        EncounterInit();
        yield return null;

        while (CanEndDual())                             // 끝날 때 까지
        {
            EncounterWave();
            yield return StartCoroutine(_turnManager.RoutineActTurn());          // 이제 이 웨이브에서 턴 전부를 이룰 거임

            Debug.Log(_encounterDual.GetBattleState);
        }

        EncounterEnd();
    }

    private void EncounterInit()
    {
        _encounterDual.ResetEncounterDual();              // 시작 시 전투 리셋
        _encounterDual.StartEncounterDual();              // 인카운터시 웨이브 리스트 미리 넣어두기. 
        _encounterStage.SetEncounterPlayerPos();          // 플레이어 생성 후 배틀 돌입
        _gameSceneUI.StartEncounter();                    // 인카운터 ui 시작

        Helper.KeySet.GetEncounterTouch = true;           // 인카우트용 터치 활성화
        GetButtonActione = true;                          // 버튼 애니메이션 초기 락
    }

    private void EncounterWave()
    {
        _encounterDual.SetEncounterWave();            // 현재 웨이브의 적 수 라운드 세팅
        _encounterStage.SetEncounterEnemyPos();       // 현재 웨이브에 맞는 적 생성
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
