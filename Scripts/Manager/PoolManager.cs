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

    [Header("풀 생성 위치")]
    [SerializeField] private Transform _poolEffectParent;
    [SerializeField] private Transform _poolFontParent;

    private Queue<GameObject> _poolEffects = new Queue<GameObject>();

    private Queue<GameObject> _poolFonts = new Queue<GameObject>();



    public void InitPool()
    {
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
}
