using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// 가장 처음 메인 메뉴 실행 프로세스
    /// </summary>

    [Header("컴포넌트")]
    public MainMenuObjectScripts _mainObjects;   // 메인 메뉴 오브젝트 스크립트 참조
    public MainMenuCharExScripts _mainCharEx;    // 메인 메뉴 캐릭터 설명 스크립트 참조

    [Header("캐릭터 슬룻 리스트")]
    public List<CharSlotsUI> _charSlotsUI; // 캐릭터 슬롯 이미지 리스트

    public GoogleMobileAdsScript mobileAds;
    public MainMenuCharList _mainCharList;       // 메인 메뉴 캐릭터 리스트 스크립트 참조

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
        mobileAds = new GoogleMobileAdsScript();    // 구글 모바일 광고 스크립트 초기화
        _mainCharList = new MainMenuCharList();     // 메인 메뉴 캐릭터 리스트 스크립트 초기화

        mobileAds.GooldAdInit();                    // 구글 모바일 광고 초기화
        _mainCharList.InitCharList();              // 메인 메뉴 캐릭터 리스트 초기화
    }

    void Process()
    {
        Helper.UM._fadeEffect.FadeOut();            // 페이드 아웃 실행

        OnUIReset();                  // UI 초기화
    }

    public void OnUIReset()
    {
        _mainCharList.UpdateUI(_mainObjects.rightCharList, _mainObjects.rightSelectCharList); // 캐릭터 리스트 UI 업데이트   
        _mainCharEx.OnResetUI();                                                              // 캐릭터 설명 UI 초기화    

        _mainObjects.ActiveReturnMainMenu();                                                  // 메인 메뉴 오브젝트 활성화
    }
}
