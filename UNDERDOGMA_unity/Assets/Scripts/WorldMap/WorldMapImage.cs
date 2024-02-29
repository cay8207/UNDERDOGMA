using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldMapImage : MonoBehaviour
{
    public bool Unlocked;
    public string WorldScene;
    public int WorldNum;

    [SerializeField]
    private SpriteRenderer selected;
    [SerializeField]
    private SpriteRenderer notSelected;
    [SerializeField]
    private SpriteRenderer locked;
    [SerializeField]
    private SpriteRenderer lockImage;

    private float sizeMultiplier = 1.03f;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        if (Unlocked)
        {
            selected.enabled = true;
            notSelected.enabled = true;
            locked.enabled = false;
            lockImage.enabled = false;
        }
        else
        {
            selected.enabled = false;
            notSelected.enabled = false;
            locked.enabled = true;
            lockImage.enabled = true;
        }
    }

    public void OnMouseEnter()
    {
        WorldMapManager.Instance.SelectedWorld = WorldNum;
        WorldMapManager.Instance.SetInfoPos(WorldNum);
        WorldMapManager.Instance.SetWorldMapImage(WorldNum);
    }
    public void OnMouseDown()
    {
        WorldMapManager.Instance.LoadSelectedWorld();
    }

    public void InitializeWorldImage(bool isSelected)
    {
        transform.localScale = originalScale;
        if (isSelected)
        {
            selected.DOFade(1, 0f);
            notSelected.DOFade(0, 0f);
            transform.DOScale(originalScale * sizeMultiplier, 0f);
        }
        else
        {
            selected.DOFade(0, 0f);
            notSelected.DOFade(1, 0f);
            transform.DOScale(originalScale, 0f);
        }
    }

    public void SelectWorld(bool isSelected)
    {
        if (isSelected)
        {
            selected.DOFade(1, 0f);
            notSelected.DOFade(0, 0f);
            transform.DOScale(originalScale * sizeMultiplier, 0.5f);
        }
        else
        {
            selected.DOFade(0, 0f);
            notSelected.DOFade(1, 0f);
            transform.DOScale(originalScale, 0.5f);
        }
    }
}
