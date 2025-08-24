using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingleManager<UIManager>
{
    /*
        기본 공통 UI 로직들에 사용
        옵션 ui << 등 항상 존재하는 친구들만 사용
    */


    [Header("버프 타입")]
    [SerializeField] public Sprite[] _typeSprite = new Sprite[2];

    [Header("페이드 전용")]
    [SerializeField] public FadeEffect _fadeEffect;

    private AllDamageUI _allDamageUi;
    public AllDamageUI GetAllDMGUI { get { return _allDamageUi; } set { _allDamageUi = value; } }



    public Color ReturnHexarColor(string hexcode)
    {
        // # 문자체크 있으면 지워버리기
        if (hexcode.StartsWith("#")) hexcode = hexcode.Substring(1);

        Color color;

        if (ColorUtility.TryParseHtmlString($"#{hexcode}", out color))
        { return color; }

        return Color.white;
    }
}