using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class restart : MonoBehaviour
{
    public AudioClip click;
    AudioSource aud;

    void Start()
    {
        
    }
    public void RestartGame()
    {     
        aud = GetComponent<AudioSource>();
        //Debug.Log(aud.name);
        //aud.Play();

        Invoke("ChangeScene", 0.1f);
    }

    void ChangeScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
