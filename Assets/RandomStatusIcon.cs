using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomStatusIcon : MonoBehaviour
{
    public List<Sprite> images;

    public Image target;
    
    // Start is called before the first frame update
    void Start()
    {
        target.sprite = images[Random.Range(0, images.Count)];
    }

    
}
