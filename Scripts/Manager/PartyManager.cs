using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class PartyManager : MonoBehaviour
{
    private GameObject[] _charParty;                                    // 오브젝트로 먼저 관리
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


    public void UpdateParty(GameObject[] partyObjects)           // 파티 배열 업데이트
    {
        for (int i = 0; i < partyObjects.Length; i++)
        {
            GetPartyInfo[i] = partyObjects[i];                   // 파티 오브젝트 배열에 넣기
            Debug.Log($"파티 업데이트: {partyObjects[i]}");
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
    }
}
