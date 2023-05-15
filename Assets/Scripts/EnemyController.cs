using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : MonoBehaviour
{
	[SerializeField] private RectTransform healthBar;
	[SerializeField] private Material mat;
	[SerializeField] private Material hurtMat;
	[SerializeField] private bool ranged;
	[SerializeField] private int maxHealth;
	private CharacterController controller;
	private new MeshRenderer renderer;
	private Transform player;
	private LayerMask attackMask;
	private LayerMask sightMask;
	private Quaternion targetRotation;

	private static readonly float MOVEMENT_SPEED = 2.0f;
	private static readonly float ATTACK_COOLDOWN = 1.0f;

	private int health;
	private int damage = 5;
	private float attacktimer = 0.0f;
	private float hitTimer = 0.0f;
	private bool hit = false;

	private bool lineOfSight = false;
	private float distance = float.MaxValue;

	public new bool enabled = false;

	private void Awake()
	{
		player = FindFirstObjectByType<PlayerController>().transform; 
		controller = GetComponent<CharacterController>();
		renderer = GetComponent<MeshRenderer>();
		attackMask = LayerMask.GetMask("Player");
		sightMask = LayerMask.GetMask("Player", "Ground");

		targetRotation = transform.rotation;
		health = maxHealth;
	}

	private void Update()
	{
		attacktimer -= Time.deltaTime;
		hitTimer -= Time.deltaTime;
		if(hitTimer <= 0.0f && hit) { renderer.material = mat; hit = false; }

		if (player != null)
		{
			Vector3 dir = player.position - transform.position;

			if(Physics.Raycast(transform.position, dir, out RaycastHit hit, 10.0f, sightMask.value) &&
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

		if (lineOfSight && enabled)
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
		healthBar.sizeDelta = new Vector2((float)health / maxHealth, 0.2f);

		hitTimer = 0.1f;
		hit = true;
		renderer.material = hurtMat;

		if(health <= 0)
		{
			transform.position = Vector3.one * 1000000.0f;
			Destroy(gameObject, 1.0f);
		}
	}

	public void KnockBack(float amount, Vector3 dir) //TODO: Smoother Knockback
	{
		dir.y = 0.0f;
		controller.Move(dir * amount);
	}

	private void Attack()
	{
		attacktimer = ATTACK_COOLDOWN;
		Quaternion rot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);

		Collider[] colliders = Physics.OverlapBox(transform.position + rot * Vector3.forward * transform.localScale.z, new Vector3(0.75f, 0.75f, 0.45f), rot, attackMask.value);

		foreach (Collider collider in colliders)
		{
			if (collider.gameObject.TryGetComponent(out PlayerController controller))
			{
				controller.Damage(damage);
			}
		}
	}
}
