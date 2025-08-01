using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSceneUI : SceneLogicUI
{
    [Header("인카운터 UI")]
    [SerializeField] public EncounterUI _encounterUI;

    [Header("Select UI")]
    [SerializeField] private InputSelect _inputSelect;
    public InputSelect GetInputSelect { get { return _inputSelect; } }
    public Character GetSelectUnit { get { return _inputSelect._selectChar; } set { _inputSelect._selectChar = value; } }

    private EncounterContext _context;

    public void InitSceneUI(EncounterContext ct)
    {
        _context = ct;

        _encounterUI.InitEncounterUI(ct);
        ActiveEncounter(false);
    }


    public void ActiveEncounter(bool active)
    {
        _encounterUI.gameObject.SetActive(active);
    }

    public void StartEncounter()
    {
        ActiveEncounter(true);
        _encounterUI.EncounterStageUI();
    }

    public void ResetUI()
    {
        // 캐릭터 초기화
        _inputSelect.NullSelectChar();
        
        // 커서 초기화
        _encounterUI.ActiveCursor(false);
    }
}
