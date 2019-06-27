using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public static int lives = 3;

	public enum GameState { Init, Game, Dead }
	public static GameState gameState;

    private GameObject pacman;
    private GameObject blinky;
    private GameObject pinky;
    private GameObject inky;
    private GameObject clyde;
    private GameGUINavigation gui;

	public float scareLength;
	private float _timeToCalm;

    public int aliveTime = 0;

    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if(this != _instance)   
                Destroy(this.gameObject);
        }

        AssignGhosts();
    }

    public void incAliveTime()
    {
        aliveTime++;
    }

	void Start () 
	{
		gameState = GameState.Init;
    }

    void OnLevelWasLoaded()
    {
        lives = 3;
        
        AssignGhosts();
        ResetVariables();
    }

    private void ResetVariables()
    {
        _timeToCalm = 0.0f;
    }
    
	void Update () 
	{
		if(_timeToCalm <= Time.time)
			CalmGhosts();
	}

	public void ResetScene()
	{
        CalmGhosts();

		pacman.transform.position = new Vector3(15f, 11f, 0f);
		blinky.transform.position = new Vector3(15f, 20f, 0f);
		pinky.transform.position = new Vector3(14.5f, 17f, 0f);
		inky.transform.position = new Vector3(16.5f, 17f, 0f);
		clyde.transform.position = new Vector3(12.5f, 17f, 0f);

		pacman.GetComponent<PlayerController>().ResetDestination();
		blinky.GetComponent<GhostMove>().InitializeGhost();
		pinky.GetComponent<GhostMove>().InitializeGhost();
		inky.GetComponent<GhostMove>().InitializeGhost();
		clyde.GetComponent<GhostMove>().InitializeGhost();

        gameState = GameState.Init;  
        gui.H_ShowReadyScreen();
	}

	public void CalmGhosts()
	{
		blinky.GetComponent<GhostMove>().Calm();
		pinky.GetComponent<GhostMove>().Calm();
		inky.GetComponent<GhostMove>().Calm();
		clyde.GetComponent<GhostMove>().Calm();
    }

    void AssignGhosts()
    {
        clyde = GameObject.Find("clyde");
        pinky = GameObject.Find("pinky");
        inky = GameObject.Find("inky");
        blinky = GameObject.Find("blinky");
        pacman = GameObject.Find("pacman");

        gui = GameObject.FindObjectOfType<GameGUINavigation>();
    }

    public void LoseLife()
    {
        lives--;
        gameState = GameState.Dead;
    }

    public static void DestroySelf()
    {
        lives = 3;
        Destroy(GameObject.Find("Game Manager"));
    }
}
