using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffInfo
{
    string InfoID { get; }          //   고유 식별자
    string InfoViewId { get; }      //   한국어 표기
    Sprite buffSprite { get; }      //   고유 스프라이트
    BuffDurationType durationType { get; }  // 고유 타입 설정
    SkillActionType actionType { get; }

    /// <summary>
    /// 적용 할 버프 스탯 목록 및 동작은 여기다가 적용
    /// </summary>
    /// <returns></returns>
    CharStats.buffStats GetBuffStats();

    Sprite ReturnSprite();              // 스프라이트 돌려주기

    void BuffRoutine();
}
