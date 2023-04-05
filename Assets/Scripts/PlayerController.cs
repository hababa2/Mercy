using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	private new Rigidbody2D rigidbody;

	private float MOVEMENT_SPEED = 2.0f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
		float inputX = Input.GetAxis("Horizontal") * MOVEMENT_SPEED * Time.deltaTime;
		float inputY = Input.GetAxis("Vertical") * MOVEMENT_SPEED * Time.deltaTime;

		transform.Translate(new Vector3(inputX, inputY, 0.0f));
    }
}
