using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*controla os eventos do game*/
public class Game : MonoBehaviour {

	public float globalTime = 0f;
	public float moonTime = 0f;
	public float moon2Time = 0f;
	public float gameTime = 0f;
	public float playerTime = 0f;
	public float enemyTime = 0f;
	public int enemyLevel = 0; //maxlevel == 20
	public int gamePhase = 0; //0 - fase de aguardar | 1 - atacar| 2 - verifica vitoria ou derrota 
	public int rand=0;
	public bool enemyAttack = false;
	public bool playerAttack = false;
	public string result = "";
	public Transform moon;
	public Transform moon2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR_WIN
			inputPC();
		#endif
			inputMobile();
		
		//start printer
		globalTime += Time.deltaTime;
		//move moon
		moonTime += Time.deltaTime;
		if(moonTime > 3){
			moon.Translate(Vector3.up * 0.1f);			
			moonTime = 0f;
		}
		moon2Time += Time.deltaTime;
		if(globalTime > 30){
			if(moon2Time > 3){
				moon2.Translate(Vector3.down * 0.1f);			
				moon2Time = 0f;
			}	
		}
		
			
		//start global timer	
		if(gamePhase != 2){
			gameTime += Time.deltaTime;
		}		
		
		//random start for game
		if(gameTime > 3f && gamePhase == 0){
			rand = Random.Range(0, 8);
			if(rand == 7){
				gamePhase = 1;
			}
		}
			
		//if state 1 start timers 
		if(gamePhase != 0){
			//count enemy time of attack
			if( enemyAttack ==false){
				enemyTime  += Time.deltaTime;
			}
			//count player time of attack
			if(playerAttack == false){
				playerTime += Time.deltaTime;
			}
			//random attack of enemy
			if(enemyTime > (1f - (enemyLevel * 0.05f))){
				enemyAttack = true;
				verify();
			}
		}
	}
	
	void verify(){
		if(gamePhase == 0){
			//phase 0 - nobody attacks, or loose
			if(enemyTime == playerTime){
				//loose	
				gamePhase =2;
				result = "LOOSE";
			}
		}
		else if(gamePhase == 1){
			//phase 1 - allow attack!

			
			if(playerAttack == enemyAttack){
				//tie
				Debug.Log("tie");
				result = "TIE";
				gamePhase = 2;
			}
			else if(playerAttack == true && enemyAttack == false){
				//win
				Debug.Log("win");
				result = "WIN";
				gamePhase =2;
			}
			else{
				//loose
				Debug.Log("loose ++++");
				result = "LOOSE";
				gamePhase = 2;
			}
			
			//phase 2 - verify results
			if(gamePhase == 2 ){
				Debug.Log("phase 2 ");
				if(result == "WIN"){
					//prepare for next level
					enemyLevel ++;
					gameTime = 0f;
					result = "";
				}
				else if(result == "TIE"){
					gameTime = 0f;
					result = "";
					
				}else if(result == "LOOSE"){
					//gameOver();
				}	
			}
		}
	}
	
	void inputPC(){		
		if(Input.GetMouseButtonDown(0)){
			playerAttack = true;
			verify();
		}
		
		if(Input.GetMouseButtonDown(1)){
			gameOver();
		}
	}
	
	void inputMobile(){		
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
			playerAttack = true;
			verify();
		}
	}
	
	void gameOver(){
		Debug.Log("game over!");
		Scene scene = SceneManager.GetActiveScene(); 
		SceneManager.LoadScene(scene.name);
		
	}
	
}
