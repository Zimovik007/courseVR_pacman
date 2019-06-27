using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int aliveEnemies = 4;

    public string curState;

    public static System.Random random = new System.Random();

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

    public void killEnemy()
    {
        aliveEnemies--;
        if (aliveEnemies == 0){
            SceneManager.LoadScene("end");
        }
    }

    public void setCurState(string s)
    {
        curState = s;
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
        AssignGhosts();
        CalmGhosts();

		pacman.transform.position = new Vector3(15f, 11f, 0f);
        int x = random.Next(1, 25);
        int y = random.Next(1, 25);
        if (blinky != null)
            blinky.transform.position = new Vector3(x, y, 0f);
        x = random.Next(1, 25);
        y = random.Next(1, 25);
        if (pinky != null)
            pinky.transform.position = new Vector3(x, y, 0f);
        x = random.Next(1, 25);
        y = random.Next(1, 25);
        if (inky != null)
            inky.transform.position = new Vector3(x, y, 0f);
        x = random.Next(1, 25);
        y = random.Next(1, 25);
        if (clyde != null)
            clyde.transform.position = new Vector3(x, y, 0f);

		pacman.GetComponent<PlayerController>().ResetDestination();

        if (blinky != null)
            blinky.GetComponent<GhostMove>().InitializeGhost();
        if (pinky != null)
            pinky.GetComponent<GhostMove>().InitializeGhost();
        if (inky != null)
            inky.GetComponent<GhostMove>().InitializeGhost();
        if (clyde != null)
            clyde.GetComponent<GhostMove>().InitializeGhost();

        gameState = GameState.Init;  
        gui.H_ShowReadyScreen();
	}

	public void CalmGhosts()
	{
        if (blinky != null)
		    blinky.GetComponent<GhostMove>().Calm();
        if (pinky != null)
            pinky.GetComponent<GhostMove>().Calm();
        if (inky != null)
            inky.GetComponent<GhostMove>().Calm();
        if (clyde != null)
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
