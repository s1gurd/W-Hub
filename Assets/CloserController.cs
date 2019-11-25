using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using V3D.Controllers;
using UnityEngine.UI;

public class CloserController : MonoBehaviour
{
    private PopupController _caller;

    public void Show(PopupController caller)
    {
        _caller = caller;
    }

   
    public void onClick()
    {
        GameState._currentPopup.Hide();
        gameObject.SetActive(false);
    }
}
