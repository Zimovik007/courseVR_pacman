using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GhostMove : MonoBehaviour {
	private Vector3 waypoint;
	private Queue<Vector3> waypoints;
    
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

	enum State { Wait, Init, Scatter, Chase, Run };
	State state;

    private Vector3 _startPos;
    private float _timeToWhite;
    private float _timeToToggleWhite;
    private float _toggleInterval;
    private bool isWhite = false;
    
	public GameGUINavigation GUINav;
    public PlayerController pacman;
    private GameManager _gm;
    
	void Start()
	{
	    _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _toggleInterval = _gm.scareLength * 0.33f * 0.20f;  
		InitializeGhost();
	}

    public float DISTANCE;

	void FixedUpdate ()
	{
	    DISTANCE = Vector3.Distance(transform.position, waypoint);

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

			case State.Chase:
				ChaseAI();
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
        string data = "";
        switch (name)
        {
        case "blinky":
            data = @"22 20
22 26

27 26
27 30
22 30
22 26";
            break;
        case "pinky":
            data = @"14.5 17
14 17
14 20
7 20

7 26
7 30
2 30
2 26";
            break;
        case "inky":
            data = @"16.5 17
15 17
15 20
22 20

22 8
19 8
19 5
16 5
16 2
27 2
27 5
22 5";
            break;
        case "clyde":
            data = @"12.5 17
14 17
14 20
7 20

7 8
7 5
2 5
2 2
13 2
13 5
10 5
10 8";
            break;
        
        }
        
        string line;

        waypoints = new Queue<Vector3>();
        Vector3 wp;

        if (st == State.Init)
        {
            using (StringReader reader = new StringReader(data))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0) break;

                    string[] values = line.Split(' ');
                    float x = float.Parse(values[0]);
                    float y = float.Parse(values[1]);

                    wp = new Vector3(x, y, 0);
                    waypoints.Enqueue(wp);
                }
            }
        }

        if (st == State.Scatter)
        {
            bool scatterWps = false;

            using (StringReader reader = new StringReader(data))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0)
                    {
                        scatterWps = true;
                        continue;
                    }

                    if (scatterWps)
                    {
                        string[] values = line.Split(' ');
                        int x = Int32.Parse(values[0]);
                        int y = Int32.Parse(values[1]);

                        wp = new Vector3(x, y, 0);
                        waypoints.Enqueue(wp);
                    }
                }
            }
        }
        
        if (st == State.Wait)
        {
            Vector3 pos = transform.position;
            
            if (transform.name == "inky" || transform.name == "clyde")
            {
                waypoints.Enqueue(new Vector3(pos.x, pos.y - 0.5f, 0f));
                waypoints.Enqueue(new Vector3(pos.x, pos.y + 0.5f, 0f));
            }
            else
            {
                waypoints.Enqueue(new Vector3(pos.x, pos.y + 0.5f, 0f));
                waypoints.Enqueue(new Vector3(pos.x, pos.y - 0.5f, 0f));
            }
        }

    }

    private Vector3 getStartPosAccordingToName()
    {
        switch (gameObject.name)
        {
            case "blinky":
                return new Vector3(15f, 20f, 0f);

            case "pinky":
                return new Vector3(14.5f, 17f, 0f);
            
            case "inky":
                return new Vector3(16.5f, 17f, 0f);

            case "clyde":
                return new Vector3(12.5f, 17f, 0f);
        }

        return new Vector3();
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "pacman")
		{
		    if (state == State.Run)
		    {
		        Calm();
		        InitializeGhost(_startPos);
		    }
		       
		    else
		    {
		        _gm.LoseLife();
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
	    _timeToWhite = 0;
        
		if(waypoints.Count == 0)
		{
			state = State.Scatter;
            
			string name = GetComponent<SpriteRenderer>().sprite.name;
			if(name[name.Length-1] == '0' || name[name.Length-1] == '1')	direction = Vector3.right;
			if(name[name.Length-1] == '2' || name[name.Length-1] == '3')	direction = Vector3.left;
			if(name[name.Length-1] == '4' || name[name.Length-1] == '5')	direction = Vector3.up;
			if(name[name.Length-1] == '6' || name[name.Length-1] == '7')	direction = Vector3.down;

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
			state = State.Chase;
		    return;
		}
        
		MoveToWaypoint(true);

	}

    void ChaseAI()
	{
        
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
		{
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}
        
		else GetComponent<AI>().AILogic();

	}

	void RunAway()
	{
        if(Time.time >= _timeToWhite && Time.time >= _timeToToggleWhite)   ToggleBlueWhite();
        
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
		{
			Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
			GetComponent<Rigidbody2D>().MovePosition(p);
		}
		
		else GetComponent<AI>().RunLogic();

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
			if(loop)	waypoints.Enqueue(waypoints.Dequeue());
			else		waypoints.Dequeue();
		}
	}

	public void Calm()
	{
	    if (state != State.Run) return;

		waypoints.Clear ();
		state = State.Chase;
	    _timeToToggleWhite = 0;
	    _timeToWhite = 0;
	}

    public void ToggleBlueWhite()
    {
        isWhite = !isWhite;
        _timeToToggleWhite = Time.time + _toggleInterval;
    }

}
