using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
	[SerializeField] string level;

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			FindObjectOfType<SceneLoader>().LoadScene(level);
		}
	}
}
