using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EncounterStage : MonoBehaviour
{
    [Header("캐릭터 위치")]
    [SerializeField] private Transform _playerPosition;

    [Header("적 위치")]
    [SerializeField] private Transform _enemyPosition;

    [Header("플레이어 인스턴스 위치")]
    [SerializeField] private Transform _playerInstanceTransform;

    [Header("적 인스턴스 위치")]
    [SerializeField] private Transform _enemyInstanceTransform;

    [SerializeField] private GameObject _instanceEnemyUI;

    private Vector3[] _charPos = new Vector3[4];
    private Vector3[] _enemyPos = new Vector3[5];

    private EncounterContext _context;
    public EncounterDual _dual { get { return _context._dual; } }
    public void InitStage(EncounterContext ct)
    {
        _context = ct;
    }

    public void SetInitCharPos()
    {
        for (int i = 0; i < 4; i++)                 // 플레이어 파티 위치 
        {
            _charPos[i] = _playerPosition.GetChild(i).transform.position;
        }

        for (int i = 0; i < 5; i++)                 // 적 파티 위치 
        {
            _enemyPos[i] = _enemyPosition.GetChild(i).transform.position;
        }
    }

    public void SetEncounterPlayerPos()          // 파티 저장 배열에서 가져오기
    {
        Helper.DualManager.EnemySkillEffectPos = (_charPos[1] / 2f) + (_charPos[2] / 2f) + new Vector3(0, 1.2f, 0);                       // 플레이어 2번과 3번 사이 위치

        Debug.Log($"적 스킬 이펙트 위치: {Helper.DualManager.EnemySkillEffectPos}");

        for (int i = 0; i < 4; i++)
        {
            int minusScaleX = (i < 2) ? -1 : 1;          // 1번 2번 왼쪽    3번 4번 오른쪽

            GameObject charObject = Instantiate(Helper.PartyManager.GetPartyInfo[i], _playerInstanceTransform);
            Character unit = charObject.GetComponent<Character>();
            Transform charTrans = charObject.transform;

            charTrans.position = _charPos[i];
            charTrans.localScale =
                new Vector3(charTrans.localScale.x * minusScaleX, charTrans.localScale.y, charTrans.localScale.z);

            _context._dual._currentParty[i] = charObject;
            unit.InitCharacter();                        // 프리펩 생성 적용
        }

        // 듀얼 매니저 할 일
        Helper.DualManager.GetInitDual(_context._dual._currentParty);
    }

    public void SetEncounterEnemyPos()          // 현재 라운드이 몬스터 웨이브 단계 확인하고 소환
    {
        int size = _dual._currentWaveEnemy.Count(obj => obj != null);

        // 몬스터 초기 설정
        for (int i = 0; i < size; i++)
        {
            GameObject charObject = Instantiate(_dual._currentWaveEnemy[i], _enemyInstanceTransform);      // 인스탄티에이트로 소환
            Character unit = charObject.GetComponent<Character>();
            unit.InitCharacter();

            GameObject uiObject = Instantiate(_instanceEnemyUI, charObject.transform);
            EnemyUI ui = uiObject.GetComponent<EnemyUI>();
            ui.InitUI(unit);  // 이 안에서 바로 OnTakeDamage += OnHpChange 호출됨

            BoxCollider col = charObject.GetComponent<BoxCollider>();
            Vector3 uiPos = new Vector3(col.bounds.center.x, col.bounds.max.y + 0.2f, col.bounds.center.z);

            uiObject.transform.position = uiPos;

            _dual._currentWaveEnemy[i] = charObject;

            unit.GetStats.UpdateFinalStats();
        }

        // 몬스터 위치 설정
        ResetEnemyPos();
    }

    public void ResetEnemyPos()
    {
        int[] startCounts = new int[5] { 2, 1, 1, 0, 0 };                                               // 몬스터 위치 배치용 시작 인덱스 배열 (남은 몬스터 수에 따라 가운데 정렬)   
        int enemyCounts = _dual._currentWaveEnemy.Count(obj => obj != null);                   // 현재 살아있는(=null이 아닌) 몬스터 수 계산  

        if (enemyCounts == 0) return;                                                                   // 몬스터가 하나도 없으면 함수 종료   

        int startIndex = startCounts[enemyCounts - 1];                                                   // 시작 인덱스: 남은 몬스터 수에 따라 가운데 정렬 시작 위치 결정
        int size = startIndex + enemyCounts;                                                             // 마지막 인덱스: 시작 위치 + 몬스터 수
        int count = 0;                                                                                   // 몬스터 위치 수정용

        // _currentWaveEnemy를 tempObject에 복사
        GameObject[] tempObject = new GameObject[5];
        Array.Copy(_dual._currentWaveEnemy, tempObject, 5);

        // 살아있는 몬스터만 왼쪽부터 채우기 (순서 유지)
        int aliveIdx = 0;
        for (int i = 0; i < tempObject.Length; i++)
        {
            if (tempObject[i] != null)
            {
                _dual._currentWaveEnemy[aliveIdx] = tempObject[i];
                aliveIdx++;
            }
        }

        // 나머지 칸은 null로 채우기
        for (int i = aliveIdx; i < tempObject.Length; i++)
        {
            _dual._currentWaveEnemy[i] = null;
        }

        for (int i = startIndex; i < size; i++)
        {
            GameObject enemy = _dual._currentWaveEnemy[count];                                  // 현재 몬스터 가져오기
            enemy.transform.position = _enemyPos[i];                                                     // 몬스터 위치 설정
            count++;
        }
    }

}
