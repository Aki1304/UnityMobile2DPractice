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

    private Vector2 plusSizeDelta = new Vector2(50f, 40f);       // �ؽ�Ʈ �ڽ� ũ�� ������

    // ��߼� �ؽ�Ʈ�� ����Ҷ��� ��� ���� �������� ������� ����
    public void OnWriteTextBox(string message)                
    {
        gameObject.SetActive(true);
        _text.text = message;

        // ����Ƽ ����Ŭ���� ������ _textRect�� �������� �� ������Ʈ ��Ŵ ���� �� �ƴ�
        // ������ �˾� �������� ����ص� ������ �� ����
        LayoutRebuilder.ForceRebuildLayoutImmediate(_textRectTrans);
        _parentRectTrans.sizeDelta = _textRectTrans.sizeDelta + plusSizeDelta;
    }


    public void OnDisableMessageBox()
    {
        this.gameObject.SetActive(false);           // �޽��� �ڽ� ��Ȱ��ȭ
    }
}
