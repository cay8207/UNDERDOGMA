using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class World2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        MinGyuManager.Instance._selectedIndex = 1;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {

    }

    void OnClick()
    {
        SceneManager.LoadScene("World2");
    }
}
