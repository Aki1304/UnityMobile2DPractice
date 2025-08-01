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

    void InitGM()                                                               // GM �ʱ�ȭ
    {
        GetPartyManager.InitParty();                                           // ��Ƽ�Ŵ��� ����
        GetCharManager.InitCharManager();                                      // ĳ���͸Ŵ��� ����
    }

    void StartGM()
    {

    }

    void Awake()
    {
        InitGM();
        Application.targetFrameRate = 30;                                       // FPS ����
    }

    void Start()
    {
        
    }
}
