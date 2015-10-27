using UnityEngine;
using System.Collections;

public class Zombie_Spawn : MonoBehaviour {

	public GameObject zombie_prefab;

	public bool spawn_ready;

	// Use this for initialization
	void Start ()
	{
		spawn_ready = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (spawn_ready)
		{
			/*Instantiate(zombie_prefab, transform.position, transform.rotation);
			
			//Wait 5 sec to spawn another zombie
			spawn_ready = false;
			Invoke("Signal_Spawn_Ready", 5);*/
		}
	}
	
	public void Signal_Spawn_Ready()
	{
		spawn_ready = true;
	}

	public void Spawn_Zombie()
	{
		Instantiate(zombie_prefab, transform.position, transform.rotation);
	}
}
