using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 캐릭터 기본 스탯 저장 용도
/// 캐릭터 스킬 정보 저장
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


    [Header("플레이어블 아이콘")]
    [SerializeField] public Sprite _playerSprite;
    [SerializeField] public Sprite _ultIconSprite;

    [Header("버프 사용 시 아이콘 및 프리펩")]
    [SerializeField] public Sprite _buffSprite;
}
