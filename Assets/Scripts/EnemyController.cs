using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : MonoBehaviour
{
	[SerializeField] private RectTransform healthBar;
	private Transform player;
	private LayerMask attackMask;
	private CharacterController controller;

	private static readonly float MOVEMENT_SPEED = 4.0f;
	private static readonly float ATTACK_COOLDOWN = 1.0f;
	private static readonly int MAX_HEALTH = 100;

	private int health = MAX_HEALTH;
	private int damage = 20;
	private float attacktimer = 0.0f;

	private void Awake()
	{
		player = FindFirstObjectByType<PlayerController>().transform; 
		controller = GetComponent<CharacterController>();
		attackMask = LayerMask.GetMask("Player");
	}

	private void Update()
	{
		if (health <= 0)
		{
			Destroy(gameObject);
		}

		attacktimer -= Time.deltaTime;

		if (player != null)
		{
			Vector3 look = player.position - transform.position;

			transform.eulerAngles = new Vector3(0.0f, Mathf.Rad2Deg * Mathf.Atan2(look.x, look.z), 0.0f);

			//TODO: Check if enemy has line of sight to player
			if (attacktimer <= 0.0f) { Attack(); }
		}
	}

	public void Damage(int damage)
	{
		health -= damage;
		healthBar.sizeDelta = new Vector2((float)health / MAX_HEALTH, 0.2f);
	}

	private void Attack()
	{
		attacktimer = ATTACK_COOLDOWN;
		Quaternion rot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);

		Collider[] colliders = Physics.OverlapBox(transform.position + rot * Vector3.forward, new Vector3(0.75f, 0.75f, 0.45f), rot, attackMask.value);

		foreach (Collider collider in colliders)
		{
			if (collider.gameObject.TryGetComponent(out PlayerController controller))
			{
				controller.Damage(damage);
			}
		}
	}
}
