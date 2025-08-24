using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IBuffInfo
{
    Character castUnit { get; set; }// 버프를 적용한 캐릭터
    string InfoID { get; }          //   고유 식별자
    string InfoViewId { get; }      //   한국어 표기
    Sprite buffSprite { get; }      //   고유 스프라이트
    BuffDurationType durationType { get; }  // 고유 타입 설정
    SkillActionType actionType { get; }

    void OnBuffAdd(); // 버프 적용 시 작동  

    /// <summary>
    /// 적용 할 버프 스탯 목록 및 동작은 여기다가 적용
    /// </summary>
    /// <returns></returns>
    CharStats.buffStats GetBuffStats();

    Sprite ReturnSprite();              // 스프라이트 돌려주기
}

public interface WhileAliveDuration 
{
    string GetBuffID { get; }          //   고유 식별자

    // 소유자 턴 시작 시 적용 없음
    public void OnWhileAliveAdd(Character owner) 
    {
        if (owner.gameObject is null) return;
        owner.OnDie += EndBuff;
    }

    public void EndBuff(CharStats stats)
    {
        GameObject[] party = Helper.DualManager._instanceParty;

        for (int i = 0; i < party.Length; i++)
        {
            if (party[i] is null) continue;

            Character unit = party[i].GetComponent<Character>();
            unit._charBuffManager.RemoveBuff(GetBuffID);

            unit.OnDie -= EndBuff; // 버프 제거 시 이벤트 해제
        }
    }
}

public interface TurnBasedDuration
{
    string GetBuffID { get; }           //   고유 식별자
    int CurrentTurn { get; set; }    // 현재 턴 수 
    int MaxTurn { get; set; }        // 최대 턴 수


    /// <summary>
    /// 소유자의 턴이 끝날 떄 작동을 하지만
    /// 해당 버프는 추가 공격, 궁극기 등에는 차감되지 않음.
    /// 단, 기본 턴을 추가로 주는 경우 차감. EX) 프리스트 추가 행동 턴
    /// </summary>
    
    public void OnTurnBaseRoutine(Character unit)
    {
        // unit은 나 자신을 지칭
        CurrentTurn -= 1;                               // 턴 수 증가

        if(CurrentTurn <= 0)                      //  현재 턴 수가 0 이하가 되면
            unit._charBuffManager.RemoveBuff(GetBuffID);   // 버프 제거
    }
}

public interface OwnerTurnBasedDuration
{
    string GetBuffID { get; }           //   고유 식별자
    int CurrentTurn { get; set; }    // 현재 턴 수
    int MaxTurn { get; }        // 최대 턴 수

    public void OnOwnerTurnStart()
    {
        CurrentTurn = MaxTurn;
    }

    public void OnOwnerTurnRoutine(Character owner)
    {
        if (CurrentTurn == 0) return; // 현재 턴 수가 0이라면 검사 할 필요 x

        CurrentTurn -= 1;                               // 턴 수 증가
        if(CurrentTurn <= 0) //  현재 턴 수가 0 이하가 되면
        {
            foreach(var instance in Helper.DualManager._instanceParty)
            {
                if (instance is null) continue;

                Character unit = instance.GetComponent<Character>();
                unit._charBuffManager.RemoveBuff(GetBuffID);   // 버프 제거
            }    
        }
    }
}