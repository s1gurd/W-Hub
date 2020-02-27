using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using V3D.Controllers;

public class UIControllerBase : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform dot;
    public CloserController CloserBackground;
    public float activeDotScale = 1f;
    private RaycastHit _hit;
    private Vector3 _baseDotScale;
    private Ray ray;


    // Start is called before the first frame update
    void Start()
    {
        if (mainCamera is null)
        {
            mainCamera = Camera.main;
        }

        _baseDotScale = dot.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera is null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera is null) return;

        ray = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

        if (!Physics.Raycast(ray, out _hit)) return;
        if (_hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactive"))
        {
            dot.DOScale(activeDotScale, 0.5f);
            GameState.InteractableLabels.TryGetValue(_hit.collider, out var c);
            if (c && _hit.distance < c?.distTreshold)
            {
                if (Input.GetMouseButtonUp(0) && !GameState._currentPopup && !GameState._currentSlider && c.popup)
                {
                    GameState._currentPopup = c.popup;
                    GameState._currentPopup?.gameObject.SetActive(true);
                    GameState._currentPopup?.Show();
                    CloserBackground.gameObject.SetActive(true);
                    GameState._currentActive?.Hide();
                    GameState.cursorLock = false;
                }

                if (c == GameState._currentActive) return;
                GameState._currentActive?.Hide();
                c.Show();
                GameState._currentActive = c;
            }
            else
            {
                GameState._currentActive?.Hide();
                GameState._currentPopup?.Hide();
                CloserBackground.gameObject.SetActive(false);
                GameState._currentActive = null;
                GameState._currentPopup = null;
            }
        }
        else
        {
            dot.DOScale(_baseDotScale, 0.1f);
            GameState._currentActive?.Hide();
            GameState._currentPopup?.Hide();
            CloserBackground.gameObject.SetActive(false);
            GameState._currentActive = null;
            GameState._currentPopup = null;
        }
    }
}