using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;


public class TurnManagerUI : MonoBehaviour
{
    private EncounterContext _context;

    [Header("프리펩")]
    [SerializeField] private GameObject _prefabTurn;
    [SerializeField] private GameObject _prefabExtra;

    [Header("턴 UI 위치")]
    [SerializeField] private Transform _turnTransform;

    [Header("라운드 텍스트")]
    [SerializeField] private TextMeshProUGUI _roundTurnText;

    [Header("웨이브 텍스트")]
    [SerializeField] private TextMeshProUGUI _waveText;

    [Header("추가 턴 위치")]
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
        // 행동 턴 관리 20개 추가
        for(int i = 0; i < 20; i++)
        {
            GameObject uiObject = Instantiate(_prefabTurn,_turnTransform);
            uiObject.SetActive(false);

            ActUI tmp = new ActUI();
            tmp.actObject = uiObject;

            _actList.Add(tmp);
        }

        // 엑스트라 턴 8개 추가
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
        int prefabCount = _context._turnManager.allCharStats.Count;         // 생성할 프리펩 수

        _waveText.text = $"[ {_context._dual._currentWaveCount + 1} ]";     // 0 시작이라 + 1
        _roundTurnText.text = $"[ {_context._dual._currentTurnRound + 1} ]";

        // 사용하는 애들 넣고
        for(int i = 0; i < prefabCount; i++)
        {
            Character unit = _context._turnManager.allCharStats[i].GetChar;
            ActUI tmp = _actList[i];

            tmp.actUnit = unit;

            tmp.actUnit.OnDie -= RemoveUnitUI;  // 기존 구독 제거 (중복 방지)
            tmp.actUnit.OnDie += RemoveUnitUI;  // 다시 구독

            AddTurnUI(tmp.actUnit, tmp.actObject);
        }

        // SetActive 구분
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

        // 사용하는 애들 넣고
        for (int i = 0; i < prefabCount; i++)
        {
            Character unit = tm._extraTurn[i].charStat.GetChar;
            ActUI tmp = _extraList[i];

            tmp.actUnit = unit;

            AddExtraTurnUI(tmp.actUnit, tmp.actObject);
        }

        // SetActive 구분
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
        foreach (var tmp in _actList)                   // 같은 stats 인 경우 다 false
        {
            if (tmp.actUnit == null) continue;          // null접근 오류 금지

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
