using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private RectTransform _parentRectTrans;
    [SerializeField] private RectTransform _textRectTrans;
    [SerializeField] private TextMeshProUGUI _text;

    private Vector2 plusSizeDelta = new Vector2(50f, 40f);       // 텍스트 박스 크기 증가량

    // 즉발성 텍스트를 사용할때만 사용 자주 업뎃에는 사용하지 않음
    public void OnWriteTextBox(string message)                
    {
        gameObject.SetActive(true);
        _text.text = message;

        // 유니티 사이클에서 강제로 _textRect의 하위들을 다 업데이트 시킴 좋은 건 아님
        // 하지만 팝업 정도에선 사용해도 괜찮을 것 같음
        LayoutRebuilder.ForceRebuildLayoutImmediate(_textRectTrans);
        _parentRectTrans.sizeDelta = _textRectTrans.sizeDelta + plusSizeDelta;
    }


    public void OnDisableMessageBox()
    {
        this.gameObject.SetActive(false);           // 메시지 박스 비활성화
    }
}
