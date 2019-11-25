using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using V3D.Controllers;

public class SliderController : MonoBehaviour
{
    public float targetY;
    public float InDuration = 0.3f;
    public float OutDuration = 0.3f;

    private float baseY;

    private bool shown = false;
    // Start is called before the first frame update
    void Start()
    {
        baseY = this.GetComponent<RectTransform>().anchoredPosition.y;
    }

    public void Toggle()
    {
        if (shown)
        {
            Hide();
            shown = false;
            GameState._currentSlider = null;
            return;
        } 
        GameState._currentSlider?.Hide();
        GameState._currentSlider = this;
        Show();
        shown = true;
    }

    public void Show()
    {
        this.GetComponent<RectTransform>().DOAnchorPosY(targetY, InDuration);
    }

    public void Hide()
    {
        this.GetComponent<RectTransform>().DOAnchorPosY(baseY, OutDuration);
    }
    
}
