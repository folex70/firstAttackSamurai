using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour {


	public Text uiTextBestLevel;
	public Text uiTextBestTime;
	public Text uiTextBestEnemy;
	public Text uiTextMyTime;

	
	// Use this for initialization
	void Start () {
		uiTextBestLevel.text = "Max Level: "+Data.bestLevel;
		uiTextBestTime.text = "Best Time: "+Data.bestTime;
		uiTextBestEnemy.text = "Enemy Time: "+Data.bestEnemy;
		uiTextMyTime.text = "My Time: "+Data.myTime;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void menuStart(){
		SceneManager.LoadScene("Scene1");  
	}
	
	public void menuExit(){
		//sair
		Application.Quit();
	}

}