using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ĳ���� �⺻ ���� ���� �뵵
/// ĳ���� ��ų ���� ����
/// </summary>
[CreateAssetMenu(fileName = "CharacterTable", menuName = "CharData/CharacterTable")]
public class CharacterTable : ScriptableObject
{
    public int unitCode;
    public string dataName;
    public float dataATk;
    public float dataDef;
    public float dataCri;
    public float dataCriDmg;
    public int dataHP;
    public int dataUlt;
    public float dataSpeed;


    [Header("�÷��̾�� ������")]
    [SerializeField] public Sprite _playerSprite;
    [SerializeField] public Sprite _ultIconSprite;

    [Header("���� ��� �� ������ �� ������")]
    [SerializeField] public Sprite _buffSprite;
}
