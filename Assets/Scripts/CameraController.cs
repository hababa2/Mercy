using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float speed;
	[SerializeField] private float offset;

    private void Update()
    {
        if(target != null)
		{
			Vector3 position = new Vector3(Mathf.Lerp(transform.position.x, target.position.x + offset, speed), 10.0f,
				Mathf.Lerp(transform.position.z, target.position.z + offset, speed));
			transform.position = position;
		}
    }
}
