using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageIcon : MonoBehaviour
{
    public SpriteRenderer SelectMarker;
    public bool IsSelected = false;
    [Header("Connection")]
    [SerializeField] private StageIcon iconUp;
    [SerializeField] private StageIcon iconDown;
    [SerializeField] private StageIcon iconLeft;
    [SerializeField] private StageIcon iconRight;

    private float sizeMultiplier = 1.03f;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }
    public enum StageDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    
    public StageIcon GetStageIcon(StageDirection stageDirection)
    {
        switch (stageDirection)
        {
            case StageDirection.Up:
                if(iconUp != null) return iconUp;
                break;
            case StageDirection.Down:
                if (iconDown != null) return iconDown;
                break;
            case StageDirection.Left:
                if (iconLeft != null) return iconLeft;
                break;
            case StageDirection.Right:
                if (iconRight != null) return iconRight;
                break;
            default:
                return null;
                Debug.Log("GetStageIcon Error");
                break;
        }
        return this;
    }

    public void InitializeStageIcon(bool isSelected)
    {
        if (isSelected)
        {
            transform.DOScale(originalScale * sizeMultiplier, 0f);
            SelectWorld(true);
        }
        else
        {
            transform.DOScale(originalScale, 0f);
            SelectWorld(false);
        }
    }

    public void SelectWorld(bool select)
    {
        if (select)
        {
            IsSelected = true;
            SelectMarker.enabled = true;
            transform.DOScale(originalScale * sizeMultiplier, 0.5f);
        }
        else
        {
            SelectMarker.enabled = false;
            IsSelected = false;
            transform.DOScale(originalScale, 0.5f);
        }
    }
}
