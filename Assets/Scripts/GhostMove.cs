using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GhostMove : MonoBehaviour {
	private Vector3 waypoint;
	private Queue<Vector3> waypoints;

    public static System.Random random = new System.Random();
    public Vector3 _direction;
	public Vector3 direction 
	{
		get
		{
			return _direction;
		}

		set
		{
			_direction = value;
			Vector3 pos = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
			waypoint = pos + _direction;		
		}
	}

	public float speed = 0.3f;
    
	public float scatterLength = 5f;
	public float waitLength = 0.0f;

	private float timeToEndScatter;
	private float timeToEndWait;

	enum State { Wait, Init, Scatter, Run };
	State state;

    private Vector3 _startPos;
    
	public GameGUINavigation GUINav;
    private GameManager _gm;
    
	void Start()
	{
	    _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
		InitializeGhost();
	}

	void FixedUpdate ()
	{
		if(GameManager.gameState == GameManager.GameState.Game){
			switch(state)
			{
			case State.Wait:
				Wait();
				break;

			case State.Init:
				Init();
				break;

			case State.Scatter:
				Scatter();
				break;

			case State.Run:
				RunAway();
				break;
			}
		}
	}

	public void InitializeGhost()
	{
	    _startPos = getStartPosAccordingToName();
		waypoint = transform.position;
		state = State.Wait;
	    timeToEndWait = Time.time + waitLength + GUINav.initialDelay;
		InitializeWaypoints(state);
	}

    public void InitializeGhost(Vector3 pos)
    {
        transform.position = pos;
        waypoint = transform.position;
        state = State.Wait;
        timeToEndWait = Time.time + waitLength + GUINav.initialDelay;
        InitializeWaypoints(state);
    }
	

    private void InitializeWaypoints(State st)
    {
        float[] data = new float[] { -1, -1 };
        switch (name)
        {
            case "blinky":
                data = new float[] { 22, 20, 22, 26, -1, -1, 27, 26, 27, 30, 22, 30, 22, 26 };
                break;
            case "pinky":
                data = new float[] { 14.5f, 17, 14, 17, 14, 20, 7, 20, -1, -1, 7, 26, 7, 30, 2, 30, 2, 26 };
                break;
            case "inky":
                data = new float[] { 16.5f, 17, 15, 17, 15, 20, 22, 20, -1, -1, 22, 8, 19, 8, 19, 5, 16, 5, 16, 2, 27, 2, 27, 5, 22, 5 };
                break;
            case "clyde":
                data = new float[] { 12.5f, 17, 14, 17, 14, 20, 7, 20, -1, -1, 7, 8, 7, 5, 2, 5, 2, 2, 13, 2, 13, 5, 10, 5, 10, 8 };
                break;        
        }

        waypoints = new Queue<Vector3>();
        Vector3 wp;
        
        if (st == State.Init)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                if (data[i] == -1 && data[i + 1] == -1)
                    continue;
                wp = new Vector3(data[i], data[i + 1], 0);
                waypoints.Enqueue(wp);
            }
        }

        if (st == State.Scatter)
        {
            bool scatterWps = false;

            for (int i = 0; i < data.Length; i += 2)
            {
                if (data[i] == -1 && data[i + 1] == -1)
                {
                    scatterWps = true;
                    continue;
                }
                if (scatterWps)
                {
                    wp = new Vector3(data[i], data[i + 1], 0);
                    waypoints.Enqueue(wp);
                }
            }
        }
        
        if (st == State.Wait)
        {
            Vector3 pos = transform.position;            
            waypoints.Enqueue(new Vector3(pos.x, pos.y - 0.5f, 0f));
            waypoints.Enqueue(new Vector3(pos.x, pos.y + 0.5f, 0f));
        }

    }

    private Vector3 getStartPosAccordingToName()
    {
        int x = random.Next(1, 25);
        int y = random.Next(1, 25);
        return new Vector3(x, y, 0f);
    }

	void OnTriggerEnter2D(Collider2D other)
	{
        if (other.name == "pacman")
        {
            if (
                (gameObject.name == "pinky" && _gm.curState == "pink") ||
                (gameObject.name == "inky" && _gm.curState == "blue") ||
                (gameObject.name == "clyde" && _gm.curState == "yellow") ||
                (gameObject.name == "blinky" && _gm.curState == "red")
               )
            {
                return;
            }
            _gm.LoseLife();
            if (state == State.Run)
		    {
		        Calm();
		        InitializeGhost(_startPos);
		    }

		}
	}
    
	void Wait()
	{
		if(Time.time >= timeToEndWait)
		{
			state = State.Init;
		    waypoints.Clear();
			InitializeWaypoints(state);
		}
        
		MoveToWaypoint(true);
	}

	void Init()
	{        
		if(waypoints.Count == 0)
		{
			state = State.Scatter;

			InitializeWaypoints(state);
			timeToEndScatter = Time.time + scatterLength;

			return;
		}
        
		MoveToWaypoint();
	}

	void Scatter()
	{
		if(Time.time >= timeToEndScatter)
		{
			waypoints.Clear();
			state = State.Run;
		    return;
		}
        
		MoveToWaypoint(true);

	}

	void RunAway()
	{        
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
		{
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}		
		else
            GetComponent<AI>().RunLogic();

	}
    
	void MoveToWaypoint(bool loop = false)
	{
        waypoint = waypoints.Peek();
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
		{									                        
			_direction = Vector3.Normalize(waypoint - transform.position);
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}
		else
		{
			if (loop)
                waypoints.Enqueue(waypoints.Dequeue());
			else
                waypoints.Dequeue();
		}
	}

	public void Calm()
	{
	    if (state != State.Run) return;

		waypoints.Clear ();
		state = State.Run;
	}

}
