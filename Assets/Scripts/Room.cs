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
	[HideInInspector] public sbyte[] doors = new sbyte[4];
	[HideInInspector] public bool[] doorsTaken = new bool[4];
	[HideInInspector] public Room prevRoom;
	[HideInInspector] public int dist;
	[HideInInspector] public byte dir;

	private bool player = false;
	private int enemyCount = 0;
	private bool cleared = false;

	private BoxCollider trigger;
	private MeshRenderer roofRenderer;

	//TODO: Doors
	//TODO: Obscuring view of closed rooms

	public void Setup()
	{
		trigger = GetComponent<BoxCollider>();
		trigger.center = position + Vector3.up * 0.5f;
		trigger.size = new Vector3(size.x, 1.0f, size.y);

		GameObject floorInstance = Instantiate(floorPrefab, new Vector3(position.x, 0.0f, position.z), Quaternion.identity, transform);
		floorInstance.transform.localScale = new Vector3(size.x, 1.0f, size.y);

		GameObject roofInstance = Instantiate(roofPrefab, new Vector3(position.x, 1.5f, position.z), Quaternion.identity, transform);
		roofInstance.transform.localScale = new Vector3(size.x + 2.0f, 2.0f, size.y + 2.0f);
		roofRenderer = roofInstance.GetComponent<MeshRenderer>();

		if (doors[0] > 0) //Setup walls with a door
		{
			GameObject instance = Instantiate(wallPrefab, position + new Vector3((doors[0] - size.x - 2.0f) / 2.0f, 0.0f, (size.y + 1) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(doors[0], 2.0f, 1.0f);

			instance = Instantiate(wallPrefab, position + new Vector3((doors[0] + 2.0f) / 2.0f, 0.0f, (size.y + 1.0f) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(size.x - doors[0], 2.0f, 1.0f);
		}
		else //No Door
		{
			doors[0] = 0;
			GameObject instance = Instantiate(wallPrefab, position + new Vector3(0.0f, 0.0f, (size.y + 1.0f) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(size.x + 2.0f, 2.0f, 1.0f);
		}

		if (doors[1] > 0) //Setup walls with a door
		{
			GameObject instance = Instantiate(wallPrefab, position + new Vector3((doors[1] - size.x - 2.0f) / 2.0f, 0.0f, -(size.y + 1) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(doors[1], 2.0f, 1.0f);

			instance = Instantiate(wallPrefab, position + new Vector3((doors[1] + 2.0f) / 2.0f, 0.0f, -(size.y + 1) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(size.x - doors[1], 2.0f, 1.0f);
		}
		else //No Door
		{
			doors[1] = 0;
			GameObject instance = Instantiate(wallPrefab, position + new Vector3(0.0f, 0.0f, -(size.y + 1) * 0.5f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(size.x + 2, 2.0f, 1.0f);
		}

		if (doors[2] > 0) //Setup walls with a door
		{
			if (doors[2] > 1)
			{
				GameObject instance = Instantiate(wallPrefab, position + new Vector3((size.x + 1.0f) * 0.5f, 0.0f, (doors[2] - size.y - 1.0f) / 2.0f), Quaternion.identity, transform);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, doors[2] - 1);
			}

			if (doors[2] < size.y - 1)
			{
				GameObject instance = Instantiate(wallPrefab, position + new Vector3((size.x + 1.0f) * 0.5f, 0.0f, (doors[2] + 1.0f) / 2.0f), Quaternion.identity, transform);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, size.y - doors[2] - 1.0f);
			}
		}
		else //No Door
		{
			doors[2] = 0;
			GameObject instance = Instantiate(wallPrefab, position + new Vector3((size.x + 1.0f) * 0.5f, 0.0f, 0.0f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(1.0f, 2.0f, size.y);
		}

		if (doors[3] > 0) //Setup walls with a door
		{
			if (doors[3] > 1)
			{
				GameObject instance = Instantiate(wallPrefab, position + new Vector3(-(size.x + 1.0f) * 0.5f, 0.0f, (doors[3] - size.y - 1.0f) / 2.0f), Quaternion.identity, transform);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, doors[3] - 1.0f);
			}

			if (doors[3] < size.y - 1)
			{
				GameObject instance = Instantiate(wallPrefab, position + new Vector3(-(size.x + 1.0f) * 0.5f, 0.0f, (doors[3] + 1.0f) / 2.0f), Quaternion.identity, transform);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, size.y - doors[3] - 1.0f);
			}
		}
		else //No Door
		{
			doors[3] = 0;
			GameObject instance = Instantiate(wallPrefab, position + new Vector3(-(size.x + 1.0f) * 0.5f, 0.0f, 0.0f), Quaternion.identity, transform);
			instance.transform.localScale = new Vector3(1.0f, 2.0f, size.y);
		}

		if (prevRoom != null)
		{
			switch (dir)
			{
				case 0:
					{
						float x = -prevRoom.size.x / 2.0f + prevRoom.doors[0];
						float z = (prevRoom.size.y + dist) / 2.0f;

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(2.0f, 1.0f, dist);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x + 1.5f, 0.0f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x - 1.5f, 0.0f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);
					}
					break;
				case 1:
					{
						float x = -prevRoom.size.x / 2.0f + prevRoom.doors[1];
						float z = -(prevRoom.size.y + dist) / 2.0f;

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(2.0f, 1.0f, dist);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x + 1.5f, 0.0f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x - 1.5f, 0.0f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);
					}
					break;
				case 2:
					{
						float x = (prevRoom.size.x + dist) / 2.0f;
						float z = -prevRoom.size.y / 2.0f + prevRoom.doors[2];

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist, 1.0f, 2.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z + 1.5f), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z - 1.5f), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);
					}
					break;
				case 3:
					{
						float x = -(prevRoom.size.x + dist) / 2.0f;
						float z = -prevRoom.size.y / 2.0f + prevRoom.doors[3];

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist, 1.0f, 2.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z + 1.5f), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z - 1.5f), Quaternion.identity, transform);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);
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
			case "Enemy": { ++enemyCount; } break;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		switch (other.tag)
		{
			case "Player": { OnPlayerExit(); } break;
			case "Enemy": { --enemyCount; } break;
		}
	}

	private void OnPlayerEnter()
	{
		roofRenderer.enabled = false;

		player = true;

		if (!cleared)
		{
			//TODO: Seal doors until cleared
		}
	}

	private void OnPlayerExit()
	{
		roofRenderer.enabled = true;

		player = false;
	}
}
