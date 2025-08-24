using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;

public class CharSlotsUI : MonoBehaviour
{
    public Character GetCharacter { get; private set; } // 캐릭터 컴포넌트

    public Image _playersprite;          // 플레이어 아이콘
    public Image _borderSprite;          // 플레이어 아이콘 테두리
    public GameObject _numIcon;          // 플레이어 아이콘 숫자
    public TextMeshProUGUI _numText;     // 플레이어 아이콘 숫자 텍스트
    public TextMeshProUGUI _charName;    // 캐릭터 이름

    // 기본 캐릭터 리스트 초기화 및 설정
    public void InitSlot(Character unit)
    {
        GetCharacter = unit;            // 캐릭터 컴포넌트 설정

        // 플레이어 아이콘
        _playersprite.sprite = GetCharacter.ReturnPlayerSprite(0); // 플레이어 스프라이트 설정
        // 플레이어 아이콘 숫자
        SetNumIcon(false, 0);
        // 캐릭터 이름
        _charName.text = GetCharacter.GetCharTable.dataName;
    }

    public void SetNumIcon(bool active,int idxNum)
    {
        if(active)                      // 킬 경우
        {
            idxNum += 1;                // 숫자 1 증가
            _numText.text = idxNum.ToString();
            _numIcon.SetActive(true);
        }
        else _numIcon.SetActive(false); // 비활성화
    }

    // 선택된 파티 리스트 UI 업데이트
    public void SelectListUI(Character unit)
    {
        if(unit is null)
        {
            GetCharacter = null;        // 캐릭터 테이블이 null인 경우
            _playersprite.sprite = null; // 플레이어 아이콘 비활성화
            _playersprite.color = new Color(1, 1, 1, 0); // 투명하게 설정
            return;
        }

        GetCharacter = unit;                                         // 선택된 캐릭터 테이블을 설정
        _playersprite.sprite = GetCharacter.ReturnPlayerSprite(0);   // 캐릭터 선택 UI 업데이트
        _playersprite.color = new Color(1, 1, 1, 1);                 // 불투명하게 설정
    }
}
