using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleManager<GameManager>
{
    [SerializeField] private PartyManager _partyManager;
    public PartyManager GetPartyManager { get { return _partyManager; } }

    [SerializeField] private CharacterManager _charManager;
    public CharacterManager GetCharManager { get { return _charManager; } }

    [SerializeField] private TouchKeySet _touchKeySet;
    public TouchKeySet GetTouchKeySet { get { return _touchKeySet; } }

    [SerializeField] private DualManager _dualManager;
    public DualManager GetDualManager { get { return _dualManager; } }

    [SerializeField] private PoolManager _poolManager;
    public PoolManager GetPoolManager { get { return _poolManager; } }

    void InitGM()                                                               // GM 초기화
    {
        GetPartyManager.InitParty();                                           // 파티매니저 인잇
        GetCharManager.InitCharManager();                                      // 캐릭터매니저 인잇
    }

    void StartGM()
    {

    }

    void Awake()
    {
        InitGM();
        Application.targetFrameRate = 30;                                       // FPS 설정
    }

    void Start()
    {
        
    }
}
