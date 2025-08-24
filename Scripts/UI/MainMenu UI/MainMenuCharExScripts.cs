using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct CharExSkillData
{
    public SkillType _skillType;                // ��ų Ÿ��
    public TextMeshProUGUI _skillName;          // ��ų �̸�
    public TextMeshProUGUI _skillDescription;   // ��ų ����
}

public class MainMenuCharExScripts : MonoBehaviour
{
    [Header("��ü ������Ʈ")]
    public GameObject _exObject;                // ĳ���� ���� ������Ʈ

    [Header("��")]
    public Image _playerImage;                  // �÷��̾� ��������Ʈ �̹���
    public TextMeshProUGUI _playerName;         // �÷��̾� �̸�

    [Header("���")]
    public TextMeshProUGUI _basicLeftText;      // �⺻ ���� �ؽ�Ʈ
    public TextMeshProUGUI _basicRightText;     // �⺻ ������ �ؽ�Ʈ

    [Header("�Ʒ�")]
    public List<CharExSkillData> _skillDataList; // ��ų ������ ����Ʈ


    // �����ؼ� ����� �뵵, ����
    private string _leftFormat;
    private string _rightFormat;

    private void Start()
    {
        OnInitExUI();
    }

    public void OnInitExUI()
    {
        _leftFormat = _basicLeftText.text;        // ���� �ؽ�Ʈ ���� ����
        _rightFormat = _basicRightText.text;      // ������ �ؽ�Ʈ ���� ����

        OnResetUI();
    }

    public void OnResetUI()
    {
        _exObject.SetActive(false);             // ĳ���� ���� ������Ʈ ��Ȱ��ȭ
    }

    public void OnSelectSkillView(Character unit)
    {
        if(!_exObject.activeSelf) _exObject.SetActive(true); // ĳ���� ���� ������Ʈ Ȱ��ȭ

        CharacterTable table = unit.GetCharTable;         // ĳ���� ���� ��������

        _playerImage.sprite = unit.ReturnPlayerSprite(0); // �÷��̾� ��������Ʈ �̹��� ����
        _playerName.text = table.dataName;                // �÷��̾� �̸� ����

        // ���� ����
        // 0 ü�� 1 ���ݷ� 2 ����
        // 0 �ӵ� 1 �ñر� 2 ũ��Ƽ�� 3 ũ��Ƽ�� ���ط�

        _basicLeftText.text = string.Format(_leftFormat, table.dataHP, table.dataHP, table.dataDef);
        _basicRightText.text = string.Format(_rightFormat, table.dataSpeed, table.dataUlt, table.dataCri, table.dataCriDmg);

        // �� ĳ���� Ư�� ����
        for(int i = 0; i < _skillDataList.Count; i++)
        {
            CharExSkillData data = _skillDataList[i];                   // ��ų ������ ��������
            SkillMetaData skillData = unit.ReturnAct(data._skillType);  // ��ų ��Ÿ ������ ��������

            data._skillName.text = ReturnSkillName(data._skillType);     // ��ų �̸� ����
            data._skillDescription.text = skillData.info;                // ��ų ���� ����
        }

        string ReturnSkillName(SkillType type)
        {
            return type switch
            {
                SkillType.normal => "�⺻ ����",
                SkillType.skill => "��ų",
                SkillType.ult => "�ñر�",
                SkillType.passive => "�нú�",
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
