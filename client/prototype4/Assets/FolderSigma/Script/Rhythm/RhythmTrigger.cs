using UnityEngine;
using System.Collections;

public class RhythmTrigger : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log ("OnTriggerEnter2D = " + other.gameObject.name);
	}
}
