using System.Collections;
using System.Collections.Generic;
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
    
    // Start is called before the first frame update
    void Start()
    {
        errorMessage.text = "";
        loginForm.SetActive(true);
        errorForm.SetActive(false);
        loading.SetActive(false);
    }

    public void Login()
    {
        if (login.text != "" && password.text != "")
        {
            var url = $"https://dev.api.w-hub.ru/wp-json/jwt-auth/v1/token?username={login.text}&password={password.text}";
            Debug.Log(url);
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
