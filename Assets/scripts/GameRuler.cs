using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using TMPro;
using Unity.VisualScripting;

public class GameRuler : MonoBehaviour
{
    
    private float timer = 0.0f;
    private bool timegoing = true;
    GameObject timertext;
    // Start is called before the first frame update
    void Start()
    {
        
        this.timertext = GameObject.Find("timer");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("player").GetComponent<PlayerController>().gameisgonging)
        {
            timer += Time.deltaTime;
            //Debug.Log("timer" + timer);
            this.timertext.GetComponent<TextMeshProUGUI>().text = Math.Ceiling(timer).ToString() + "s";
        }   
    }

    public void Stoptimer()
    {
        timegoing = false;
    }
}
