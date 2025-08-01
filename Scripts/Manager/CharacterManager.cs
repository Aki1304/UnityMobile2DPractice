using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("게임 내 등장 플레이어블 캐릭터")]
    [SerializeField] private GameObject[] _charPlayer;
    public GameObject[] GetCharPlayer { get { return _charPlayer; } }

    [Header("게임 내 등장 몬스터")]
    [SerializeField] private GameObject[] _charEnemy;

    public Dictionary<EnemyType, List<GameObject>> _typeEnemys;

    public List<GameObject> ReturnTypeEnemy(EnemyType methodType)
    {
        List<GameObject> returnList;

        _typeEnemys.TryGetValue(methodType, out returnList);
        return returnList;
    }

    public void InitCharManager()
    {
        _typeEnemys = new Dictionary<EnemyType, List<GameObject>>();        // 딕셔너리 생성

        // 플레이어 , 적 stas 연결 시키기
        foreach(var unit in _charPlayer)
        {
            Character player = unit.GetComponent<Character>();
            player.InitCharacter();
        }

        foreach (var unit in _charEnemy)
        {
            Character enemy = unit.GetComponent<Character>();
            enemy.InitCharacter();
        }
    }

    void DictonaryAddbjects()        
    {
        foreach(var unit in _charEnemy)
        {
            Enemy enemy = unit.GetComponent<Enemy>();
            EnemyType enemyType = enemy._enemyType;

            if (!_typeEnemys.ContainsKey(enemyType))
            {
                _typeEnemys[enemyType] = new List<GameObject>();
            }

            _typeEnemys[enemyType].Add(unit);
        }
    }


    void Start()
    {
        DictonaryAddbjects();
    }
}
