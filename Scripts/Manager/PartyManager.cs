using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class PartyManager : MonoBehaviour
{
    private GameObject[] _charParty;                                    // ������Ʈ�� ���� ����
    public GameObject[] GetPartyInfo {  get { return _charParty; } }

    public CharStats[] GetPartyStatsInfo { get; set; }

    public void ResetParty()
    {
        for(int i = 0; i < 4; i++) _charParty[i] = null;
    }

    public void InitParty()
    {
        _charParty = new GameObject[4];
        GetPartyStatsInfo = new CharStats[4];
    }

    public void ReadyParty()
    {

    }

    public void Update()
    {
        // �׽�Ʈ
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            UpdateParty();
        }
    }

    public void UpdateParty()           // ��Ƽ �迭 ������Ʈ
    {
        for (int i = 0; i < 4; i++)
        {
            _charParty[i] = Helper.CharacterManager.GetCharPlayer[i];
            Player player = _charParty[i].GetComponent<Player>();
            GetPartyStatsInfo[i] = player.GetStats;
            Debug.Log($"{ GetPartyStatsInfo[i]}");
        }
    }


    public void EndUpdateParty(GameObject[] tmp)        // ��Ʋ ������ ������Ʈ �ϱ�
    {
        // �迭 ����ȭ

        /* ����ȭ �ؾ��� ��
           1. ���� ü�� 2. �� ������ 
        */

        for(int i = 0; i < 4; i++)
        {
            if (tmp[i] == null) continue;

            Character player = tmp[i].GetComponent<Character>();
            CharStats stats = player.GetStats;

            Character origin = GetPartyInfo[i].GetComponent<Character>();
            CharStats originStats = origin.GetStats;

            // ü��
            originStats.GetCurrentHp = stats.GetCurrentHp;
            // �� ������
            originStats.GetCurrentUlt = stats.GetCurrentUlt;
        }

        // ���� ����ȭ
        UpdateParty();
    }
}
