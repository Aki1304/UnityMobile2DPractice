using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class PartyManager : MonoBehaviour
{
    private GameObject[] _charParty;                                    // 오브젝트로 먼저 관리
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
        // 테스트
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            UpdateParty();
        }
    }

    public void UpdateParty()           // 파티 배열 업데이트
    {
        for (int i = 0; i < 4; i++)
        {
            _charParty[i] = Helper.CharacterManager.GetCharPlayer[i];
            Player player = _charParty[i].GetComponent<Player>();
            GetPartyStatsInfo[i] = player.GetStats;
            Debug.Log($"{ GetPartyStatsInfo[i]}");
        }
    }


    public void EndUpdateParty(GameObject[] tmp)        // 배틀 끝나고 업데이트 하기
    {
        // 배열 동기화

        /* 동기화 해야할 것
           1. 현재 체력 2. 궁 게이지 
        */

        for(int i = 0; i < 4; i++)
        {
            if (tmp[i] == null) continue;

            Character player = tmp[i].GetComponent<Character>();
            CharStats stats = player.GetStats;

            Character origin = GetPartyInfo[i].GetComponent<Character>();
            CharStats originStats = origin.GetStats;

            // 체력
            originStats.GetCurrentHp = stats.GetCurrentHp;
            // 궁 게이지
            originStats.GetCurrentUlt = stats.GetCurrentUlt;
        }

        // 스탯 동기화
        UpdateParty();
    }
}
