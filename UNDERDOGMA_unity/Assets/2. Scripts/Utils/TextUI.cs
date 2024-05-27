using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;

    // 십의 자리를 표현하는 텍스트.
    [SerializeField] private GameObject Text1;
    // 일의 자리를 표현하는 텍스트. 
    [SerializeField] private GameObject Text2;

    private Image _image1;
    private Image _image2;

    private void Awake()
    {
        _image1 = Text1.GetComponent<Image>();
        _image2 = Text2.GetComponent<Image>();
    }

    public void SetText(int num)
    {
        if (num >= 0 && num < 10)
        {
            _image1.sprite = _sprites[num];
            Text1.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 430.0f, 0.0f);
            Text1.GetComponent<RectTransform>().sizeDelta = new Vector2(37.0f, 50.0f);

            _image2.enabled = false;
        }
        else if (num >= 10)
        {
            _image1.sprite = _sprites[num / 10];
            Text1.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 430.0f, 0.0f);
            Text1.GetComponent<RectTransform>().sizeDelta = new Vector2(37.0f, 50.0f);

            _image2.enabled = true;
            _image2.sprite = _sprites[num % 10];
            Text2.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 430.0f, 0.0f);
            Text2.GetComponent<RectTransform>().sizeDelta = new Vector2(37.0f, 50.0f);
        }
    }
}
