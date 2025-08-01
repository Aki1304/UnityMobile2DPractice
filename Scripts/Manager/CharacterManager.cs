using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("���� �� ���� �÷��̾�� ĳ����")]
    [SerializeField] private GameObject[] _charPlayer;
    public GameObject[] GetCharPlayer { get { return _charPlayer; } }

    [Header("���� �� ���� ����")]
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
        _typeEnemys = new Dictionary<EnemyType, List<GameObject>>();        // ��ųʸ� ����

        // �÷��̾� , �� stas ���� ��Ű��
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
