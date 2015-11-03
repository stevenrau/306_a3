using UnityEngine;
using System.Collections;

/*
 * This script control's the zombie's AI, implemented in the form
 * of a decision tree. Any behaviour/status changes or checks are
 * done in the Zombie_Behaviour script.
 * 
 * REASONS FOR CHOOSING THRESHOLD PARAMETERS:
 * ------------------------------------------------------------------------------------------
 * 
 * !note: The Threat Zone is implemented as a circle collider trigger with a radius of 7.
 *        Since the zombies spawn in the corners of the map, this allows the zombies to
 *        see threats up to the aproximately the center of the map from their spawn points.
 * 
 * RANGE - since my map is 10 units wide, I figured the player should
 *         be in the enemy's range when they are just within spprozimately
 *         half the map's width away
 * 
 * FRIEND_DIST - I chose friend distance to be 2 units because I didn't want
 *               it to be so small that enemies would always have friends "neraby"
 *               and never retreat due to low health.
 * 
 * LOW_HP - Since the zombie's only have 3 hit points, 1 seemed to be the logical
 *          choice as a threshold for indicating low HP
 * 
 * SPAWN_DIST - Again, I chose 2 units for the spawn distance limit to keep the zombies
 *              relatively close to their spawn points when they are not chasing the player.
 * -------------------------------------------------------------------------------------------
 * 
 */

public class Zombie_AI : MonoBehaviour
{
	//AI constants/magic numbers chosen as deescribed above
	private const uint RANGE = 4;
	private const uint FRIEND_DIST = 2;
	private const uint LOW_HP = 1;
	private const uint SPAWN_DIST = 2;

	// The player's transform and behaviour script
	Transform target;
	Player_Control player_control;

	//This zombie's behaviour script
	Zombie_Bahaviour behaviour;

	//Delegate for zombie's action
	delegate void ZombieDelegate();
	ZombieDelegate zombie_action;
	ZombieDelegate decision_tree_root;

	//Boolean to keep track of whether or not player is in threat zone
	bool player_in_threat_zone;

	//Boolean to keep track if the zombie is doing a random walk
	private bool randomly_walking;
	//Vector to keep track of random walk dir
	Vector3 random_walk_dir;

	//Boolean to keep track if the zombie is currently idling
	private bool idling;

	//Position of the zombie's spawn point that gets set when it spawns
	Vector3 spawn_pt;

	// Use this for initialization
	void Start ()
	{
		//Get the player's transform and control script
		target = GameObject.FindGameObjectWithTag("Player").transform;
		player_control = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Control>();

		//Get this zombie's behaviour script
		behaviour = gameObject.GetComponent<Zombie_Bahaviour>();

		player_in_threat_zone = false;

		//First step is to make sure this zombie is not dead
		zombie_action = Check_If_Self_Dead;
		decision_tree_root = Check_If_Self_Dead;

		randomly_walking = false;
		idling = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		zombie_action();
	}

	//Set the zombie's spawn point location. Called by the spawn point handler script
	public void Set_Spawn_Point(Vector3 spawn)
	{
		spawn_pt = spawn;
	}

	/*******************************************************************
	 * Trigger actions
	 *******************************************************************/

	//The circle collider trigger that acts as the zombie's "Threat Zone"
	//Need to handle player entering and exiting the threat zone
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			player_in_threat_zone = true;
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			player_in_threat_zone = false;
		}
	}

	/*******************************************************************
	 * Delegate Functions
	 *******************************************************************/

	//Recursively calls self. Ensures nothingis done from the time the zombie dies 
	//to when it is deleted
	void Wait_For_Deletion()
	{
		zombie_action = Wait_For_Deletion;
	}

	//Checks to see if this zombie object is dead
	void Check_If_Self_Dead()
	{
		if (behaviour.Is_Dead())
		{
			//Perform the zombie's death behaviour
			behaviour.Die();

			//And wait to be deleted
			zombie_action = Wait_For_Deletion;
		}
		else
		{
			zombie_action = Check_If_Player_Dead;
		}
	}

	//Checks to see if the player is dead
	void Check_If_Player_Dead()
	{
		if(player_control.Is_Dead())
		{
			//The player is dead. Stop moving.
			behaviour.Idle(false);

			//Then wait for the game to restart
			zombie_action = Wait_For_Deletion;
		}
		else
		{
			//The player is alive, determine if inside threat zone
			zombie_action = Target_Inside_Threat_Zone;
		}
	}

	//Checks to see if the player is inside the zombie's threat zone.
	void Target_Inside_Threat_Zone()
	{
		if (player_in_threat_zone)
		{
			zombie_action = Player_In_Range;
		}
		else
		{
			zombie_action = Check_Spawn_Distance;
		}
	}

	//Checks to see if the player is in range to attack
	void Player_In_Range()
	{
		if (Vector3.Distance(gameObject.transform.position, target.position) <= RANGE)
		{
			//Check to see if any other zombies are near
			zombie_action = Are_Friends_Near;
		}
		else
		{
			//Check to see if HP is low
			zombie_action = Is_Hp_Low;
		}
	}

	//Checks to see if any other zombies are nearby
	void Are_Friends_Near()
	{
		bool friend_near = false;
		GameObject[] zombies;
		zombies = GameObject.FindGameObjectsWithTag("Zombie");

		//Check all other zombies to see if any are near
		foreach(GameObject cur_zombie in zombies)
		{
			if (cur_zombie == gameObject)
			{
				continue;
			}
			else if (Vector3.Distance(gameObject.transform.position, cur_zombie.transform.position) <= FRIEND_DIST)
			{
				friend_near = true;
				break;
			}
		}

		if (friend_near)
		{
			//Attack the player (move towards them)
			behaviour.Move_Towards(target.position);

			//Go back to decision tree root now that we have reached a leaf
			zombie_action = decision_tree_root;
		}
		else
		{
			zombie_action = Is_Hp_Low;
		}
	}

	//Checks to see if HP is low (one hit point left). If so, retreat
	void Is_Hp_Low()
	{
		//Check if current HP is less than or equal to low hp threshold
		if (behaviour.Get_Hp() <= LOW_HP)
		{
			//Retreat from the player
			behaviour.Retreat(target.position);
		}
		else
		{
			//Attack the player (move towards them)
			behaviour.Move_Towards(target.position);
		}

		//Go back to decision tree root now that we have reached a leaf
		zombie_action = decision_tree_root;
	}

	//Checks the zombie's distance from its spawn point
	void Check_Spawn_Distance()
	{
		//If further away than the max distance from spawn, return to spawn point
		if (Vector3.Distance(gameObject.transform.position, spawn_pt) > SPAWN_DIST && !randomly_walking)
		{
			behaviour.Move_Towards(spawn_pt);
		}
		//Else randomly walk or idle. If already walking, then continue to walk in that dir
		else if ((Random.value > 0.5 || randomly_walking) && !idling)
		{
			//If we weren't already doing a random walk, start a walk in a new random direction
			if (!randomly_walking)
			{
				random_walk_dir = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
				//Walk in this random direction for up to 2 seconds;
				Invoke("End_Random_Walk", 2);
			}

			randomly_walking = true;

			behaviour.Move_Towards(random_walk_dir);
		}
		else
		{
			//Don't Idle if we are currently doing a random walk && don't bother re-calling
			//Idle() if we are already idling
			if(!randomly_walking && !idling)
			{
				//Randomly taunt
				bool taunt = false;
				if (Random.value > 0.5)
				{
					taunt = true;
				}

				behaviour.Idle(taunt);

				idling = true;
				//Idle for 2 seconds
				Invoke("End_Idle", 2);
			}
		}

		//Go back to decision tree root now that we have reached a leaf
		zombie_action = decision_tree_root;
	}

	/*******************************************************************
	 * Invoked functions to reset flags
	 *******************************************************************/

	//Resets the randomly_walking flag, allowing the random walk to change directions
	void End_Random_Walk()
	{
		randomly_walking = false;
	}

	//Resets the idling flag, allowing the zombie to switch "idle" tasks (idle animation or random walk)
	void End_Idle()
	{
		idling = false;
	}
}
