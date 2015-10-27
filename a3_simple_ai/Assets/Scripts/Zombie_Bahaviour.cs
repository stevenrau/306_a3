using UnityEngine;
using System.Collections;
using System;

public class Zombie_Bahaviour : MonoBehaviour {

	AudioSource audio_source;
	Animator anim;
	Transform target;

	public float speed;
	private bool dead;

	// Use this for initialization
	void Awake ()
	{
		audio_source = gameObject.GetComponent<AudioSource>();
		anim = gameObject.GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;

		dead = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!dead)
		{
			//Since the player can die, we need to cathc errors when the player object no longer exists
			try
			{
				transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);

				//Rotate the zombie to always face the player
				Vector3 dir = target.position - transform.position;
				float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
			catch(Exception e)
			{
				print ("IN catch");
				transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, Time.deltaTime * speed);
			}
		}
	}

	//Called when the zombie should die. Plays death animation and death sound, then deletes the gameObject
	public void Die()
	{
		dead  = true; 

		//Stop the zombie's movement, remove the collider, trigger death animation, and play death sound clip
		gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Dead_Player";
		gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		gameObject.GetComponent<BoxCollider2D>().enabled = false;
		anim.SetTrigger("dead");
		audio_source.PlayOneShot(audio_source.clip);

		//Give the dead zombie a few seconds before destroying
		Destroy(gameObject, 2);
	}
}
