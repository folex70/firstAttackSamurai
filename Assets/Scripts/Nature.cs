using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nature : MonoBehaviour {

	float timeDropPetals;
	public float timePass;
	public float speed = 3f;

	
	// Use this for initialization
	void Start () {
		//instanciador
		timeDropPetals   = 0.5f;//= Random.Range(1f,3f);
		InvokeRepeating ("DropPetals",timeDropPetals, timeDropPetals);
	}
	
	// Update is called once per frame
	void Update () {
		timePass += Time.deltaTime;
		transform.Translate (Vector2.right* speed* Time.deltaTime);
		if(timePass > 10f){
			speed = speed * (-1);
			timePass = 0;
		}
	}
	
	void DropPetals()
    {
        //Instantiate (obstaculo_prefab);
        GameObject obj = ObjectPoolPetals.current.GetPooledObject();

        if (obj == null) return;
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.SetActive(true);
    }
}
