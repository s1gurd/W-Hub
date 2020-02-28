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
    public GameObject LectionButton;
    public GameObject LectionMenu;

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

        if (!UIActive && _hit.collider.gameObject.tag.Equals("Interactive", StringComparison.Ordinal))
        {
            LectionButton.SetActive(true);
        }
        else
        {
            LectionButton.SetActive(false);
        }

        if (!UIActive && _hit.collider.gameObject.tag.Equals("NPC", StringComparison.Ordinal))
        {
            UserButton.SetActive(true);
        }
        else
        {
            UserButton.SetActive(false);
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
        
        UIActive = false;
        LectionButton?.SetActive(false);
        LectionMenu?.SetActive(false);

        UserButton?.SetActive(false);
        UserMenu?.SetActive(false);
    }
}