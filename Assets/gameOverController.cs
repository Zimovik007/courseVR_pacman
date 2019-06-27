using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameOverController : MonoBehaviour
{

    private GameManager GM;

    public Text aliveTime;

    void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        aliveTime.text = "alive time: " + GM.aliveTime;
    }

    void Update()
    {
        
    }
}
