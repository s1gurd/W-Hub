using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Entities;
using UnityEngine.SceneManagement;
using V3D.Controllers;

public class UIControllerBase : MonoBehaviour
{
    public GameObject ActivitiesButton;
    public GameObject ActivitiesMenu;

    public GameObject LectionButton;
    public GameObject NegotiationButton;

    public GameObject UserButton;
    public GameObject UserMenu;

    public float distanceThreshold = 20;
    private Camera _mainCamera;

    private RaycastHit _hit;

    private Ray ray;

    [HideInInspector]
    public bool UIActive = false;


    // Start is called before the first frame update
    void Start()
    {
        if (_mainCamera is null)
        {
            _mainCamera = Camera.main;
        }

        HideAll();
    }

    private void Update()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        if (_mainCamera == null) return;

        ray = _mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

        if (!Physics.Raycast(ray, out _hit) || _hit.distance > distanceThreshold)
        {
            if (!UIActive) HideAll();
            return;
        }

        if (UIActive) return;

        HideButtons();
        
        switch (_hit.collider.gameObject.tag)
        {
            case "Interactive": ActivitiesButton.SetActive(true);
                break;
            case "Lection": LectionButton.SetActive(true);
                break;
            case "Negotiation": NegotiationButton.SetActive(true);
                break;


        }
        
        
    }

    public void SetUIInactive()
    {
        UIActive = false;
    }

    public void SetUIActive()
    {
        UIActive = true;
    }

    public void LoadLevel(int level)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityArray = entityManager.GetAllEntities();
        foreach (var e in entityArray)
            entityManager.DestroyEntity(e);
        entityArray.Dispose();
        SceneManager.LoadScene(level);
    }

    private void HideAll()
    {
        ActivitiesMenu?.SetActive(false);
        
        UIActive = false;
        
        HideButtons();

        UserButton?.SetActive(false);
        UserMenu?.SetActive(false);
    }

    private void HideButtons()
    {
        ActivitiesButton?.SetActive(false);
        LectionButton?.SetActive(false);
        NegotiationButton?.SetActive(false);
    }
}