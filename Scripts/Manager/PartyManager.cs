using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class PartyManager : MonoBehaviour
{
    private GameObject[] _charParty;                                    // ������Ʈ�� ���� ����
    public GameObject[] GetPartyInfo {  get { return _charParty; } }

    public void ResetParty()
    {
        for(int i = 0; i < 4; i++) _charParty[i] = null;
    }

    public void InitParty()
    {
        _charParty = new GameObject[4];
    }

    public void ReadyParty()
    {

    }


    public void UpdateParty(GameObject[] partyObjects)           // ��Ƽ �迭 ������Ʈ
    {
        for (int i = 0; i < partyObjects.Length; i++)
        {
            GetPartyInfo[i] = partyObjects[i];                   // ��Ƽ ������Ʈ �迭�� �ֱ�
            Debug.Log($"��Ƽ ������Ʈ: {partyObjects[i]}");
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
    }
}
