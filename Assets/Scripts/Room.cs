using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	[SerializeField] private GameObject floorPrefab;
	[SerializeField] private GameObject wallPrefab;
	[SerializeField] private GameObject roofPrefab;

	[HideInInspector] public Vector3 position;
	[HideInInspector] public Vector2Int size;
	[HideInInspector] public sbyte[] doorLocations = new sbyte[4];
	[HideInInspector] public bool[] doorsTaken = new bool[4];
	[HideInInspector] public Room prevRoom;
	[HideInInspector] public int dist;
	[HideInInspector] public byte dir;

	private bool player = false;
	private bool cleared = true;

	private BoxCollider trigger;
	private BoxCollider hallwayTrigger;
	private MeshRenderer roofRenderer;
	private List<GameObject> doors = new List<GameObject>();
	private List<EnemyController> enemies = new List<EnemyController>();

	//TODO: Obscuring view of closed rooms

	public void Setup()
	{
		trigger = GetComponent<BoxCollider>();
		hallwayTrigger = transform.GetChild(0).GetComponent<BoxCollider>();

		trigger.center = position + Vector3.up * 0.5f;
		trigger.size = new Vector3(size.x, 1.0f, size.y);

		GameObject floorInstance = Instantiate(floorPrefab, new Vector3(position.x, 0.0f, position.z), Quaternion.identity, transform);
		floorInstance.transform.localScale = new Vector3(size.x, 1.0f, size.y);

		GameObject roofInstance = Instantiate(roofPrefab, new Vector3(position.x, 1.5f, position.z), Quaternion.identity, transform);
		roofInstance.transform.localScale = new Vector3(size.x + 2.0f, 2.0f, size.y + 2.0f);
		roofRenderer = roofInstance.GetComponent<MeshRenderer>();
		roofRenderer.enabled = false;

		if (doorLocations[0] > 0) //Setup walls with a door
		{
			GameObject instance = Instantiate(wallPrefab, position + new Vector3((doorLocations[0] - size.x - 2.0f) / 2.0f, 0.0f, (size.y + 1) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(doorLocations[0], 2.0f, 1.0f);

			instance = Instantiate(wallPrefab, position + new Vector3((doorLocations[0] + 2.0f) / 2.0f, 0.0f, (size.y + 1.0f) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(size.x - doorLocations[0], 2.0f, 1.0f);

			GameObject door = Instantiate(wallPrefab, position + new Vector3(-size.x / 2.0f + doorLocations[0], 0.5f, (size.y + 1) * 0.5f), Quaternion.identity, transform);
			door.transform.localScale = new Vector3(2.0f, 1.0f, 1.0f);
			door.SetActive(false);
			doors.Add(door);
		}
		else //No Door
		{
			doorLocations[0] = 0;
			GameObject instance = Instantiate(wallPrefab, position + new Vector3(0.0f, 0.0f, (size.y + 1.0f) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(size.x + 2.0f, 2.0f, 1.0f);
		}

		if (doorLocations[1] > 0) //Setup walls with a door
		{
			GameObject instance = Instantiate(wallPrefab, position + new Vector3((doorLocations[1] - size.x - 2.0f) / 2.0f, 0.0f, -(size.y + 1) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(doorLocations[1], 2.0f, 1.0f);

			instance = Instantiate(wallPrefab, position + new Vector3((doorLocations[1] + 2.0f) / 2.0f, 0.0f, -(size.y + 1) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(size.x - doorLocations[1], 2.0f, 1.0f);

			GameObject door = Instantiate(wallPrefab, position + new Vector3(-size.x / 2.0f + doorLocations[1], 0.5f, -(size.y + 1) * 0.5f), Quaternion.identity, transform);
			door.transform.localScale = new Vector3(2.0f, 1.0f, 1.0f);
			door.SetActive(false);
			doors.Add(door);
		}
		else //No Door
		{
			doorLocations[1] = 0;
			GameObject instance = Instantiate(wallPrefab, position + new Vector3(0.0f, 0.0f, -(size.y + 1) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(size.x + 2, 2.0f, 1.0f);
		}

		if (doorLocations[2] > 0) //Setup walls with a door
		{
			if (doorLocations[2] > 1)
			{
				GameObject instance = Instantiate(wallPrefab, position + new Vector3((size.x + 1.0f) * 0.5f, 0.0f, (doorLocations[2] - size.y - 1.0f) / 2.0f), Quaternion.identity, transform);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, doorLocations[2] - 1);
			}

			if (doorLocations[2] < size.y - 1)
			{
				GameObject instance = Instantiate(wallPrefab, position + new Vector3((size.x + 1.0f) * 0.5f, 0.0f, (doorLocations[2] + 1.0f) / 2.0f), Quaternion.identity, transform);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, size.y - doorLocations[2] - 1.0f);
			}

			GameObject door = Instantiate(wallPrefab, position + new Vector3((size.x + 1.0f) * 0.5f, 0.5f, -size.y / 2.0f + doorLocations[2]), Quaternion.identity, transform);
			door.transform.localScale = new Vector3(1.0f, 1.0f, 2.0f);
			door.SetActive(false);
			doors.Add(door);
		}
		else //No Door
		{
			doorLocations[2] = 0;
			GameObject instance = Instantiate(wallPrefab, position + new Vector3((size.x + 1.0f) * 0.5f, 0.0f, 0.0f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(1.0f, 2.0f, size.y);
		}

		if (doorLocations[3] > 0) //Setup walls with a door
		{
			if (doorLocations[3] > 1)
			{
				GameObject instance = Instantiate(wallPrefab, position + new Vector3(-(size.x + 1.0f) * 0.5f, 0.0f, (doorLocations[3] - size.y - 1.0f) / 2.0f), Quaternion.identity, transform);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, doorLocations[3] - 1.0f);
			}

			if (doorLocations[3] < size.y - 1)
			{
				GameObject instance = Instantiate(wallPrefab, position + new Vector3(-(size.x + 1.0f) * 0.5f, 0.0f, (doorLocations[3] + 1.0f) / 2.0f), Quaternion.identity, transform);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, size.y - doorLocations[3] - 1.0f);
			}

			GameObject door = Instantiate(wallPrefab, position + new Vector3(-(size.x + 1.0f) * 0.5f, 0.5f, -size.y / 2.0f + doorLocations[3]), Quaternion.identity, transform);
			door.transform.localScale = new Vector3(1.0f, 1.0f, 2.0f);
			door.SetActive(false);
			doors.Add(door);
		}
		else //No Door
		{
			doorLocations[3] = 0;
			GameObject instance = Instantiate(wallPrefab, position + new Vector3(-(size.x + 1.0f) * 0.5f, 0.0f, 0.0f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(1.0f, 2.0f, size.y);
		}

		if (prevRoom != null)
		{
			switch (dir)
			{
				case 0:
					{
						float x = -prevRoom.size.x / 2.0f + prevRoom.doorLocations[0];
						float z = (prevRoom.size.y + dist) / 2.0f;

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(2.0f, 1.0f, dist);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x + 1.5f, 0.0f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x - 1.5f, 0.0f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						hallwayTrigger.center = prevRoom.position + new Vector3(x, 0.5f, z);
						hallwayTrigger.size = new Vector3(2.0f, 1.0f, dist);
					}
					break;
				case 1:
					{
						float x = -prevRoom.size.x / 2.0f + prevRoom.doorLocations[1];
						float z = -(prevRoom.size.y + dist) / 2.0f;

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(2.0f, 1.0f, dist);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x + 1.5f, 0.0f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x - 1.5f, 0.0f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						hallwayTrigger.center = prevRoom.position + new Vector3(x, 0.5f, z);
						hallwayTrigger.size = new Vector3(2.0f, 1.0f, dist);
					}
					break;
				case 2:
					{
						float x = (prevRoom.size.x + dist) / 2.0f;
						float z = -prevRoom.size.y / 2.0f + prevRoom.doorLocations[2];

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist, 1.0f, 2.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z + 1.5f), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z - 1.5f), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						hallwayTrigger.center = prevRoom.position + new Vector3(x, 0.5f, z);
						hallwayTrigger.size = new Vector3(dist, 1.0f, 2.0f);
					}
					break;
				case 3:
					{
						float x = -(prevRoom.size.x + dist) / 2.0f;
						float z = -prevRoom.size.y / 2.0f + prevRoom.doorLocations[3];

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist, 1.0f, 2.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z + 1.5f), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z - 1.5f), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						hallwayTrigger.center = prevRoom.position + new Vector3(x, 0.5f, z);
						hallwayTrigger.size = new Vector3(dist, 1.0f, 2.0f);
					}
					break;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		switch (other.tag)
		{
			case "Player": { OnPlayerEnter(); } break;
			case "Enemy": { OnEnemyEnter(other.GetComponent<EnemyController>()); } break;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		switch (other.tag)
		{
			case "Player": { OnPlayerExit(); } break;
			case "Enemy": { OnEnemyExit(other.GetComponent<EnemyController>()); } break;
		}
	}

	private void OnPlayerEnter()
	{
		//roofRenderer.enabled = false;

		player = true;
	}

	private void OnPlayerExit()
	{
		//roofRenderer.enabled = true;

		player = false;
	}

	public void OnPlayerExitHallway()
	{
		if(player)
		{
			if (!cleared)
			{
				foreach (GameObject door in doors)
				{
					door.SetActive(true);
				}

				foreach (EnemyController enemy in enemies)
				{
					enemy.enabled = true;
				}
			}
		}
	}

	private void OnEnemyEnter(EnemyController enemy)
	{
		enemies.Add(enemy);
		cleared = false;
	}

	private void OnEnemyExit(EnemyController enemy)
	{
		enemies.Remove(enemy);

		if(enemies.Count == 0)
		{
			cleared = true;

			foreach (GameObject door in doors)
			{
				door.SetActive(false);
			}
		}
	}
}
