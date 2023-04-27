using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private RectTransform healthBar;
	private CharacterController controller;
	private Vector2 halfScreenSize;
	private LayerMask attackMask;

	private static readonly float MOVEMENT_SPEED = 4.0f;
	private static readonly float ATTACK_COOLDOWN = 0.5f;
	private static readonly float IFRAME_TIME = 0.25f;
	private static readonly int MAX_HEALTH = 100;
	private int health = MAX_HEALTH;
	private int damage = 20;
	private float attackTimer = 0.0f;
	private float IframeTimer = 0.0f; //TODO: Hit/Iframe Effect

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		halfScreenSize = new Vector2(Screen.width, Screen.height) / 2.0f;
		attackMask = LayerMask.GetMask("Enemy");
	}

	private void Update()
	{
		if (health <= 0) { Destroy(gameObject); }

		Vector2 mousePos = (Vector2)Input.mousePosition - halfScreenSize;

		transform.eulerAngles = new Vector3(0.0f, Mathf.Rad2Deg * Mathf.Atan2(mousePos.x, mousePos.y) + 45.0f, 0.0f);

		float inputX = Input.GetAxis("Horizontal") * MOVEMENT_SPEED * Time.deltaTime;
		float inputY = Input.GetAxis("Vertical") * MOVEMENT_SPEED * Time.deltaTime;

		controller.Move(Quaternion.AngleAxis(45.0f, Vector3.up) * new Vector3(inputX, 0.0f, inputY));

		attackTimer -= Time.deltaTime;
		IframeTimer -= Time.deltaTime;

		if(Input.GetMouseButton(0) && attackTimer <= 0) { Attack(); }
	}

	private void Attack()
	{
		attackTimer = ATTACK_COOLDOWN;
		Quaternion rot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);

		Collider[] colliders = Physics.OverlapBox(transform.position + rot * Vector3.forward, new Vector3(0.75f, 0.75f, 0.45f), rot, attackMask.value);

		foreach(Collider collider in colliders)
		{
			if(collider.gameObject.TryGetComponent(out EnemyController controller))
			{
				controller.Damage(damage);
			}
		}
	}

	public void Damage(int damage)
	{
		if (IframeTimer <= 0.0f)
		{
			health -= damage;
			healthBar.sizeDelta = new Vector2((float)health / MAX_HEALTH * 500.0f, 60.0f);
			IframeTimer = IFRAME_TIME;
		}
	}
}
