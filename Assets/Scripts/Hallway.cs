using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway : MonoBehaviour
{
	private Room parent;

	private void Awake()
	{
		parent = GetComponentInParent<Room>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") { parent.OnPlayerEnterHallway(); }
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") { parent.OnPlayerExitHallway(); }
	}
}
