using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	private new Rigidbody rigidbody;

	private float MOVEMENT_SPEED = 2.0f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
		//TODO: Input acceleration

		float inputX = Input.GetAxis("Horizontal") * MOVEMENT_SPEED * Time.deltaTime;
		float inputY = Input.GetAxis("Vertical") * MOVEMENT_SPEED * Time.deltaTime;

		transform.Translate(Quaternion.AngleAxis(45.0f, Vector3.up) * new Vector3(inputX, 0.0f, inputY));
    }
}
