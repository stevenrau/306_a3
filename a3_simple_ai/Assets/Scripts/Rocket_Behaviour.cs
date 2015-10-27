using UnityEngine;
using System.Collections;

public class Rocket_Behaviour : MonoBehaviour {

	AudioSource audio_source;
	Animator anim;
	Score_Text score_text;

	// Use this for initialization
	void Start ()
	{
		audio_source = gameObject.GetComponent<AudioSource>();
		anim = gameObject.GetComponent<Animator>();

		GameObject score_text_object = GameObject.FindWithTag("Score");
		if (score_text_object != null)
		{
			score_text = score_text_object.GetComponent<Score_Text>();
		}
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
			zombie_script.Die();

			score_text.Increment_Score();

		}
	}

	void Destroy_Rocket()
	{
		Destroy(gameObject);
	}

}
