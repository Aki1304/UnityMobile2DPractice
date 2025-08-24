using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class EncounterUI : MonoBehaviour
{
    [Header("플레이어 인포")]
    [SerializeField] private GameObject[] _playerInfo;

    [Header("전투 스킬")]
    [SerializeField] private GameObject _skillTrans;
    [SerializeField] private Sprite[] _skillPointSprite;
    [HideInInspector] private Image[] _skillPointImages;

    [Header("UI 오브젝트")]
    [SerializeField] private RectTransform _uiCanvas;
    [SerializeField] private Transform _cursorTransform;
    private GameObject[] _selectCursors;

    [Header("공격 / 스킬 버튼")]
    [SerializeField] private GameObject[] _actionButtons;

    [Header("AllDamageUI")]
    public AllDamageUI _allDamageUI;
    public TurnManagerUI _turnManagerUI;

    private EncounterContext _context;

    private Encounter _encounter { get { return _context._encounter; } }

    public void InitEncounterUI(EncounterContext ct)
    {
        _context = ct;
        int count = _playerInfo.Length;

        InitArray();
        GetComponentArray();

        void InitArray()
        {
            // 전투 스킬 ui
            _skillPointImages = new Image[5];

            // 커서 ui
            _selectCursors = new GameObject[5];

            // 턴 매니저
            _turnManagerUI.InitTMUI(ct);
        }

        void GetComponentArray()
        {
            // 전투 스킬
            for (int i = 0; i < _skillPointImages.Length; i++)
            {
                _skillPointImages[i] = _skillTrans.transform.GetChild(i).GetComponent<Image>();
            }

            // 커서 ui
            for(int i = 0; i < _selectCursors.Length; i++)
            {
                _selectCursors[i] = _cursorTransform.GetChild(i).gameObject;
            }

            // ActiveFalse
            ActiveCursor(false);
        }

    }

    public void EncounterStageUI()
    {
        InitPlayerUI();                 // 플레이어 ui 작용
        InitSkillPoints();              // 스킬 포인트 적용
    }

    private void InitPlayerUI()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject unit = _encounter.GetPartyInfo[i];
            if (unit is null) continue;

            Character player = unit.GetComponent<Character>();
            if (player.GetStats.IsDie()) continue;

            EncounterPlayerUI playerUI = _playerInfo[i].GetComponent<EncounterPlayerUI>();
            UpdatePlayerVisuals(player, playerUI);
            UpdateStackUI(player, playerUI);
            InitBuffsAndEvents(player, playerUI, unit);
        }
    }

    private void UpdatePlayerVisuals(Character player, EncounterPlayerUI ui)
    {
        ui._playerSprite.sprite = player.ReturnPlayerSprite(0);
        ui._ultIconSprite.sprite = player.ReturnPlayerSprite(1);
        ui._ultGaugeSprite.fillAmount = player.GetStats.Amount(1);
        ui._hpSprite.fillAmount = player.GetStats.Amount(0);
        ui._hpText.text = player.GetStats.GetBaseStats.currentHP.ToString();
    }

    private void UpdateStackUI(Character player, EncounterPlayerUI ui)
    {
        if (player is IStack stack)
        {
            for (int i = 0; i < 3; i++)
            {
                if (stack.ReturnMaxStack() > i)
                {
                    ui._stackObject[i].SetActive(true);
                }
                else ui._stackObject[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
                ui._stackObject[i].SetActive(false);
        }
    }

    private void InitBuffsAndEvents(Character player, EncounterPlayerUI ui, GameObject unit)
    {
        ui.InitBuffList(player);
        player.OnTakeDamage += () => OnUpdateHp(unit);
        player.OnUltChange += () => OnUpdateUlt(unit);
        player.OnMyTurn += () => OnStackRoutineUI(player);

        OnStackRoutineUI(player);
    }

    private void InitSkillPoints()
    {
        Helper.DualManager.OnChangeSP += OnUpdateSP;
        Helper.DualManager.InitSkill(2);
        OnUpdateSP();
    }

    public void EncounterEndUI()
    {
      ActiveCursor(false);
    }

    private void OnUpdateUlt(GameObject unit)
    {
        int index = Array.IndexOf(_encounter.GetPartyInfo, unit);
        if (index == -1) return;

        Character character = unit.GetComponent<Character>();
        EncounterPlayerUI playerUI = _playerInfo[index].GetComponent<EncounterPlayerUI>();
        playerUI._ultGaugeSprite.fillAmount = character.GetStats.Amount(1);
    }

    private void OnUpdateHp(GameObject unit)
    {
        int index = Array.IndexOf(_encounter.GetPartyInfo, unit);
        if (index == -1) return;

        Character character = unit.GetComponent<Character>();
        EncounterPlayerUI playerUI = _playerInfo[index].GetComponent<EncounterPlayerUI>();

        playerUI._hpSprite.fillAmount = character.GetStats.Amount(0);
        playerUI._hpText.text = character.GetStats.GetCurrentHp.ToString();
    }

    private void OnUpdateSP()
    {
        int sp = Helper.DualManager.GetSP;

        for(int i = 0; i < _skillPointImages.Length; i++)        
        {
            if (sp == 0) { _skillPointImages[i].sprite = _skillPointSprite[0]; continue; }   // 0 => 전포 비어있는것

            if (sp - 1 >= i) _skillPointImages[i].sprite = _skillPointSprite[1];
            else _skillPointImages[i].sprite = _skillPointSprite[0];
        }
    }

    public void ActiveCursor(bool active)
    { 
        foreach (GameObject cursor in _selectCursors) cursor.SetActive(active);
    }

    public void ActiveAllButton(bool active)
    {
        for(int i = 0; i < _actionButtons.Length; i++)
        {
            _actionButtons[i].SetActive(active);
        }
    }

    public void ActiveButton(bool active)
    {
        for(int i = 0; i < _actionButtons.Length - 1; i++) _actionButtons[i].SetActive(active);
    }

    public void ActiveButton(bool active, int divide)
    {
        _actionButtons[2].SetActive(active);
    }


    public void OnCursorMove()
    {
        ActiveCursor(false);                                                     // 커서 비활성화

        List<GameObject> targetList = _encounter._encounterDual.GetTargetList;

        Debug.Log($"타겟 리스트 개수: {targetList.Count}");

        for (int i = 0; i < targetList.Count;i++)
        {
            GameObject unit = targetList[i];
            Collider col = unit.GetComponent<Collider>();

            Vector3 boundPos = col.bounds.center;

            // 1. 월드 좌표를 스크린 좌표로 변환
            Vector3 screenPos = Camera.main.WorldToScreenPoint(boundPos);

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _uiCanvas,
                screenPos,
                null,
                out localPoint
            );

            // 3. UI 이미지의 위치 설정
            RectTransform imageRect = _selectCursors[i].GetComponent<RectTransform>();
            imageRect.localPosition = localPoint;

            // 커서 활성화
            _selectCursors[i].SetActive(true);
        }
    }


    public void OnFocusButton()
    {
        // _actioneButtons 0 => 일반 공격 /// 1 = > 스킬
        
        Debug.Log(_encounter.GetButtonActione);

        if (_encounter.GetButtonActione) return;                                         // 버튼 액션이 활성화 되어 있으면 중단

        if (!_actionButtons[0].activeSelf) ActiveButton(true);                           // 액션 버튼 활성화

        SkillType type = _encounter._encounterDual._buttonSelectType;
        int count = (int)type;                                                           // 0 => 일반 공격, 1 => 스킬, 2 => 궁극기
        StartCoroutine(ButtonScaleChange(count));
    }

    public IEnumerator ButtonScaleChange(int index)
    {
        _encounter.GetButtonActione = true;                                                // 버튼 액션 활성화

        // 선택된 버튼 크기
        Vector2 targetSize = new Vector2(125f, 125f);
        // 선택 안된 버튼 크기
        Vector2 normalSize = new Vector2(105f, 105f);

        // 모든 버튼 크기 초기화
        for (int i = 0; i < _actionButtons.Length; i++)
        {
            _actionButtons[i].transform.GetChild(0).GetComponent<Outline>().enabled = false;         // 선택된 버튼에 아웃라인 비활성화
            RectTransform rect = _actionButtons[i].GetComponent<RectTransform>();
            rect.sizeDelta = normalSize;                                                            // 모든 버튼 크기 초기화
        }

        RectTransform selectedRect = _actionButtons[index].GetComponent<RectTransform>();
        float speed = 15f;
        _actionButtons[index].transform.GetChild(0).GetComponent<Outline>().enabled = true;         // 선택된 버튼에 아웃라인 활성화

        while (Vector2.Distance(selectedRect.sizeDelta, targetSize) > 0.1f)
        {
            selectedRect.sizeDelta = Vector2.Lerp(selectedRect.sizeDelta, targetSize, Time.deltaTime * speed);
            yield return null;
        }
        selectedRect.sizeDelta = targetSize; // 최종 크기 고정

        _encounter.GetButtonActione = false;                                                        // 버튼 액션 비활성화
    }

    public void OnActiveRouTineUI(bool active)
    {
        for(int i = 3; i < this.transform.childCount; i++) this.transform.GetChild(i).gameObject.SetActive(active);
    }

    public void OnStackRoutineUI(Character unit)
    {
        IStack stack = unit as IStack;                                                 // 패시브 스택

        if (unit is null) return;
        if (stack is null) return;

        int idx = Array.IndexOf(_context._dual._currentParty, unit.gameObject);        // 자리 찾기
        int curStack = stack.ReturnCurStack();                                         // 현재 스택

        EncounterPlayerUI playerUI = _playerInfo[idx].GetComponent<EncounterPlayerUI>();

        for (int i = 0; i < stack.ReturnMaxStack(); i++)
        {
            Image image = playerUI._stackIcon[i];
            if (curStack > i)
            {
                image.color = Helper.UM.ReturnHexarColor("C7FF00");
            }
            else
            {
                image.color = new Color(0,0,0,0);
            }
        }
    }
}
