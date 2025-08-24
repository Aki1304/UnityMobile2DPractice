using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllDamageUI : MonoBehaviour
{
    public int totalDamage = 0;

    [SerializeField] private TextMeshProUGUI _damgeTMP;

    [Header("���� ������Ʈ")]
    [SerializeField] private GameObject _damageObject;
    [SerializeField] private RectTransform rect;

    private bool isAct = false;

    private float _normalSize = 80f;              // �Ϲ� ���� ũ�� ����
    public float _focusSize = 120f;              // ���� ũ�� ����

    private void Start()
    {
        Helper.UM.GetAllDMGUI = this;
    }

    public void OnAllDamgeUI()
    {
        isAct = true;

        rect.anchoredPosition = new Vector2(0f, -200f);

        _damageObject.SetActive(true);
        StartCoroutine(ActionFont());
    }

    public IEnumerator ActionFont()
    {
        string stringText = totalDamage.ToString();
        string tempText = "";

        for(int i = 0; i < stringText.Length; i++)
        {
            tempText += stringText[i];
            _damgeTMP.text = tempText;
            yield return new WaitForSeconds(0.05f);
        }

        _damgeTMP.fontSize = _focusSize;              // ���� ũ�� ����
        StartCoroutine(RoutineCheck());               // �ڷ�ƾ ����
    }

    private IEnumerator RoutineCheck()
    {
        float duraiton = 0f;
        float targetTime = 2.5f;

        float targetPosX = 400f;

        while(duraiton < targetTime)
        {
            // if������ ���� ����� �׶� �ٷ� ������
            if (!isAct) break;

            duraiton += Time.deltaTime;          // �ð� ���� 
            yield return null;
        }

        while(true)
        {
            // if������ ���� ����� �׶� �ٷ� ������
            if (!isAct) break;

            float velocity = 0f; // Ŭ���� ��� ������ ���� �ʿ�
            float update = Mathf.SmoothDamp(rect.anchoredPosition.x, targetPosX, ref velocity, 0.1f);

            rect.anchoredPosition = new Vector2(update, rect.anchoredPosition.y);

            if(rect.anchoredPosition.x == targetPosX) { break; }
            yield return null;
        }

        // Ȥ�� �ٷ� ���ð�츦 ���
        rect.anchoredPosition = new Vector2(targetPosX, rect.anchoredPosition.y);
    }


    public void ResetDamage()
    {
        isAct = false;                        // �ڷ�ƾ ��� ����
        _damageObject.SetActive(false);

        totalDamage = 0;
        _damgeTMP.text = totalDamage.ToString();

        _damgeTMP.fontSize = _normalSize;      // ���� ũ�� �ʱ�ȭ
    }
}
