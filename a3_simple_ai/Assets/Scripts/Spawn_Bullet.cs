using UnityEngine;
using System.Collections;

public class Spawn_Bullet : MonoBehaviour {

	public GameObject bullet_prefab;

	public GameObject spawned_bullet;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("Fire1"))
		{
			print ("Fire");
			spawned_bullet = Instantiate(bullet_prefab, transform.position, transform.rotation) as GameObject;
			spawned_bullet.transform.eulerAngles = new Vector3(0, 0, -150);
			spawned_bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(750, 0));
		}
	
	}
}
