using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    // 전역으로 자주 호출하는 내용 정리 해두기.
    public static GameManager GM => GameManager.Instance;
    public static PartyManager PartyManager => GM.GetPartyManager;
    public static CharacterManager CharacterManager => GM.GetCharManager;
    public static TouchKeySet KeySet => GM.GetTouchKeySet;
    public static DualManager DualManager => GM.GetDualManager;
    public static PoolManager PoolManager => GM.GetPoolManager;
    public static UIManager UM => UIManager.Instance;

    //  위 내용은 각 매니저에 접근하기 위한 편의성 제공용.
}
