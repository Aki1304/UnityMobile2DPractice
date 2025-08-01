using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("����Ʈ ������")]
    [SerializeField] private GameObject _effectPrefab;

    [Header("������ ��Ʈ ������")]
    [SerializeField] private GameObject _fontPrefab;

    [Header("Ǯ ���� ��ġ")]
    [SerializeField] private Transform _poolEffectParent;
    [SerializeField] private Transform _poolFontParent;

    private Queue<GameObject> _poolEffects = new Queue<GameObject>();

    private Queue<GameObject> _poolFonts = new Queue<GameObject>();



    public void InitPool()
    {
       // ����Ʈ Ǯ ���� �� ��ġ child 0 => effect , child 1 => font

        for(int i = 0; i < 5; i++)                  // ����Ʈ
        {
            GameObject effect = Instantiate(_effectPrefab, _poolEffectParent);
            effect.SetActive(false);
            _poolEffects.Enqueue(effect);
        }

        for (int i = 0; i < 30; i++)                // ��Ʈ
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
                Debug.LogWarning("����Ʈ Ǯ�� ���� ����Ʈ�� �����ϴ�.");
                break;
            }
        }
        return tmpEffects;          // ��� �Ŀ��� �ݵ�� ReturnEffectPool()�� ȣ���Ͽ� Ǯ�� ��ȯ�ؾ� �մϴ�.
    }

    public GameObject GetFontPool()
    {
        GameObject font = _poolFonts.Dequeue();
        font.SetActive(true);
        Debug.Log($"GetFontPool: {font.name}"); // ����׿� �α�

        return font;          // ��� �Ŀ��� �ݵ�� ReturnFontPool()�� ȣ���Ͽ� Ǯ�� ��ȯ�ؾ� �մϴ�.
    }

    public void ReturnEffectPool(List<GameObject> effects)
    {
        foreach (GameObject effect in effects)                      // �� �ִϸ��̼� �� ��������Ʈ null �ʱ�ȭ
        {
            Animator animator = effect.GetComponent<Animator>();
            animator.runtimeAnimatorController = null;              // �ִϸ����� ��Ʈ�ѷ� �ʱ�ȭ

            for(int i = 0; i < effect.transform.childCount; i++)
            {
                Transform child = effect.transform.GetChild(i);
                if (child.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
                {
                    spriteRenderer.sprite = null;                  // �ڽ� ��������Ʈ ������ �ʱ�ȭ
                }
            }
        }

        foreach (GameObject effect in effects)                      // �ʱ�ȭ ����
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
