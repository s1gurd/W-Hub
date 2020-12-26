using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.Video;

public class VideoScreenController : MonoBehaviour
{
    public VideoPlayer videoScreen;
    public MediaPlayer videoScreenAvPro;
    public GameObject payScreen;
    public GameObject finishScreen;

    public Material AvProMaterial;
    public Material StandardMaterial;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IOS || UNITY_ANDROID
        videoScreenAvPro.enabled = false;
        videoScreen.enabled = true;
        videoScreen.gameObject.GetComponent<Renderer>().material = StandardMaterial;
#else
        videoScreenAvPro.enabled = true;
        videoScreen.enabled = false;
        videoScreen.gameObject.GetComponent<Renderer>().material = AvProMaterial;
#endif
        videoScreenAvPro.gameObject.SetActive(false);
        payScreen.SetActive(false);
        finishScreen.SetActive(false);

        switch (LoginState.VideoState)
        {
            case VideoState.VideoOk:
                Debug.Log("[VIDEO] " + LoginState.VideoUrl);
                videoScreenAvPro.gameObject.SetActive(true);
#if UNITY_IOS || UNITY_ANDROID
                videoScreen.url = LoginState.VideoUrl;
                videoScreen.Play();
#else
                videoScreenAvPro.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, LoginState.VideoUrl);
                videoScreenAvPro.Play();
#endif
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