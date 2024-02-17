using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class World5 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void Start()
    {

    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        MinGyuManager.Instance._selectedIndex = 4;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {

    }
}
