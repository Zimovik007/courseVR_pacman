using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 0.4f;
    Vector2 _dest = Vector2.zero;
    Vector2 _dir = Vector2.zero;
    Vector2 _nextDir = Vector2.zero;

    private GameGUINavigation GUINav;
    private GameManager GM;

    public Text textState;
    public Text textArmor;
    public Text timer;

    public GameObject player;

    public int time = 0;

    public static System.Random random = new System.Random();
    public int state; 
    public String[] states = new String[] { "yellow", "blue", "pink", "red" };

    private bool _deadPlaying = false;
    
    void Start()
    {
        state = random.Next(0, 4);
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        GUINav = GameObject.Find("UI Manager").GetComponent<GameGUINavigation>();
        textState.text = "state: " + states[state];
        textArmor.text = "Armor: " + GameManager.lives;
        _dest = transform.position;
        StartCoroutine(Countdown());
        GM.setCurState(states[state]);
    }

    IEnumerator Countdown()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            time++;
            timer.text = "" + time;
            GM.incAliveTime();
        }
    }


void FixedUpdate()
    {
        switch (GameManager.gameState)
        {
            case GameManager.GameState.Game:
                ReadInputAndMove();
                break;

            case GameManager.GameState.Dead:
                textArmor.text = "Armor: " + GameManager.lives;
                if (GameManager.lives <= 0)
                {
                    SceneManager.LoadScene("end");
                }
                else
                    GM.ResetScene();
                break;
        }


    }

    bool Valid(Vector2 direction)
    {
        Vector2 pos = transform.position;
        direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
        RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
        return hit.collider.name == "blue" || hit.collider.name == "yellow" || hit.collider.name == "red" || hit.collider.name == "pink" || (hit.collider == GetComponent<Collider2D>());
    }

    public void ResetDestination()
    {
        _dest = new Vector2(15f, 11f);
    }

    void ReadInputAndMove()
    {
        Vector2 p = Vector2.MoveTowards(transform.position, _dest, speed);
        GetComponent<Rigidbody2D>().MovePosition(p);

        if (Input.GetAxis("Horizontal") > 0) _nextDir = Vector2.right;
        if (Input.GetAxis("Horizontal") < 0) _nextDir = -Vector2.right;
        if (Input.GetAxis("Vertical") > 0) _nextDir = Vector2.up;
        if (Input.GetAxis("Vertical") < 0) _nextDir = -Vector2.up;

        if (Vector2.Distance(_dest, transform.position) < 0.00001f)
        {
            if (Valid(_nextDir))
            {
                _dest = (Vector2)transform.position + _nextDir;
                _dir = _nextDir;
            }
            else
            {
                if (Valid(_dir))
                    _dest = (Vector2)transform.position + _dir;
            }
        }
    }

    public Vector2 getDir()
    {
        return _dir;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "yellow")
        {
            state = 0;
            Destroy(other.gameObject);
        }
        else if (other.name == "blue")
        {
            state = 1;
            Destroy(other.gameObject);
        }
        else if (other.name == "pink")
        {
            state = 2;
            Destroy(other.gameObject);
        }
        else if (other.name == "red")
        {
            state = 3;
            Destroy(other.gameObject);
        }
        textState.text = "state: " + states[state];
        GM.setCurState(states[state]);
        if (
            (other.name == "pinky" && states[state] == "pink") ||
            (other.name == "inky" && states[state] == "blue") ||
            (other.name == "blinky" && states[state] == "red") ||
            (other.name == "clyde" && states[state] == "yellow")
           )
        {
            GM.killEnemy();
            Destroy(other.gameObject);
        }
    }
}
