using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncounterPlayerUI : MonoBehaviour
{
    [Header("플레이어 인포")]
    public Image _playerSprite;
    public Image _ultIconSprite;
    public Image _ultGaugeSprite;
    public Image _hpSprite;
    public TextMeshProUGUI _hpText;
    public Transform _buffTransform;
    public GameObject[] _stackObject;
    public Image[] _stackIcon;

    [Header("프리펩")]
    [SerializeField] private GameObject _instancePrefab;

    // 리스트로 관리하는게 맞다.
    // 중간에 사라져야 할 경우의 수가 너무 많다.

    private List<GameObject> _buffUiList = new List<GameObject>();
    private List<Image> _buffImages = new List<Image>();    

    public void InitBuffList(Character player)
    {
        _buffImages.Clear();
        _buffUiList.Clear();

        player._charBuffManager.OnUpdateBuff += UpdateBuffList;
        
        for (int i = 0; i < 15; i++)
        {
            GameObject instance = Instantiate(_instancePrefab, _buffTransform);
            Image image = instance.transform.GetChild(1).GetComponent<Image>();

            instance.SetActive(false);
            _buffImages.Add(image);
            _buffUiList.Add(instance);
        }

        UpdateBuffList(player._charBuffManager.GetActiveBuffs);
    }

    public void UpdateBuffList(List<IBuffInfo> infos)
    {
        for (int i = 0; i < _buffUiList.Count; i++)
        {
            if (i < infos.Count)
            {
                Image type = _buffUiList[i].transform.GetChild(2).GetChild(1).GetComponent<Image>();
                int typeIdx = (infos[i].actionType == SkillActionType.Buff) ? 0 : 1;

                _buffImages[i].sprite = infos[i].ReturnSprite();
                type.sprite = Helper.UM._typeSprite[typeIdx];
                _buffUiList[i].gameObject.SetActive(true);
            }
            else
            {
                _buffUiList[i].SetActive(false);
            }
        }
    }

}
