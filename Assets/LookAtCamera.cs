using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform _target;

    private RectTransform _rectTransform;
    
    // Start is called before the first frame update
    void Start()
    {

        _rectTransform = this.gameObject.GetComponent<RectTransform>();
        _target = Camera.main?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
        {
            _target = Camera.main?.transform;
        }
        if (_target == null)  return;
        var p = _target.position;
        Vector3 targetPostition = new Vector3( p.x, 
            _rectTransform.position.y, 
            p.z ) ;
        _rectTransform.LookAt(targetPostition);
    }
}
