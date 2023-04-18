using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

class Room
{
	public Vector3 position;
	public Vector2Int size;
	public sbyte[] doors = new sbyte[4];
}

public class MapGenerator : MonoBehaviour
{
	[SerializeField] private Transform floor;
	[SerializeField] private Transform walls;
	[SerializeField] private GameObject floorPrefab;
	[SerializeField] private GameObject wallPrefab;

	private static readonly byte ROOM_COUNT = 8;

	private Room[] rooms = new Room[ROOM_COUNT];

    private void Start()
    {
		int prevRoom = 0;

		for (int i = 0; i < ROOM_COUNT; ++i)
		{
			GenerateRoom(i, ref prevRoom);
		}
	}

	private void GenerateRoom(int num, ref int prevNum)
	{
		rooms[num] = new Room();

		if (num == 0) //This is the spawn room
		{
			Room spawn = rooms[num];
			spawn.position = Vector3.up * 0.5f;
			spawn.size = new Vector2Int(Random.Range(3, 11), Random.Range(3, 11)) * 2 + Vector2Int.one;

			spawn.doors[0] = (sbyte)Random.Range(1, spawn.size.x - 1);
			spawn.doors[1] = (sbyte)Random.Range(1, spawn.size.x - 1);
			spawn.doors[2] = (sbyte)Random.Range(1, spawn.size.y - 1);
			spawn.doors[3] = (sbyte)Random.Range(1, spawn.size.y - 1);

			GameObject instance = Instantiate(floorPrefab, new Vector3(spawn.position.x, 0.0f, spawn.position.z), Quaternion.identity, floor);
			instance.transform.localScale = new Vector3(spawn.size.x, 1.0f, spawn.size.y);

			SetupWalls(spawn);

			//TODO: Spawn enemies

			prevNum = num;
		}
		else
		{
			Room prevRoom = rooms[prevNum];


		}
	}

	private void SetupWalls(Room room)
	{
		bool hasDoor = false;

		if (room.doors[1] > 0) //Setup walls with a door
		{
			hasDoor = true;

			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3((room.doors[1] - room.size.x - 2.0f) / 2.0f, 0.0f, -(room.size.y + 1) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.doors[1], 2.0f, 1.0f);

			instance = Instantiate(wallPrefab, room.position + new Vector3((room.doors[1] + 2.0f) / 2.0f, 0.0f, -(room.size.y + 1) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.size.x - room.doors[1], 2.0f, 1.0f);
		}
		else //No Door
		{
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3(0.0f, 0.0f, -(room.size.y + 1) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.size.x + 2, 2.0f, 1.0f);
		}

		if (room.doors[2] > 0) //Setup walls with a door
		{
			hasDoor = true;

			if (room.doors[2] > 1)
			{
				GameObject instance = Instantiate(wallPrefab, room.position + new Vector3((room.size.x + 1.0f) * 0.5f, 0.0f, (room.doors[2] - room.size.y - 1.0f) / 2.0f), Quaternion.identity, walls);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, room.doors[2] - 1);
			}

			if (room.doors[2] < room.size.y - 1)
			{
				GameObject instance = Instantiate(wallPrefab, room.position + new Vector3((room.size.x + 1.0f) * 0.5f, 0.0f, (room.doors[2] + 1.0f) / 2.0f), Quaternion.identity, walls);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, room.size.y - room.doors[2] - 1.0f);
			}
		}
		else //No Door
		{
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3((room.size.x + 1.0f) * 0.5f, 0.0f, 0.0f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(1.0f, 2.0f, room.size.y);
		}

		if (room.doors[3] > 0) //Setup walls with a door
		{
			hasDoor = true;

			if (room.doors[3] > 1)
			{
				GameObject instance = Instantiate(wallPrefab, room.position + new Vector3(-(room.size.x + 1.0f) * 0.5f, 0.0f, (room.doors[3] - room.size.y - 1.0f) / 2.0f), Quaternion.identity, walls);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, room.doors[3] - 1.0f);
			}

			if (room.doors[3] < room.size.y - 1)
			{
				GameObject instance = Instantiate(wallPrefab, room.position + new Vector3(-(room.size.x + 1.0f) * 0.5f, 0.0f, (room.doors[3] + 1.0f) / 2.0f), Quaternion.identity, walls);
				instance.transform.localScale = new Vector3(1.0f, 2.0f, room.size.y - room.doors[3] - 1.0f);
			}
		}
		else //No Door
		{
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3(-(room.size.x + 1.0f) * 0.5f, 0.0f, 0.0f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(1.0f, 2.0f, room.size.y);
		}

		if (room.doors[0] < 1 && !hasDoor) { room.doors[0] = (sbyte)Random.Range(1, room.size.x - 1); }

		if (room.doors[0] > 0) //Setup walls with a door
		{
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3((room.doors[0] - room.size.x - 2.0f) / 2.0f, 0.0f, (room.size.y + 1) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.doors[0], 2.0f, 1.0f);

			instance = Instantiate(wallPrefab, room.position + new Vector3((room.doors[0] + 2.0f) / 2.0f, 0.0f, (room.size.y + 1.0f) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.size.x - room.doors[0], 2.0f, 1.0f);
		}
		else //No Door
		{
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3(0.0f, 0.0f, (room.size.y + 1.0f) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.size.x + 2.0f, 2.0f, 1.0f);
		}
	}
}
