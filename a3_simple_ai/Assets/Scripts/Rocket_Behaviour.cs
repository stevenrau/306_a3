using UnityEngine;
using System.Collections;

public class Rocket_Behaviour : MonoBehaviour {

	AudioSource audio_source;
	Animator anim;

	// Use this for initialization
	void Start ()
	{
		audio_source = gameObject.GetComponent<AudioSource>();
		anim = gameObject.GetComponent<Animator>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Border" || other.tag == "Zombie")
		{
			//On a collision, stop the rocket and play explosion animation
			gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			gameObject.GetComponent<BoxCollider2D>().enabled = false;
			anim.SetTrigger("explode");

			Invoke("Destroy_Rocket", 1);

			//Play the rocket fire sound clip
			audio_source.PlayOneShot(audio_source.clip);
		}

		if (other.tag == "Zombie")
		{
			//Call the zombie's die function
			Zombie_Bahaviour zombie_script = other.gameObject.GetComponent<Zombie_Bahaviour>();
			zombie_script.Reduce_Hp();
		}
	}

	void Destroy_Rocket()
	{
		Destroy(gameObject);
	}

}
