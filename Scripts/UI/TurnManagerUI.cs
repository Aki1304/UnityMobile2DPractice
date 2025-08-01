using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;


public class TurnManagerUI : MonoBehaviour
{
    private EncounterContext _context;

    [Header("������")]
    [SerializeField] private GameObject _prefabTurn;
    [SerializeField] private GameObject _prefabExtra;

    [Header("�� UI ��ġ")]
    [SerializeField] private Transform _turnTransform;

    [Header("���� �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI _roundTurnText;

    [Header("���̺� �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI _waveText;

    [Header("�߰� �� ��ġ")]
    [SerializeField] private Transform _extraTurnTransform;

    private struct ActUI
    {
        public Character actUnit;
        public GameObject actObject;
    }

    private List<ActUI> _actList = new List<ActUI>();
    private List<ActUI> _extraList = new List<ActUI>();

    public void InitTMUI(EncounterContext ct)
    {
        _context = ct;

        AddList();
        SubsEvent();
    }

    void AddList()
    {
        // �ൿ �� ���� 20�� �߰�
        for(int i = 0; i < 20; i++)
        {
            GameObject uiObject = Instantiate(_prefabTurn,_turnTransform);
            uiObject.SetActive(false);

            ActUI tmp = new ActUI();
            tmp.actObject = uiObject;

            _actList.Add(tmp);
        }

        // ����Ʈ�� �� 8�� �߰�
        for(int i = 0; i < 8; i++)
        {
            GameObject uiObject = Instantiate(_prefabExtra, _extraTurnTransform);
            uiObject.SetActive(false);

            ActUI tmp = new ActUI();
            tmp.actObject = uiObject;

            _extraList.Add(tmp);
        }
    }

    void SubsEvent()
    {
        TurnManager tm = _context._turnManager;
        tm.OnAddTurn += SortTurn;
        tm.OnExtraTurn += SortExtraTurn;
    }

    public void SortTurn()
    {
        int prefabCount = _context._turnManager.allCharStats.Count;         // ������ ������ ��

        _waveText.text = $"[ {_context._dual._currentWaveCount + 1} ]";     // 0 �����̶� + 1
        _roundTurnText.text = $"[ {_context._dual._currentTurnRound + 1} ]";

        // ����ϴ� �ֵ� �ְ�
        for(int i = 0; i < prefabCount; i++)
        {
            Character unit = _context._turnManager.allCharStats[i].GetChar;
            ActUI tmp = _actList[i];

            tmp.actUnit = unit;

            tmp.actUnit.OnDie -= RemoveUnitUI;  // ���� ���� ���� (�ߺ� ����)
            tmp.actUnit.OnDie += RemoveUnitUI;  // �ٽ� ����

            AddTurnUI(tmp.actUnit, tmp.actObject);
        }

        // SetActive ����
        for(int i = 0; i < _actList.Count; i++)
        {
            if(i >= prefabCount) _actList[i].actObject.SetActive(false);
            else _actList[i].actObject.SetActive(true);
        }
    }

    public void SortExtraTurn()
    {
        var tm = _context._turnManager;
        int prefabCount = tm._extraTurn.Count;

        // ����ϴ� �ֵ� �ְ�
        for (int i = 0; i < prefabCount; i++)
        {
            Character unit = tm._extraTurn[i].charStat.GetChar;
            ActUI tmp = _extraList[i];

            tmp.actUnit = unit;

            AddExtraTurnUI(tmp.actUnit, tmp.actObject);
        }

        // SetActive ����
        for (int i = 0; i < _extraList.Count; i++)
        {
            if (i >= prefabCount) _extraList[i].actObject.SetActive(false);
            else _extraList[i].actObject.SetActive(true);
        }
    }

    public void AddTurnUI(Character unit, GameObject uiObject)
    {
        ActionTurnIconRenderer render = uiObject.GetComponent<ActionTurnIconRenderer>();

        Color color = (unit.GetStats.GetUnitCode == 0) ? Color.green : Color.red;
        Sprite sprite = unit.ReturnPlayerSprite(0);

        render.SetRenderer(color, sprite);
    }

    public void AddExtraTurnUI(Character unit, GameObject uiObject)
    {
        ActionTurnIconRenderer render = uiObject.GetComponent<ActionTurnIconRenderer>();

        Sprite sprite = unit.ReturnPlayerSprite(0);

        render.ExtraSetRenderer(sprite);
    }

    public void RemoveUnitUI(CharStats stats)
    {
        foreach (var tmp in _actList)                   // ���� stats �� ��� �� false
        {
            if (tmp.actUnit == null) continue;          // null���� ���� ����

            if (stats == tmp.actUnit.GetStats)
            {
                tmp.actObject.SetActive(false);
            }
        }

        foreach(var tmp in _extraList)
        {
            if (tmp.actUnit == null) continue;

            if(stats == tmp.actUnit.GetStats)
            {
                tmp.actObject.SetActive(false);
            }
        }
    }

}
