using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTurnIconRenderer : MonoBehaviour
{
    [Header("색상")]
    [SerializeField] private Image _turnImage;

    [Header("스프라이트")]
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
