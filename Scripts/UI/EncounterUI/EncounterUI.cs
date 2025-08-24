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
    [Header("�÷��̾� ����")]
    [SerializeField] private GameObject[] _playerInfo;

    [Header("���� ��ų")]
    [SerializeField] private GameObject _skillTrans;
    [SerializeField] private Sprite[] _skillPointSprite;
    [HideInInspector] private Image[] _skillPointImages;

    [Header("UI ������Ʈ")]
    [SerializeField] private RectTransform _uiCanvas;
    [SerializeField] private Transform _cursorTransform;
    private GameObject[] _selectCursors;

    [Header("���� / ��ų ��ư")]
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
            // ���� ��ų ui
            _skillPointImages = new Image[5];

            // Ŀ�� ui
            _selectCursors = new GameObject[5];

            // �� �Ŵ���
            _turnManagerUI.InitTMUI(ct);
        }

        void GetComponentArray()
        {
            // ���� ��ų
            for (int i = 0; i < _skillPointImages.Length; i++)
            {
                _skillPointImages[i] = _skillTrans.transform.GetChild(i).GetComponent<Image>();
            }

            // Ŀ�� ui
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
        InitPlayerUI();                 // �÷��̾� ui �ۿ�
        InitSkillPoints();              // ��ų ����Ʈ ����
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
            if (sp == 0) { _skillPointImages[i].sprite = _skillPointSprite[0]; continue; }   // 0 => ���� ����ִ°�

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
        ActiveCursor(false);                                                     // Ŀ�� ��Ȱ��ȭ

        List<GameObject> targetList = _encounter._encounterDual.GetTargetList;

        Debug.Log($"Ÿ�� ����Ʈ ����: {targetList.Count}");

        for (int i = 0; i < targetList.Count;i++)
        {
            GameObject unit = targetList[i];
            Collider col = unit.GetComponent<Collider>();

            Vector3 boundPos = col.bounds.center;

            // 1. ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
            Vector3 screenPos = Camera.main.WorldToScreenPoint(boundPos);

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _uiCanvas,
                screenPos,
                null,
                out localPoint
            );

            // 3. UI �̹����� ��ġ ����
            RectTransform imageRect = _selectCursors[i].GetComponent<RectTransform>();
            imageRect.localPosition = localPoint;

            // Ŀ�� Ȱ��ȭ
            _selectCursors[i].SetActive(true);
        }
    }


    public void OnFocusButton()
    {
        // _actioneButtons 0 => �Ϲ� ���� /// 1 = > ��ų
        
        Debug.Log(_encounter.GetButtonActione);

        if (_encounter.GetButtonActione) return;                                         // ��ư �׼��� Ȱ��ȭ �Ǿ� ������ �ߴ�

        if (!_actionButtons[0].activeSelf) ActiveButton(true);                           // �׼� ��ư Ȱ��ȭ

        SkillType type = _encounter._encounterDual._buttonSelectType;
        int count = (int)type;                                                           // 0 => �Ϲ� ����, 1 => ��ų, 2 => �ñر�
        StartCoroutine(ButtonScaleChange(count));
    }

    public IEnumerator ButtonScaleChange(int index)
    {
        _encounter.GetButtonActione = true;                                                // ��ư �׼� Ȱ��ȭ

        // ���õ� ��ư ũ��
        Vector2 targetSize = new Vector2(125f, 125f);
        // ���� �ȵ� ��ư ũ��
        Vector2 normalSize = new Vector2(105f, 105f);

        // ��� ��ư ũ�� �ʱ�ȭ
        for (int i = 0; i < _actionButtons.Length; i++)
        {
            _actionButtons[i].transform.GetChild(0).GetComponent<Outline>().enabled = false;         // ���õ� ��ư�� �ƿ����� ��Ȱ��ȭ
            RectTransform rect = _actionButtons[i].GetComponent<RectTransform>();
            rect.sizeDelta = normalSize;                                                            // ��� ��ư ũ�� �ʱ�ȭ
        }

        RectTransform selectedRect = _actionButtons[index].GetComponent<RectTransform>();
        float speed = 15f;
        _actionButtons[index].transform.GetChild(0).GetComponent<Outline>().enabled = true;         // ���õ� ��ư�� �ƿ����� Ȱ��ȭ

        while (Vector2.Distance(selectedRect.sizeDelta, targetSize) > 0.1f)
        {
            selectedRect.sizeDelta = Vector2.Lerp(selectedRect.sizeDelta, targetSize, Time.deltaTime * speed);
            yield return null;
        }
        selectedRect.sizeDelta = targetSize; // ���� ũ�� ����

        _encounter.GetButtonActione = false;                                                        // ��ư �׼� ��Ȱ��ȭ
    }

    public void OnActiveRouTineUI(bool active)
    {
        for(int i = 3; i < this.transform.childCount; i++) this.transform.GetChild(i).gameObject.SetActive(active);
    }

    public void OnStackRoutineUI(Character unit)
    {
        IStack stack = unit as IStack;                                                 // �нú� ����

        if (unit is null) return;
        if (stack is null) return;

        int idx = Array.IndexOf(_context._dual._currentParty, unit.gameObject);        // �ڸ� ã��
        int curStack = stack.ReturnCurStack();                                         // ���� ����

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
