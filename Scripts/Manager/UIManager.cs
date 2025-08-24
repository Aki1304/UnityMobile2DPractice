using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingleManager<UIManager>
{
    /*
        �⺻ ���� UI �����鿡 ���
        �ɼ� ui << �� �׻� �����ϴ� ģ���鸸 ���
    */


    [Header("���� Ÿ��")]
    [SerializeField] public Sprite[] _typeSprite = new Sprite[2];

    [Header("���̵� ����")]
    [SerializeField] public FadeEffect _fadeEffect;

    private AllDamageUI _allDamageUi;
    public AllDamageUI GetAllDMGUI { get { return _allDamageUi; } set { _allDamageUi = value; } }



    public Color ReturnHexarColor(string hexcode)
    {
        // # ����üũ ������ ����������
        if (hexcode.StartsWith("#")) hexcode = hexcode.Substring(1);

        Color color;

        if (ColorUtility.TryParseHtmlString($"#{hexcode}", out color))
        { return color; }

        return Color.white;
    }
}