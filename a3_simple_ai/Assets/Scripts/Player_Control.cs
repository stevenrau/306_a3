using UnityEngine;
using System.Collections;

public class Player_Control : MonoBehaviour {

	public float speed;
	public float rotation_speed;
	bool dead;

	AudioSource audio_source;
	Transform trans;
	Vector3 rot;
	float angle;
	Rigidbody2D r_body;

	//The player's feet. Are a child object.
	Animator feet_animator;
	public GameObject feet;

	// Use this for initialization
	void Start ()
	{
		dead = false;
		trans = transform;
		rot = trans.rotation.eulerAngles;
		feet_animator = feet.GetComponent<Animator>();
		r_body = gameObject.GetComponent<Rigidbody2D>();
		audio_source = gameObject.GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update ()
	{
		if (!dead)
		{
			//grab the current angle
			angle = trans.eulerAngles.magnitude * Mathf.Deg2Rad;
			
			//rotate
			if (Input.GetKey (KeyCode.RightArrow)) {
				rot.z -= rotation_speed;
			}
			if (Input.GetKey (KeyCode.LeftArrow)) {
				rot.z += rotation_speed;
			}
			
			//move forward and backward
			if (Input.GetKey (KeyCode.UpArrow))
			{
				r_body.AddForce(new Vector2(Mathf.Cos (angle) * speed, 0));
				r_body.AddForce(new Vector2(0, Mathf.Sin (angle) * speed));
				feet_animator.SetBool("walking", true);
			}
			else if (Input.GetKey (KeyCode.DownArrow))
			{
				r_body.AddForce(new Vector2(-1f * (Mathf.Cos (angle) * speed), 0));
				r_body.AddForce(new Vector2(0, -1f * (Mathf.Sin (angle) * speed)));
				feet_animator.SetBool("walking", true);
			}
			else
			{
				feet_animator.SetBool("walking", false);
			}
			
			//update
			trans.rotation = Quaternion.Euler (rot);
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Zombie")
		{
			dead = true;

			//Play the death sound clip, disable movement, colliders, and sprite renderers for player
			audio_source.PlayOneShot(audio_source.clip);
			gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			gameObject.GetComponent<BoxCollider2D>().enabled = false;
			gameObject.GetComponent<SpriteRenderer>().enabled = false;
			feet.GetComponent<SpriteRenderer>().enabled = false;

			//Wait a few seconds before restarting the level
			Invoke("Restart_Level", 3);
		}
	}

	void Restart_Level()
	{
		Application.LoadLevel(0);
	}
}
