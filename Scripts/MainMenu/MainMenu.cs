using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// ���� ó�� ���� �޴� ���� ���μ���
    /// </summary>

    [Header("������Ʈ")]
    public MainMenuObjectScripts _mainObjects;   // ���� �޴� ������Ʈ ��ũ��Ʈ ����
    public MainMenuCharExScripts _mainCharEx;    // ���� �޴� ĳ���� ���� ��ũ��Ʈ ����

    [Header("ĳ���� ���� ����Ʈ")]
    public List<CharSlotsUI> _charSlotsUI; // ĳ���� ���� �̹��� ����Ʈ

    public GoogleMobileAdsScript mobileAds;
    public MainMenuCharList _mainCharList;       // ���� �޴� ĳ���� ����Ʈ ��ũ��Ʈ ����

    private void Awake()
    {
        InitMain();
    }

    void Start()
    {
        Process();
    }

    void InitMain()
    {
        mobileAds = new GoogleMobileAdsScript();    // ���� ����� ���� ��ũ��Ʈ �ʱ�ȭ
        _mainCharList = new MainMenuCharList();     // ���� �޴� ĳ���� ����Ʈ ��ũ��Ʈ �ʱ�ȭ

        mobileAds.GooldAdInit();                    // ���� ����� ���� �ʱ�ȭ
        _mainCharList.InitCharList();              // ���� �޴� ĳ���� ����Ʈ �ʱ�ȭ
    }

    void Process()
    {
        Helper.UM._fadeEffect.FadeOut();            // ���̵� �ƿ� ����

        OnUIReset();                  // UI �ʱ�ȭ
    }

    public void OnUIReset()
    {
        _mainCharList.UpdateUI(_mainObjects.rightCharList, _mainObjects.rightSelectCharList); // ĳ���� ����Ʈ UI ������Ʈ   
        _mainCharEx.OnResetUI();                                                              // ĳ���� ���� UI �ʱ�ȭ    

        _mainObjects.ActiveReturnMainMenu();                                                  // ���� �޴� ������Ʈ Ȱ��ȭ
    }
}
