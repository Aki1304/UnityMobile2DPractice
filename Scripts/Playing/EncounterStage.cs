using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EncounterStage : MonoBehaviour
{
    [Header("ĳ���� ��ġ")]
    [SerializeField] private Transform _playerPosition;

    [Header("�� ��ġ")]
    [SerializeField] private Transform _enemyPosition;

    [Header("�÷��̾� �ν��Ͻ� ��ġ")]
    [SerializeField] private Transform _playerInstanceTransform;

    [Header("�� �ν��Ͻ� ��ġ")]
    [SerializeField] private Transform _enemyInstanceTransform;

    [SerializeField] private GameObject _instanceEnemyUI;

    private Vector3[] _charPos = new Vector3[4];
    private Vector3[] _enemyPos = new Vector3[5];

    private EncounterContext _context;
    public EncounterDual _dual { get { return _context._dual; } }
    public void InitStage(EncounterContext ct)
    {
        _context = ct;
    }

    public void SetInitCharPos()
    {
        for (int i = 0; i < 4; i++)                 // �÷��̾� ��Ƽ ��ġ 
        {
            _charPos[i] = _playerPosition.GetChild(i).transform.position;
        }

        for (int i = 0; i < 5; i++)                 // �� ��Ƽ ��ġ 
        {
            _enemyPos[i] = _enemyPosition.GetChild(i).transform.position;
        }
    }

    public void SetEncounterPlayerPos()          // ��Ƽ ���� �迭���� ��������
    {
        Helper.DualManager.EnemySkillEffectPos = (_charPos[1] / 2f) + (_charPos[2] / 2f) + new Vector3(0, 1.2f, 0);                       // �÷��̾� 2���� 3�� ���� ��ġ

        Debug.Log($"�� ��ų ����Ʈ ��ġ: {Helper.DualManager.EnemySkillEffectPos}");

        for (int i = 0; i < 4; i++)
        {
            int minusScaleX = (i < 2) ? -1 : 1;          // 1�� 2�� ����    3�� 4�� ������

            GameObject charObject = Instantiate(Helper.PartyManager.GetPartyInfo[i], _playerInstanceTransform);
            Character unit = charObject.GetComponent<Character>();
            Transform charTrans = charObject.transform;

            charTrans.position = _charPos[i];
            charTrans.localScale =
                new Vector3(charTrans.localScale.x * minusScaleX, charTrans.localScale.y, charTrans.localScale.z);

            _context._dual._currentParty[i] = charObject;
            unit.InitCharacter();                        // ������ ���� ����
        }

        // ��� �Ŵ��� �� ��
        Helper.DualManager.GetInitDual(_context._dual._currentParty);
    }

    public void SetEncounterEnemyPos()          // ���� ������ ���� ���̺� �ܰ� Ȯ���ϰ� ��ȯ
    {
        int size = _dual._currentWaveEnemy.Count(obj => obj != null);

        // ���� �ʱ� ����
        for (int i = 0; i < size; i++)
        {
            GameObject charObject = Instantiate(_dual._currentWaveEnemy[i], _enemyInstanceTransform);      // �ν�źƼ����Ʈ�� ��ȯ
            Character unit = charObject.GetComponent<Character>();
            unit.InitCharacter();

            GameObject uiObject = Instantiate(_instanceEnemyUI, charObject.transform);
            EnemyUI ui = uiObject.GetComponent<EnemyUI>();
            ui.InitUI(unit);  // �� �ȿ��� �ٷ� OnTakeDamage += OnHpChange ȣ���

            BoxCollider col = charObject.GetComponent<BoxCollider>();
            Vector3 uiPos = new Vector3(col.bounds.center.x, col.bounds.max.y + 0.2f, col.bounds.center.z);

            uiObject.transform.position = uiPos;

            _dual._currentWaveEnemy[i] = charObject;

            unit.GetStats.UpdateFinalStats();
        }

        // ���� ��ġ ����
        ResetEnemyPos();
    }

    public void ResetEnemyPos()
    {
        int[] startCounts = new int[5] { 2, 1, 1, 0, 0 };                                               // ���� ��ġ ��ġ�� ���� �ε��� �迭 (���� ���� ���� ���� ��� ����)   
        int enemyCounts = _dual._currentWaveEnemy.Count(obj => obj != null);                   // ���� ����ִ�(=null�� �ƴ�) ���� �� ���  

        if (enemyCounts == 0) return;                                                                   // ���Ͱ� �ϳ��� ������ �Լ� ����   

        int startIndex = startCounts[enemyCounts - 1];                                                   // ���� �ε���: ���� ���� ���� ���� ��� ���� ���� ��ġ ����
        int size = startIndex + enemyCounts;                                                             // ������ �ε���: ���� ��ġ + ���� ��
        int count = 0;                                                                                   // ���� ��ġ ������

        // _currentWaveEnemy�� tempObject�� ����
        GameObject[] tempObject = new GameObject[5];
        Array.Copy(_dual._currentWaveEnemy, tempObject, 5);

        // ����ִ� ���͸� ���ʺ��� ä��� (���� ����)
        int aliveIdx = 0;
        for (int i = 0; i < tempObject.Length; i++)
        {
            if (tempObject[i] != null)
            {
                _dual._currentWaveEnemy[aliveIdx] = tempObject[i];
                aliveIdx++;
            }
        }

        // ������ ĭ�� null�� ä���
        for (int i = aliveIdx; i < tempObject.Length; i++)
        {
            _dual._currentWaveEnemy[i] = null;
        }

        for (int i = startIndex; i < size; i++)
        {
            GameObject enemy = _dual._currentWaveEnemy[count];                                  // ���� ���� ��������
            enemy.transform.position = _enemyPos[i];                                                     // ���� ��ġ ����
            count++;
        }
    }

}
