using UnityEngine;
using System.Collections;

public class Player_Controls : MonoBehaviour {

	public float min_x;
	public float min_y;
	public float max_x;
	public float max_y;

	public float speed;
	public float rotation_speed;

	Transform trans;
	Vector3 pos;
	Vector3 rot;
	float angle;

	// Use this for initialization
	void Start () {

		trans = transform;
		pos = trans.position;
		rot = trans.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {

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
		if (Input.GetKey (KeyCode.UpArrow)) {
			pos.x += (Mathf.Cos (angle) * speed) * Time.deltaTime;
			pos.y += (Mathf.Sin (angle) * speed) * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			pos.x -= (Mathf.Cos (angle) * speed) * Time.deltaTime;
			pos.y -= (Mathf.Sin (angle) * speed) * Time.deltaTime;
		}

		//update
		trans.position = pos;
		trans.rotation = Quaternion.Euler (rot);
	}
}
