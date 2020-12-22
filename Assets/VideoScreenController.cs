using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Video;

public class VideoScreenController : MonoBehaviour
{
    public VideoPlayer videoScreen;
    public GameObject payScreen;
    public GameObject finishScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        videoScreen.gameObject.SetActive(false);
        payScreen.SetActive(false);
        finishScreen.SetActive(false);
        
        switch (LoginState.VideoState)
        {
            case VideoState.VideoOk:
                videoScreen.gameObject.SetActive(true);
                videoScreen.url = LoginState.VideoUrl;
                Debug.Log("[VIDEO] " + LoginState.VideoUrl);
                videoScreen.Play();
                break;
            case VideoState.ShowFinished:
                finishScreen.SetActive(true);
                break;
            case VideoState.NotSet:
                break;
            case VideoState.HasToPay:
                payScreen.SetActive(true);
                break;
        } 
    }

    
}
