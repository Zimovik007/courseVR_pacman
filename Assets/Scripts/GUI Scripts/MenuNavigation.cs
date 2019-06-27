using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour {
	
	public void Play()
	{
		SceneManager.LoadScene("game");
	}
}
