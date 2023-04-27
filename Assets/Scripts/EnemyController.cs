using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : MonoBehaviour
{
	[SerializeField] private RectTransform healthBar;
	private Transform player;
	private LayerMask attackMask;
	private LayerMask sightMask;
	private CharacterController controller;
	private Quaternion targetRotation;

	private static readonly float MOVEMENT_SPEED = 2.0f;
	private static readonly float ATTACK_COOLDOWN = 1.0f;
	private static readonly int MAX_HEALTH = 100;

	private int health = MAX_HEALTH;
	private int damage = 5;
	private float attacktimer = 0.0f;

	private bool lineOfSight = false;
	private float distance = float.MaxValue;

	private void Awake()
	{
		player = FindFirstObjectByType<PlayerController>().transform; 
		controller = GetComponent<CharacterController>();
		attackMask = LayerMask.GetMask("Player");
		sightMask = LayerMask.GetMask("Player", "Ground");

		targetRotation = transform.rotation;
	}

	private void Update()
	{
		if (health <= 0) { Destroy(gameObject); }

		attacktimer -= Time.deltaTime;

		if (player != null)
		{
			Vector3 dir = player.position - transform.position;

			if(Physics.Raycast(transform.position, dir, out RaycastHit hit,  30.0f, sightMask.value) &&
				hit.collider.tag == "Player")
			{
				targetRotation = Quaternion.Euler(0.0f, Mathf.Rad2Deg * Mathf.Atan2(dir.x, dir.z), 0.0f);
				lineOfSight = true;
				distance = hit.distance;
			}
			else { lineOfSight = false; }
		}
		else { lineOfSight = false; }

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.4f);

		if (lineOfSight)
		{
			Vector3 move = transform.forward;
			move.y = 0.0f;
			controller.Move(move * MOVEMENT_SPEED * Time.deltaTime);

			if (distance <= 1.2f && attacktimer <= 0.0f) { Attack(); }
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
