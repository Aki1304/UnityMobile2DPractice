using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class TargetSelector : MonoBehaviour
{
    private EncounterContext _context;

    private Character GetCurUnit { get { return _context._dual.GetCurUnit; } } 
    private List<GameObject> targetList { get { return _context._dual.GetTargetList; } }
    private GameObject[] GetPartyInfo { get { return _context._dual._currentParty; } } 
    private GameObject[] GetEnemyInfo { get {  return _context._dual._currentWaveEnemy; } }

    public void InitTargetSelector(EncounterContext ct)
    {
        _context = ct;

        // �̺�Ʈ ����
        _context._sceneUI.GetInputSelect.OnChangeCursor += OnSelectEvent;        // Ŀ�� �̵� �̺�Ʈ ����
    }

    public void EndDual()
    {

        // �̺�Ʈ ���� ����
        _context._sceneUI.GetInputSelect.OnChangeCursor -= OnSelectEvent;  // Ŀ�� �̵� �̺�Ʈ ���� ����
    }

    public void OnSelectEvent()
    {
        // ��ų Ÿ���� �Ʊ����� ������ ����
        SkillMetaData data = GetCurUnit.ReturnAct(_context._dual._buttonSelectType);
        SkillActionType type = data.actionType;


        if(type == SkillActionType.Attack)         // ��ų Ÿ���� �����̶��
        {
            _context._sceneUI.GetInputSelect.CanSelectPlayer = false;    // �÷��̾� ���� �Ұ�
            _context._encounter._camMove.CurrentCameraTurn(GetCurUnit);  // ī�޶� ����
        }
        else                                        // ��ų Ÿ���� ������ �ƴ϶��
        {
            _context._sceneUI.GetInputSelect.CanSelectPlayer = true;    // �÷��̾� ���� ����
            _context._encounter._camMove.OnHealAndBuffCamera();  // �� �� ���� ī�޶� ����
        }

        SetTargetList(type);                                                                        // Ÿ�� ����Ʈ ����
        OnCursorChange();
    }

    public void SelectButton(int chooseType)
    {
        Character target = _context._encounter.GetSelectUnit();
        Character unit = GetCurUnit;

        SkillType selectType = (SkillType)chooseType;
        SkillMetaData data = unit.ReturnAct(selectType);

        if (_context._encounter.GetButtonActione) return;                          // ��ư �׼��� Ȱ��ȭ �Ǿ� ������ �ߴ�

        if (_context._dual._buttonSelectType != selectType)
        {
            _context._dual._buttonSelectType = selectType;                          // ��ư Ÿ�� ����
            OnSelectEvent();                                                        // Ÿ�� ����Ʈ ����
            _context._encounterUI.OnFocusButton();                                  // ��ư ��Ŀ�� Ȱ��ȭ
            return;                                                                 // ��ư Ÿ���� �ٸ��� ����
        }

        if (_context._dual._buttonSelectType == selectType)
        {
            Helper.GM.GetTouchKeySet.GetEncounterTouch = false;
            _context._encounterUI.ActiveCursor(false);                              // Ŀ�� ��Ȱ��ȭ
            _context._encounterUI.ActiveAllButton(false);                           // ��ư ��Ȱ��ȭ
            _context._encounterUI._allDamageUI.ResetDamage();                       // ������ ����
            ActButton(data);                                                            // ��ų ���� ���� �� Ÿ�� ����
        }

    }

    public void UltimateButton(int idx)
    {
        GameObject objects = _context._dual._currentParty[idx];
        Character unit = objects.GetComponent<Character>();
        CharStats stat = unit.GetStats;

        // �ñر� ������ üũ
        if (stat.GetCurrentUlt != stat.GetBaseStats.baseMaxUlt) { Debug.Log(  $" {unit.name}�ñر� ������ �� ����"); return; }

        // �ñر� ��ư �� �Ŵ������� �����ֱ�� �ش޶��ϱ�.
        _context._turnManager.OnPushExtraTurn(stat, SkillType.ult);
    }

    public void ExtraTurn()
    {
        _context._encounterUI._allDamageUI.ResetDamage();                       // ������ ����

        GetCurUnit.OnTurnEnd += EndTurn;
        IPassiveExtraAttack extra = GetCurUnit as IPassiveExtraAttack;
        if (extra == null) 
        {
            GetCurUnit.OnTurnEnd -= EndTurn;
            return;
        }

        extra.ActAttack(_context._dual._currentWaveEnemy);
    }

    public void ActButton(SkillMetaData data)
    {
        GetCurUnit.OnTurnEnd += EndTurn;

        // ���� Ÿ������ üũ �� ����
        if (data.actionType == SkillActionType.Attack)
        {
            Helper.DualManager.BrokerPassiveType(targetList);
        }

        // ���� ���� ����
        if (_context._dual._buttonSelectType == SkillType.normal) GetCurUnit.NormalAttack(targetList);
        if (_context._dual._buttonSelectType == SkillType.skill) GetCurUnit.Skill(targetList);
        if (_context._dual._buttonSelectType == SkillType.ult) GetCurUnit.Ultimate(targetList);
    }

    // ���� ���� �����ٸ�
    public void EndTurn()
    {
        _context._dual.GetCurEndTurn = true;
        GetCurUnit.OnTurnEnd -= EndTurn;
    }

    public void SetTargetList(SkillActionType type)
    {
        targetList.Clear();                                                                     // Ÿ�� ����Ʈ �ʱ�ȭ

        Character target = _context._encounter.GetSelectUnit();
        Character unit = GetCurUnit;

        SkillMetaData act = unit.ReturnAct(_context._dual._buttonSelectType);
        int centerIndex = (type == SkillActionType.Attack) 
            ? Array.IndexOf(GetEnemyInfo, target.gameObject) : Array.IndexOf(GetPartyInfo, target.gameObject);

        switch (act.range)
        {
            case 1:
                { 
                    if(type == SkillActionType.Attack) targetList.Add(GetEnemyInfo[centerIndex]);
                    else targetList.Add(GetPartyInfo[centerIndex]);

                    break;
                }

            case 3:
                {
                    targetList.Add(GetEnemyInfo[centerIndex]);                                     // ���� ������Ʈ �߰�
                    int leftindex = centerIndex - 1;                                               // ���� �ε���
                    int rightindex = centerIndex + 1;                                              // ������ �ε���

                    if (leftindex >= 0 && GetEnemyInfo[leftindex] != null)                         // �ε��� �迭 ���� ���� üũ
                    {
                        targetList.Add(GetEnemyInfo[leftindex]);
                    }

                    if (rightindex < GetEnemyInfo.Length && GetEnemyInfo[rightindex] != null)
                    {
                        targetList.Add(GetEnemyInfo[rightindex]);
                    }

                    break;
                }

            case 4:     // 4���� ���� ���� �÷��̾� ����
                { foreach (var party in GetPartyInfo) if (party != null) targetList.Add(party); break; }

            case 5:
                { foreach (var enemy in GetEnemyInfo) if (enemy != null) targetList.Add(enemy); break; }

            default : throw new ArgumentOutOfRangeException(nameof(act.range), "���� ������ ��ų ������ ������ϴ�.");
        }
    }

    public void OnCursorChange()
    {
        _context._encounterUI.OnCursorMove();        // Ŀ�� ����
    }
}
