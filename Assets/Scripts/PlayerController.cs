using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	private float MOVEMENT_SPEED = 2.0f;

	private CharacterController controller;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		float inputX = Input.GetAxis("Horizontal") * MOVEMENT_SPEED * Time.deltaTime;
		float inputY = Input.GetAxis("Vertical") * MOVEMENT_SPEED * Time.deltaTime;

		controller.Move(Quaternion.AngleAxis(45.0f, Vector3.up) * new Vector3(inputX, 0.0f, inputY));
	}
}
