using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    // �������� ���� ȣ���ϴ� ���� ���� �صα�.
    public static GameManager GM => GameManager.Instance;
    public static PartyManager PartyManager => GM.GetPartyManager;
    public static CharacterManager CharacterManager => GM.GetCharManager;
    public static TouchKeySet KeySet => GM.GetTouchKeySet;
    public static DualManager DualManager => GM.GetDualManager;
    public static PoolManager PoolManager => GM.GetPoolManager;
    public static UIManager UM => UIManager.Instance;

    //  �� ������ �� �Ŵ����� �����ϱ� ���� ���Ǽ� ������.
}
