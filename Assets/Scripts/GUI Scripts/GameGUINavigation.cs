using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class GameGUINavigation : MonoBehaviour {

	public float initialDelay;
	
	public Button MenuButton;
    
	void Start() 
	{
		StartCoroutine("ShowReadyScreen", initialDelay);
	}
	
	void Update() 
	{

	}
    
	public void H_ShowReadyScreen()
	{
		StartCoroutine("ShowReadyScreen", initialDelay);
	}

	IEnumerator ShowReadyScreen(float seconds)
	{
		GameManager.gameState = GameManager.GameState.Init;
		yield return new WaitForSeconds(seconds);
        GameManager.gameState = GameManager.GameState.Game;
	}
}
