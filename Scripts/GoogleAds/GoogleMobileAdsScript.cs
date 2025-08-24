using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

/// <summary>
/// 보상형 리워드 테스트 ID 번호
/// Andrioid => ca-app-pub-3940256099942544/5224354917
/// </summary>


public class GoogleMobileAdsScript
{
    private const string testUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트용 광고 단위 ID
    private RewardedAd rewardedAd; // 광고 객체 저장


    public void GooldAdInit()
    {
        InitializeAds(); // 구글 모바일 광고 SDK 초기화
        LoadRewardedAd(); // 리워드 광고 로드
    }

    public void InitializeAds()
    {
        // Google Mobile Ads SDK 초기화
        MobileAds.Initialize(initStatus => { });                        // 초기화 콜백은 비움
    }

    public void LoadRewardedAd()
    {
        // 리퀘스트 생성
        var adRequest = new AdRequest();

        // 리워드 광고 요청
        RewardedAd.Load(testUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                // 실패 했을 경우 아래 실행
                // 실패 팝업 실행
                GameObject msgObject = Helper.PoolManager.GetMessagePool();
                MessageBox box = msgObject.GetComponent<MessageBox>();
                box.OnWriteTextBox("광고를 불러오는데 실패 했습니다. \n 잠시 후 다시 시도 해주세요.");
                return;
            }
            // 성공 했을 경우

            // 리워드를 저장한다.
            rewardedAd = ad;
            Debug.Log("리워드 광고 로드 성공");

            // 광고를 다 보았다면 미리 로드 시키기
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("광고가 닫혔습니다. 다음 광고를 로드합니다.");
                LoadRewardedAd(); // 광고가 닫히면 다음 광고를 로드
            };

        });
    }

    public void ShowRerdedAd()
    {
        // rewardedAd가 null이 아니고 광고를 보여줄 수 있는 상태인지 확인
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            // 광고가 준비되었으면 보여줌
            rewardedAd.Show((Reward reward) =>
            {
                // 광고를 다 본 후 보상을 지급
                Debug.Log($"보상 지급: {reward.Amount} {reward.Type}");
                GiftReward();                   // 보상 지급 임시 함수
            });
        }
        else
        {
            Debug.Log("리워드 광고가 준비되지 않았습니다. 다시 시도해주세요.");
            LoadRewardedAd(); // 광고가 준비되지 않았으면 다시 로드
        }
    }

    public void GiftReward()
    {
        // 광고를 통해 보상을 지급하는 로직을 여기에 추가
        // 예: 플레이어에게 코인, 아이템 등을 지급

        // 성공 했을 경우 아래 실행
        // 성공 팝업 실행
        GameObject msgObject = Helper.PoolManager.GetMessagePool();
        MessageBox box = msgObject.GetComponent<MessageBox>();
        box.OnWriteTextBox("보상을 지급합니다.");

        Debug.Log("보상 지급 로직 실행");
    }

}