using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
	[SerializeField] private RectTransform healthBar;
	private Transform player;

	public static readonly int MAX_HEALTH = 100;

	private int health = MAX_HEALTH;

	private void Awake()
	{
		player = FindFirstObjectByType<PlayerController>().transform;
	}

	private void Update()
	{
		Vector3 look = player.position - transform.position;

		transform.eulerAngles = new Vector3(0.0f, Mathf.Rad2Deg * Mathf.Atan2(look.x, look.z), 0.0f);

		if (health <= 0)
		{
			Destroy(gameObject);
		}
	}

	public void Damage(int damage)
	{
		health -= damage;
		healthBar.sizeDelta = new Vector2((float)health / MAX_HEALTH, 0.2f);
	}

	private void Attack()
	{

	}
}
