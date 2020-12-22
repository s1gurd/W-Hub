using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.Video;

public class VideoScreenController : MonoBehaviour
{
    //public VideoPlayer videoScreen;
    public MediaPlayer videoScreen;
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
                Debug.Log("[VIDEO] " + LoginState.VideoUrl);
                videoScreen.gameObject.SetActive(true);
                videoScreen.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, LoginState.VideoUrl);
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

    public void NoPaymentURL()
    {
        Debug.Log("No Payment button");
        Application.OpenURL("http://elka.w-hub.ru");
    }
    
    public void ShowFinishedURL()
    {
        Application.OpenURL("http://w-hub.ru");
    }

    
}
