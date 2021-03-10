using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public InputField login;

    public InputField password;

    public GameObject loginForm;
    public GameObject errorForm;
    public GameObject loading;
    public Text errorMessage;
    
    public bool skipLogin;
    
    // Start is called before the first frame update
    void Start()
    {
        if (skipLogin) SceneManager.LoadScene(1);
        
        if (PlayerPrefs.HasKey("name"))
        {
            login.text = PlayerPrefs.GetString("name");
        }
        errorMessage.text = "";
        loginForm.SetActive(true);
        errorForm.SetActive(false);
        loading.SetActive(false);
    }

    public void Login()
    {
        if (login.text != "" && password.text != "")
        {
            var url = $"https://api.w-hub.ru/wp-json/whub/v3/auth/unity?username={login.text}&password={password.text}";
            Debug.Log(url);
            PlayerPrefs.SetString("name", login.text);
            PlayerPrefs.Save();
            loading.SetActive(true);
            StartCoroutine(LoginRoutine(url));
        }
        
    }
    
    IEnumerator LoginRoutine(string url) {
        UnityWebRequest www = UnityWebRequest.Post(url, "");
        yield return www.Send();
 
        loading.SetActive(false);
        
        if(www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            if (www.responseCode == 200)
            {
                var s = www.downloadHandler.text;
                if (s.Contains("video"))
                {
                    var sSplit = s.Split('"');
                    s = sSplit[3].Replace("\\","");
                    Debug.Log(s);
                    LoginState.VideoUrl = s;
                    LoginState.VideoState = VideoState.VideoOk;
                } else if (s.Contains("\"has_greeting\":false"))
                {
                    LoginState.VideoState = VideoState.HasToPay;
                    Debug.Log("Has to pay");
                } else if (s.Contains("\"show_has_finished\":true"))
                {
                    LoginState.VideoState = VideoState.ShowFinished;
                    Debug.Log("Finished");
                }
                SceneManager.LoadScene(1);
            }
            else
            {
                loginForm.SetActive(false);
                errorForm.SetActive(true);
                errorMessage.text = www.responseCode.ToString();
            }
        }
    }

    public void TryAgain()
    {
        password.text = "";
        Start();
    }

    public void GoToSite()
    {
        Application.OpenURL("http://elka.w-hub.ru/");
    }
}
