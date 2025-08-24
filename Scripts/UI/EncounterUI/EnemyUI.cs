using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EnemyUI : MonoBehaviour
{
    private Character _enemy;

    [SerializeField] private Image _enemyHpImage;
    [SerializeField] private Image _enemyStunImage;
    [SerializeField] private TextMeshProUGUI _enemyHpText;

    public void InitUI(Character enemy)
    {
        _enemy = enemy;
        _enemy.OnTakeDamage += OnHpChange;

        OnHpChange();
    }

    public void OnHpChange()
    {
        _enemyHpImage.fillAmount = _enemy.GetStats.Amount(0);
        _enemyHpText.text = string.Format("{0} %", Mathf.RoundToInt(_enemy.GetStats.Amount(0) * 100f));
    }

    private void OnDestroy()
    {
        if (_enemy != null)
            _enemy.OnTakeDamage -= OnHpChange;
    }


}
