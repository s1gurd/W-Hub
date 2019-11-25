using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using V3D.Controllers;

public class PopupController : MonoBehaviour
{
    private CanvasGroup c;
    public float InDuration = 0.3f;
    public float OutDuration = 0.1f;

    void Awake()
    {
        c = GetComponent<CanvasGroup>();
    }
    public void Show()
    {
        c.alpha = 0f;
        c.DOFade(1f, InDuration);
    }

    public void Hide()
    {
        var s = DOTween.Sequence();
        s.Append(c.DOFade(0f, OutDuration)).AppendCallback(() =>
        {
            GameState._currentPopup = null;
            GameState.cursorLock = true;
            c.gameObject.SetActive(false);
        });

    }
}
