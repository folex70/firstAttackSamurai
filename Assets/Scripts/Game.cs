using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*controla os eventos do game*/
[RequireComponent(typeof(AudioSource))]
public class Game : MonoBehaviour {
	//---------
	public float globalTime = 0f;
	public float moonTime = 0f;
	public float moon2Time = 0f;
	public float gameTime = 0f;
	public float playerTime = 0f;
	public float enemyTime = 0f;
	public float blinkTime = 0f;
	public int enemyLevel = 0; //maxlevel == 20
	public int gamePhase = 0; //0 - fase de aguardar | 1 - atacar| 2 - verifica vitoria ou derrota 
	public int rand=0;
	public bool enemyAttack = false;
	public bool playerAttack = false;
	public string result = "";
	public Transform moon;
	public Transform moon2;
	public Transform [] cloud;
	//---------
	//ui
	public Text uiText;
	public Text uiLevel;
	//---------
	//sounds
	AudioSource audio;
	public AudioClip  batida;
	public AudioClip  phase1Sound;
	public AudioClip  tie;
	public AudioClip  damage;
	public AudioClip  endGame;
	public AudioClip  tieSound;
	//---------	
	//animations
	public Animator animPlayer;
	public Animator animEnemy;
	//---------
	//prefab
	public GameObject prefabEnemy;
	//---------
	 void Awake() {
       // DontDestroyOnLoad();
    }
	//---------
	// Use this for initialization
	void Start () {
		GameObject player = GameObject.Find("Player");
		GameObject enemy = GameObject.Find("Enemy");
		animPlayer = player.GetComponent<Animator>();
		animEnemy = enemy.GetComponent<Animator>();
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		//UNITY_EDITOR_WIN
		#if UNITY_ANDROID 
			inputMobile();
		#endif
			inputPC();
		
		//Level count
		uiLevel.text="Level "+enemyLevel;		
		//start printer
		globalTime += Time.deltaTime;
		//move moon
		moonTime += Time.deltaTime;
		if(moonTime > 3){
			moon.Translate(Vector3.up * 0.15f);			
			moonTime = 0f;
		}
		moon2Time += Time.deltaTime;
		if(globalTime > 30){
			if(moon2Time > 3){
				moon2.Translate(Vector3.down * 0.1f);			
				moon2Time = 0f;
			}	
		}	
		//move cloud
		cloud[0].Translate(Vector3.right * 0.001f);		
		cloud[1].Translate(Vector3.right * 0.001f);		
		cloud[2].Translate(Vector3.right * 0.001f);		
		cloud[3].Translate(Vector3.right * 0.001f);		
		cloud[4].Translate(Vector3.right * 0.001f);		
		cloud[5].Translate(Vector3.right * 0.001f);				
		//start global timer	
		if(gamePhase != 2){
			gameTime += Time.deltaTime;
		}				
		//random start for game
		if(gameTime > 3f && gamePhase == 0){
			rand = Random.Range(0, 8);
			if(rand == 7){
				gamePhase = 1;
				uiText.text="ATTACK!";
				audio.PlayOneShot(phase1Sound, 0.7F);
			}
		}
		//trying blink effect
		if(gamePhase == 1 ){
			uiText.text="ATTACK!"; 
			blinkTime += Time.deltaTime;
			if(blinkTime >0.1){
				blinkTime = 0;
				uiText.text=""; 
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
			uiText.text="ATTACK!";
						
			if(playerAttack == enemyAttack){
				//tie
				Debug.Log("tie");
				result = "TIE";
				uiText.text="TIE";
				gamePhase = 2;
			}
			else if(playerAttack == true && enemyAttack == false){
				//win
				Debug.Log("win");
				result = "WIN";
				uiText.text="WIN";
				gamePhase =2;
			}
			else{
				//loose
				Debug.Log("loose ++++");
				result = "LOOSE";
				uiText.text="LOOSE";
				gamePhase = 2;
			}
		}
		//phase 2 - verify results
		else if(gamePhase == 2 ){
			Debug.Log("phase 2 ");
			if(result == "WIN"){
				//prepare for next level
				if (enemyLevel > 0){
					GameObject enemy = GameObject.Find("Enemy(Clone)");
					animEnemy = enemy.GetComponent<Animator>();
				}else{
					GameObject enemy = GameObject.Find("Enemy");
					animEnemy = enemy.GetComponent<Animator>();
				}
				animEnemy.Play("enemy1_die");
				Debug.Log("WIN22222222222");
				StartCoroutine(nextLevel(3));
			}
			else if(result == "TIE"){
				audio.PlayOneShot(tieSound, 0.7F);
				gameTime = 0f;
				result = "";
				//animPlayer.Play("attack");
				animEnemy.Play("enemy1_attack");
				if (enemyLevel > 0){
					GameObject enemy = GameObject.Find("Enemy(Clone)");
					animEnemy = enemy.GetComponent<Animator>();
					enemy.transform.position = new Vector3(0, 0, 0);
				}else{
					GameObject enemy = GameObject.Find("Enemy");
					animEnemy = enemy.GetComponent<Animator>();
					enemy.transform.position = new Vector3(0, 0, 0);
				}
			}else if(result == "LOOSE"){
				audio.PlayOneShot(endGame, 0.7F);
				if (enemyLevel > 0){
					GameObject enemy = GameObject.Find("Enemy(Clone)");
					animEnemy = enemy.GetComponent<Animator>();
					enemy.transform.position = new Vector3(-8, 0, 0);
				}else{
					GameObject enemy = GameObject.Find("Enemy");
					animEnemy = enemy.GetComponent<Animator>();
					enemy.transform.position = new Vector3(-8, 0, 0);
				}
				audio.PlayOneShot(damage, 0.7F);
				animPlayer.Play("player_die");
				animEnemy.Play("enemy1_attack");
				StartCoroutine(gameOver(4));
				Debug.Log("caiu aquio no loose");
			}else if(result == "FOUL"){
				uiText.text="FOUL";
				Debug.Log("foul22222222");
				
				StartCoroutine(gameOver(2));
			}	
			gamePhase = 3;
		}
	}
	
	void inputPC(){		
		if(Input.GetMouseButtonDown(0)){
			if(gamePhase == 0){
				gamePhase = 2;
				audio.PlayOneShot(damage, 0.7F);
				//animPlayer.Play("player_attack");
				animPlayer.SetTrigger("attack");
				result = "FOUL";
				Debug.Log("foul");
				verify();				
			}
			if(gamePhase == 1){
				playerAttack = true;
				audio.PlayOneShot(damage, 0.7F);
				animPlayer.Play("player_attack");
				//animPlayer.SetTrigger("attack");
				GameObject player = GameObject.Find("Player");
				player.transform.position = new Vector3(8, 0, 0);
				verify();	
			}
		}
		
		if(Input.GetMouseButtonDown(1)){
			StartCoroutine(gameOver(0));
		}
	}
	
	void inputMobile(){		
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
			if(gamePhase == 0){
				gamePhase = 2;
				result = "FOUL";
				Debug.Log("foul");
				verify();				
			}
			if(gamePhase == 1){
				playerAttack = true;
				audio.PlayOneShot(damage, 0.7F);
				animPlayer.Play("player_attack");
				animPlayer.SetTrigger("attack");
				GameObject player = GameObject.Find("Player");
				player.transform.position = new Vector3(8, 0, 0);
				verify();	
			}
		}
	}
	
	//IEnumerator 
	IEnumerator gameOver(int val){
		Debug.Log("game over!");
		yield return new WaitForSeconds(val);
		//Scene scene = SceneManager.GetActiveScene(); 
		//SceneManager.LoadScene(scene.name);	
		if(Data.bestLevel < enemyLevel){
			Data.bestLevel = enemyLevel;
		}
		Data.bestEnemy = enemyTime;
		Data.myTime = playerTime;
		SceneManager.LoadScene("Scene0");  
	}
		
    IEnumerator nextLevel(int val)
    {
		//prepare game for new level
        print(Time.time);
        yield return new WaitForSeconds(val);
		if(result != "FOUL"){
			if(playerTime < Data.bestTime && playerTime > 0){
				Data.bestTime = playerTime;
			}	
		}	
		Debug.Log("level up");
		Destroy(GameObject.Find("Enemy"));
		Destroy(GameObject.Find("Enemy(Clone)"));
		enemyLevel ++;
		gameTime = 0f;
		result = "";
		gamePhase = 0;
		playerTime = 0f;
		enemyTime = 0f;
		gameTime = -1f;
		enemyAttack = false;
		playerAttack = false;
		uiText.text="HOLD";
		Instantiate(prefabEnemy, new Vector3(8.0F, 0, 0), Quaternion.identity);
		GameObject player = GameObject.Find("Player");
		player.transform.position = new Vector3(-6, 0, 0);
        print(Time.time);
    }	
		
}