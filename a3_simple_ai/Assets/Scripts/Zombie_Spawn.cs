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
	
	public void Signal_Spawn_Ready()
	{
		spawn_ready = true;
	}

	public void Spawn_Zombie()
	{
		GameObject zombie = (GameObject)Instantiate(zombie_prefab, transform.position, transform.rotation);

		zombie.GetComponent<Zombie_AI>().Set_Spawn_Point(gameObject.transform.position);
	}
}
