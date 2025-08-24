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
        GetPoolManager.InitPoolManger();                                       // Ǯ�Ŵ��� ����
    }
    
    void StartGM()
    {

    }

    void Awake()
    {
        InitGM();
        OnSetFrameRate(30);                     // �ʱ� ������ ����Ʈ ����
    }

    void Start()
    {
        
    }

    public void OnSetFrameRate(int frame)
    {
        if (frame <= 30) Application.targetFrameRate = 30;
        else if (frame <= 60) Application.targetFrameRate = 60;
    }
}
