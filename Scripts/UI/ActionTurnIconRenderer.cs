using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTurnIconRenderer : MonoBehaviour
{
    [Header("����")]
    [SerializeField] private Image _turnImage;

    [Header("��������Ʈ")]
    [SerializeField] private Image _unitImage;

    public void SetRenderer(Color color, Sprite sprite)
    {
        _turnImage.color = color;
        _unitImage.sprite = sprite;
    }

    public void ExtraSetRenderer(Sprite sprite)
    {
        _unitImage.sprite = sprite;
    }
}
