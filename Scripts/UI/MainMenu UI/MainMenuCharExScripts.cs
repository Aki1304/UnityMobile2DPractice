using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct CharExSkillData
{
    public SkillType _skillType;                // 스킬 타입
    public TextMeshProUGUI _skillName;          // 스킬 이름
    public TextMeshProUGUI _skillDescription;   // 스킬 설명
}

public class MainMenuCharExScripts : MonoBehaviour
{
    [Header("전체 오브젝트")]
    public GameObject _exObject;                // 캐릭터 설명 오브젝트

    [Header("위")]
    public Image _playerImage;                  // 플레이어 스프라이트 이미지
    public TextMeshProUGUI _playerName;         // 플레이어 이름

    [Header("가운데")]
    public TextMeshProUGUI _basicLeftText;      // 기본 왼쪽 텍스트
    public TextMeshProUGUI _basicRightText;     // 기본 오른쪽 텍스트

    [Header("아래")]
    public List<CharExSkillData> _skillDataList; // 스킬 데이터 리스트


    // 저장해서 끌어쓰는 용도, 포맷
    private string _leftFormat;
    private string _rightFormat;

    private void Start()
    {
        OnInitExUI();
    }

    public void OnInitExUI()
    {
        _leftFormat = _basicLeftText.text;        // 왼쪽 텍스트 포맷 저장
        _rightFormat = _basicRightText.text;      // 오른쪽 텍스트 포맷 저장

        OnResetUI();
    }

    public void OnResetUI()
    {
        _exObject.SetActive(false);             // 캐릭터 설명 오브젝트 비활성화
    }

    public void OnSelectSkillView(Character unit)
    {
        if(!_exObject.activeSelf) _exObject.SetActive(true); // 캐릭터 설명 오브젝트 활성화

        CharacterTable table = unit.GetCharTable;         // 캐릭터 스탯 가져오기

        _playerImage.sprite = unit.ReturnPlayerSprite(0); // 플레이어 스프라이트 이미지 설정
        _playerName.text = table.dataName;                // 플레이어 이름 설정

        // 스탯 정리
        // 0 체력 1 공격력 2 방어력
        // 0 속도 1 궁극기 2 크리티컬 3 크리티컬 피해량

        _basicLeftText.text = string.Format(_leftFormat, table.dataHP, table.dataHP, table.dataDef);
        _basicRightText.text = string.Format(_rightFormat, table.dataSpeed, table.dataUlt, table.dataCri, table.dataCriDmg);

        // 각 캐릭터 특성 설명
        for(int i = 0; i < _skillDataList.Count; i++)
        {
            CharExSkillData data = _skillDataList[i];                   // 스킬 데이터 가져오기
            SkillMetaData skillData = unit.ReturnAct(data._skillType);  // 스킬 메타 데이터 가져오기

            data._skillName.text = ReturnSkillName(data._skillType);     // 스킬 이름 설정
            data._skillDescription.text = skillData.info;                // 스킬 설명 설정
        }

        string ReturnSkillName(SkillType type)
        {
            return type switch
            {
                SkillType.normal => "기본 공격",
                SkillType.skill => "스킬",
                SkillType.ult => "궁극기",
                SkillType.passive => "패시브",
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
