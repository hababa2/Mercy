using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private Rigidbody rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void SetVelocity(Vector3 dir, float speed)
	{
		dir.y = 0.0f;
		rb.velocity = dir * speed;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.TryGetComponent(out EnemyController controller))
		{
			controller.Damage(20);
		}

		Destroy(gameObject); //TODO: Maybe bounce?
	}
}
