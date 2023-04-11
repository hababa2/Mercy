using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
	[SerializeField] private RectTransform healthBar;

	public static readonly int MAX_HEALTH = 100;

	private int health = MAX_HEALTH;

	private void Awake()
	{

	}

	private void Update()
	{
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
