using UnityEngine;
using System.Collections;

public class Score_Text : MonoBehaviour {

	//The kills score
	public GUIText score_text;
	int score;

	// Use this for initialization
	void Start ()
	{
		score_text = gameObject.GetComponent<GUIText>();
		score = 0;
		Update_Score();

	}
	
	public void Increment_Score()
	{
		score++;
		Update_Score();
	}
	
	void Update_Score()
	{
		score_text.text = "Kills: " + score;
	}
}
