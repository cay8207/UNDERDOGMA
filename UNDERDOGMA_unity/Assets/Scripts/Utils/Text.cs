using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Text : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;

    // 십의 자리를 표현하는 텍스트.
    [SerializeField] private GameObject Text1;
    // 일의 자리를 표현하는 텍스트. 
    [SerializeField] private GameObject Text2;

    private SpriteRenderer _spriteRenderer1;
    private SpriteRenderer _spriteRenderer2;

    private void Awake()
    {
        _spriteRenderer1 = Text1.GetComponent<SpriteRenderer>();
        _spriteRenderer2 = Text2.GetComponent<SpriteRenderer>();
    }

    public void SetText(int num)
    {
        if (num >= 0 && num < 10)
        {
            _spriteRenderer1.sprite = _sprites[num];
            _spriteRenderer2.sprite = null;
        }
        else if (num >= 10)
        {
            _spriteRenderer1.sprite = _sprites[num / 10];
            Text1.transform.localPosition = new Vector3(-0.03f, 0, 0);
            _spriteRenderer2.sprite = _sprites[num % 10];
            Text2.transform.localPosition = new Vector3(0.08f, 0, 0);
        }
    }
}
