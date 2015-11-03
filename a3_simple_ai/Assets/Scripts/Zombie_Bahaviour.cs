using UnityEngine;
using System.Collections;
using System;

/*
 * This script is used to control the zombie's status
 * and behaviour/movement.
 */

public class Zombie_Bahaviour : MonoBehaviour
{
	//The game score
	Score_Text score_text;

	//The zombie's audio source and animation
	AudioSource audio_source;
	Animator anim;

	//Player status variables (hit points, speed)
	private uint hp;
	public float speed;

	// Use this for initialization
	void Awake ()
	{
		audio_source = gameObject.GetComponent<AudioSource>();
		anim = gameObject.GetComponent<Animator>();

		//Gte the game score text
		GameObject score_text_object = GameObject.FindWithTag("Score");
		if (score_text_object != null)
		{
			score_text = score_text_object.GetComponent<Score_Text>();
		}

		//Set status variables
		hp = 3; //Start with 3 hit points
	}

	//Called when the zombie should die. Plays death animation and death sound, then deletes the gameObject
	public void Die()
	{
		score_text.Increment_Score();

		//Stop the zombie's movement, remove the collider, trigger death animation, and play death sound clip
		gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Dead_Player";
		gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		gameObject.GetComponent<BoxCollider2D>().enabled = false;
		anim.SetTrigger("dead");
		audio_source.PlayOneShot(audio_source.clip);

		//Give the dead zombie a few seconds before destroying
		Destroy(gameObject, 2);
	}

	//Reduces the zombie's hit points by 1. Happens when hit by a rocket.
	public void Reduce_Hp()
	{
		hp -= 1;
	}

	//Checks if this zombie is at 0 hit points.
	public bool Is_Dead()
	{
		return hp == 0;
	}

	//Returns the zombie's hit points
	public uint Get_Hp()
	{
		return hp;
	}

	//Moves the zombie towards the given target
	public void Move_Towards(Vector3 target)
	{
		anim.SetBool("walking", true);
		
		//Rotate the zombie to always face the player
		Vector3 dir = target - transform.position;
		float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
	}

	//Moves the zombie in the opposit diretion of the given target
	public void Retreat(Vector3 target)
	{
		anim.SetBool("walking", true);
		
		//Rotate the zombie to always face the retreat direction
		Vector3 dir = target - transform.position;
		float angle = (Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg) - 180f;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		//get the Vector of distance between zombie and player
		Vector3 distance = target - transform.position;
		//Then we want to retreat in the opposite direction
		Vector3 retreat_vector = transform.position - distance;
		transform.position = Vector3.MoveTowards(transform.position, retreat_vector, Time.deltaTime * speed);
	}

	//Causes the zombie to stop moving and play the idle animation + possible taunt animiation
	public void Idle(bool taunt)
	{
		if(taunt)
		{
			anim.SetTrigger("taunt");
		}
		anim.SetBool("walking", false);
	}
}
