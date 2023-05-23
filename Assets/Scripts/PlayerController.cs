using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private FillBar healthBar;
	[SerializeField] private FillBar meleeCooldown;
	[SerializeField] private FillBar rangedCooldown;
	[SerializeField] private Material mat;
	[SerializeField] private Material hurtMat;
	[SerializeField] private GameObject projectile;
	[SerializeField] private bool inHub;
	private CharacterController controller;
	private new MeshRenderer renderer;
	private Vector2 halfScreenSize;
	private LayerMask attackMask;
	private Transform eye;
	private Quaternion walkAngle = Quaternion.AngleAxis(45.0f, Vector3.up);

	private static readonly float MOVEMENT_SPEED = 4.0f;
	private static readonly float MELEE_ATTACK_COOLDOWN = 1.0f;
	private static readonly float RANGED_ATTACK_COOLDOWN = 0.25f;
	private static readonly float ATTACK_COOLDOWN = 0.05f;
	private static readonly float IFRAME_TIME = 0.25f;
	private static readonly int MAX_HEALTH = 100;
	private int health = MAX_HEALTH;
	private int damage = 3;
	private float meleeTimer = 0.0f;
	private float rangedTimer = 0.0f;
	private float attackTimer = 0.0f;
	private float IframeTimer = 0.0f;
	private bool invulnerable = false;

	public bool canAttack = true;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		renderer = GetComponent<MeshRenderer>();
		halfScreenSize = new Vector2(Screen.width, Screen.height) / 2.0f;
		attackMask = LayerMask.GetMask("Enemy");
		eye = transform.GetChild(0);
	}

	private void Update()
	{
		if (health <= 0) { Destroy(gameObject); }

		Vector2 mousePos = (Vector2)Input.mousePosition - halfScreenSize;

		//TODO: Because the camera is at an angle to the world, the player doesn't face the mouse perfectly
		transform.eulerAngles = new Vector3(0.0f, Mathf.Rad2Deg * Mathf.Atan2(mousePos.x * 0.75f, mousePos.y) + 45.0f, 0.0f);

		float inputX = Input.GetAxis("Horizontal") * MOVEMENT_SPEED * Time.deltaTime;
		float inputY = Input.GetAxis("Vertical") * MOVEMENT_SPEED * Time.deltaTime;

		controller.Move(walkAngle * new Vector3(inputX, 0.0f, inputY));

		Vector3 positon = transform.position;
		positon.y = 1.5f;
		transform.position = positon;

		if (!inHub)
		{
			meleeTimer -= Time.deltaTime;
			rangedTimer -= Time.deltaTime;
			attackTimer -= Time.deltaTime;
			IframeTimer -= Time.deltaTime;
			if (IframeTimer <= 0.0f && invulnerable) { renderer.material = mat; invulnerable = false; }

			if (rangedCooldown != null) { rangedCooldown.SetPercent(1.0f - Mathf.Max(rangedTimer, 0.0f) / RANGED_ATTACK_COOLDOWN); }
			if (meleeCooldown != null) { meleeCooldown.SetPercent(1.0f - Mathf.Max(meleeTimer, 0.0f) / MELEE_ATTACK_COOLDOWN); }

			if (attackTimer <= 0 && canAttack)
			{
				if (Input.GetMouseButtonDown(1) && meleeTimer <= 0) { MeleeAttack(); }
				else if (Input.GetMouseButton(0) && rangedTimer <= 0) { RangedAttack(); }
			}
		}
	}

	private void MeleeAttack()
	{
		attackTimer = ATTACK_COOLDOWN;
		meleeTimer = MELEE_ATTACK_COOLDOWN;
		meleeCooldown.SetPercent(1.0f - meleeTimer / MELEE_ATTACK_COOLDOWN);
		Quaternion rot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);

		Collider[] colliders = Physics.OverlapBox(transform.position + rot * Vector3.forward, new Vector3(0.75f, 0.75f, 0.45f), rot, attackMask.value);

		foreach (Collider collider in colliders)
		{
			if (collider.gameObject.TryGetComponent(out EnemyController controller))
			{
				controller.KnockBack(1.0f, (controller.transform.position - transform.position).normalized);
				controller.Damage(damage);
			}
		}
	}

	private void RangedAttack()
	{
		attackTimer = ATTACK_COOLDOWN;
		rangedTimer = RANGED_ATTACK_COOLDOWN;
		rangedCooldown.SetPercent(1.0f - rangedTimer / RANGED_ATTACK_COOLDOWN);

		GameObject proj = Instantiate(projectile, eye.position, Quaternion.identity);
		Projectile p = proj.GetComponent<Projectile>();
		p.SetVelocity(Quaternion.AngleAxis(0.0f, transform.up) * transform.forward, 5.0f);
	}

	public void Damage(int damage)
	{
		if (IframeTimer <= 0.0f)
		{
			health -= damage;
			healthBar.SetPercent((float)health / MAX_HEALTH);
			IframeTimer = IFRAME_TIME;
			renderer.material = hurtMat;
			invulnerable = true;
		}
	}
}
