using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("이펙트 프리펩")]
    [SerializeField] private GameObject _effectPrefab;

    [Header("데미지 폰트 프리펩")]
    [SerializeField] private GameObject _fontPrefab;

    [Header("메세지 박스 프리펩")]
    [SerializeField] private GameObject _msgPrefab;
    [SerializeField] private Transform _msgParent;          // 메세지 박스 위치

    private Transform _poolEffectParent;
    private Transform _poolFontParent;

    private Queue<GameObject> _poolEffects = new Queue<GameObject>();

    private Queue<GameObject> _poolFonts = new Queue<GameObject>();

    private Queue<GameObject> _poolMsg = new Queue<GameObject>();

    public void Awake()
    {
        
    }

    public void InitPoolManger()
    {
        // 메세지 박스 생성
        for(int i = 0; i < 3; i++)
        {
            GameObject msgBox = Instantiate(_msgPrefab, _msgParent);
            msgBox.SetActive(false);                                    // 초기에는 비활성화
            msgBox.name = $"MessageBox_{i + 1}";                        // 이름 설정
            _poolMsg.Enqueue(msgBox);                                   // 메세지 박스 풀에 추가
        }
    }

    public void InitEncounterPool(Transform effectTrans, Transform fontTrans)
    {
        _poolEffectParent = effectTrans;
        _poolFontParent = fontTrans;

       // 이펙트 풀 생성 폼 위치 child 0 => effect , child 1 => font

        for(int i = 0; i < 5; i++)                  // 이펙트
        {
            GameObject effect = Instantiate(_effectPrefab, _poolEffectParent);
            effect.SetActive(false);
            _poolEffects.Enqueue(effect);
        }

        for (int i = 0; i < 30; i++)                // 폰트
        {
            GameObject font = Instantiate(_fontPrefab, _poolFontParent);
            font.SetActive(false);
            _poolFonts.Enqueue(font);
        }
    }

    #region 겟 풀

    public List<GameObject> GetEffectPool(int count)
    {
        List<GameObject> tmpEffects = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            if (_poolEffects.Count > 0)
            {
                GameObject effect = _poolEffects.Dequeue();
                effect.SetActive(true);
                tmpEffects.Add(effect);
            }
            else
            {
                Debug.LogWarning("이펙트 풀에 남은 이펙트가 없습니다.");
                break;
            }
        }
        return tmpEffects;          // 사용 후에는 반드시 ReturnEffectPool()을 호출하여 풀에 반환해야 합니다.
    }

    public GameObject GetFontPool()
    {
        GameObject font = _poolFonts.Dequeue();
        font.SetActive(true);
        Debug.Log($"GetFontPool: {font.name}"); // 디버그용 로그

        return font;          // 사용 후에는 반드시 ReturnFontPool()을 호출하여 풀에 반환해야 합니다.
    }

    public GameObject GetMessagePool()
    {
        GameObject msgBox = _poolMsg.Dequeue();
        return msgBox;          // 메세지 박스 풀에서 하나 꺼내서 반환
    }


    #endregion

    #region 리턴 풀 

    public void ReturnEffectPool(List<GameObject> effects)
    {
        foreach (GameObject effect in effects)                      // 각 애니메이션 값 스프라이트 null 초기화
        {
            Animator animator = effect.GetComponent<Animator>();
            animator.runtimeAnimatorController = null;              // 애니메이터 컨트롤러 초기화

            for(int i = 0; i < effect.transform.childCount; i++)
            {
                Transform child = effect.transform.GetChild(i);
                if (child.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
                {
                    spriteRenderer.sprite = null;                  // 자식 스프라이트 렌더러 초기화
                }
            }
        }

        foreach (GameObject effect in effects)                      // 초기화 로직
        {
            effect.SetActive(false);
            _poolEffects.Enqueue(effect);
        }
    }

    public void ReturnFontPool(GameObject font)
    {
        TextMeshProUGUI tmp = font.GetComponent<TextMeshProUGUI>();

        font.SetActive(false);
        _poolFonts.Enqueue(font);
    }

    public void ReturnMessagePool(GameObject msgBox)
    {
        // 메세지 박스 비활성화
        MessageBox _box = msgBox.GetComponent<MessageBox>();
        _box.OnDisableMessageBox();                                 // 메세지 박스 비활성화
        _poolMsg.Enqueue(msgBox);                                   // 메세지 박스 풀에 반환
    }

    #endregion
}
