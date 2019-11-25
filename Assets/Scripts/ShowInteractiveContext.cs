using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using V3D.Controllers;

public class ShowInteractiveContext : MonoBehaviour
{
    public CanvasGroup interactive;
    public float InDuration = 0.3f;
    public float OutDuration = 0.1f;
    public float distTreshold = 4f;

    public PopupController popup;

    private void Start()
    {
        GameState.InteractableLabels.Add(this.gameObject.GetComponent<Collider>(),this);
    }

    public void Show()
    {
        interactive.gameObject.SetActive(true);
        interactive.alpha = 0f;
        interactive.DOFade(1f, InDuration);
    }

    public void Hide()
    {
        var s = DOTween.Sequence();
        s.Append(interactive.DOFade(0f, OutDuration)).AppendCallback(() =>
        {
           
            interactive.gameObject.SetActive(false);
        });
    }
}
