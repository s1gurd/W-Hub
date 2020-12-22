using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class findCamera : MonoBehaviour
{
    private Canvas canvas;
    private bool set;
    
    // Start is called before the first frame update
    void Start()
    {
        canvas = this.GetComponent<Canvas>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!set)
        {
            var c = Camera.main;
            if (c != null)
            {
                canvas.worldCamera = c;
                set = true;
            }
        }
    }
}
