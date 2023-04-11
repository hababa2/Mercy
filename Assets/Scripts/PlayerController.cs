using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	private LayerMask attackMask;

	private float MOVEMENT_SPEED = 2.0f;
	private int damage = 20;

	private CharacterController controller;
	private Vector2 halfScreenSize;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		halfScreenSize = new Vector2(Screen.width, Screen.height) / 2.0f;
		attackMask = LayerMask.GetMask("Enemy");
	}

	private void Update()
	{
		Vector2 mousePos = (Vector2)Input.mousePosition - halfScreenSize;

		transform.eulerAngles = new Vector3(0.0f, Mathf.Rad2Deg * Mathf.Atan2(mousePos.x, mousePos.y) + 45.0f, 0.0f);

		float inputX = Input.GetAxis("Horizontal") * MOVEMENT_SPEED * Time.deltaTime;
		float inputY = Input.GetAxis("Vertical") * MOVEMENT_SPEED * Time.deltaTime;

		controller.Move(Quaternion.AngleAxis(45.0f, Vector3.up) * new Vector3(inputX, 0.0f, inputY));

		if(Input.GetMouseButtonDown(0))
		{
			Attack();
		}
	}

	private void Attack()
	{
		Quaternion rot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);

		Collider[] colliders = Physics.OverlapBox(transform.position + rot * Vector3.forward, new Vector3(0.75f, 0.75f, 0.45f), rot, attackMask.value);

		foreach(Collider collider in colliders)
		{
			collider.gameObject.GetComponent<EnemyController>().Damage(damage);
		}
	}
}
