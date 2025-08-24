using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

/// <summary>
/// ������ ������ �׽�Ʈ ID ��ȣ
/// Andrioid => ca-app-pub-3940256099942544/5224354917
/// </summary>


public class GoogleMobileAdsScript
{
    private const string testUnitId = "ca-app-pub-3940256099942544/5224354917"; // �׽�Ʈ�� ���� ���� ID
    private RewardedAd rewardedAd; // ���� ��ü ����


    public void GooldAdInit()
    {
        InitializeAds(); // ���� ����� ���� SDK �ʱ�ȭ
        LoadRewardedAd(); // ������ ���� �ε�
    }

    public void InitializeAds()
    {
        // Google Mobile Ads SDK �ʱ�ȭ
        MobileAds.Initialize(initStatus => { });                        // �ʱ�ȭ �ݹ��� ���
    }

    public void LoadRewardedAd()
    {
        // ������Ʈ ����
        var adRequest = new AdRequest();

        // ������ ���� ��û
        RewardedAd.Load(testUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                // ���� ���� ��� �Ʒ� ����
                // ���� �˾� ����
                GameObject msgObject = Helper.PoolManager.GetMessagePool();
                MessageBox box = msgObject.GetComponent<MessageBox>();
                box.OnWriteTextBox("���� �ҷ����µ� ���� �߽��ϴ�. \n ��� �� �ٽ� �õ� ���ּ���.");
                return;
            }
            // ���� ���� ���

            // �����带 �����Ѵ�.
            rewardedAd = ad;
            Debug.Log("������ ���� �ε� ����");

            // ���� �� ���Ҵٸ� �̸� �ε� ��Ű��
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("���� �������ϴ�. ���� ���� �ε��մϴ�.");
                LoadRewardedAd(); // ���� ������ ���� ���� �ε�
            };

        });
    }

    public void ShowRerdedAd()
    {
        // rewardedAd�� null�� �ƴϰ� ���� ������ �� �ִ� �������� Ȯ��
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            // ���� �غ�Ǿ����� ������
            rewardedAd.Show((Reward reward) =>
            {
                // ���� �� �� �� ������ ����
                Debug.Log($"���� ����: {reward.Amount} {reward.Type}");
                GiftReward();                   // ���� ���� �ӽ� �Լ�
            });
        }
        else
        {
            Debug.Log("������ ���� �غ���� �ʾҽ��ϴ�. �ٽ� �õ����ּ���.");
            LoadRewardedAd(); // ���� �غ���� �ʾ����� �ٽ� �ε�
        }
    }

    public void GiftReward()
    {
        // ���� ���� ������ �����ϴ� ������ ���⿡ �߰�
        // ��: �÷��̾�� ����, ������ ���� ����

        // ���� ���� ��� �Ʒ� ����
        // ���� �˾� ����
        GameObject msgObject = Helper.PoolManager.GetMessagePool();
        MessageBox box = msgObject.GetComponent<MessageBox>();
        box.OnWriteTextBox("������ �����մϴ�.");

        Debug.Log("���� ���� ���� ����");
    }

}