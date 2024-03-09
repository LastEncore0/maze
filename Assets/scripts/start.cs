using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{
    //public AudioClip click;
    //AudioSource aud;
    private GameObject player;
    private GameObject startbutton;

    void Start()
    {

    }
    public void StartGame()
    {
        //aud = GetComponent<AudioSource>();
        //Debug.Log(aud.name);
        //aud.Play();
        player = GameObject.Find("player");
        player.GetComponent<PlayerController>().gameisgonging = true;
        startbutton = GameObject.Find("start_button");
        startbutton.SetActive(false);
    }
}