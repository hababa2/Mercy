using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

class Room
{
	public Vector3 position;
	public Vector2Int size;
	public sbyte[] doors = new sbyte[4];
	public bool[] doorsTaken = new bool[4];
	public Room prevRoom;
	public int dist;
	public byte dir;
}

public class MapGenerator : MonoBehaviour
{
	[SerializeField] private Transform floor;
	[SerializeField] private Transform walls;
	[SerializeField] private GameObject floorPrefab;
	[SerializeField] private GameObject wallPrefab;

	private static readonly byte ROOM_COUNT = 8;
	private byte roomCount;

	private Room[] rooms;

	private void Start()
	{
		int prevRoom;
		bool success;

		do
		{
			roomCount = 1;
			prevRoom = 0;
			success = true;
			rooms = new Room[ROOM_COUNT];

			for (int i = 0; i < ROOM_COUNT && prevRoom != ROOM_COUNT;)
			{
				rooms[i] = new Room();
				if (GenerateRoom(i, ref prevRoom)) { ++i; }
				if (prevRoom >= i)
				{
					success = false;
					break;
				}
			}
		} while (!success);

		for (int i = 0; i < ROOM_COUNT; ++i)
		{
			SetupWalls(rooms[i]);
		}
	}

	private bool GenerateRoom(int num, ref int prevNum)
	{
		if (num == 0) //This is the spawn room
		{
			Room spawn = rooms[num];
			spawn.position = Vector3.up * 0.5f;
			spawn.size = new Vector2Int(Random.Range(3, 11), Random.Range(3, 11)) * 2 + Vector2Int.one;

			spawn.doors[0] = (sbyte)Random.Range(-spawn.size.x, spawn.size.x - 1);
			if (spawn.doors[0] > 0) { ++roomCount; }
			spawn.doors[1] = (sbyte)Random.Range(-spawn.size.x, spawn.size.x - 1);
			if (spawn.doors[1] > 0) { ++roomCount; }
			spawn.doors[3] = (sbyte)Random.Range(-spawn.size.y, spawn.size.y - 1);
			if (spawn.doors[3] > 0) { ++roomCount; }

			spawn.doors[2] = (sbyte)Random.Range(1, spawn.size.y - 1);

			prevNum = num;

			return true;
		}
		else
		{
			Room room = rooms[num];
			room.prevRoom = rooms[prevNum];

			room.size = new Vector2Int(Random.Range(3, 11), Random.Range(3, 11)) * 2 + Vector2Int.one;

			byte doorIndex = 4;

			while (doorIndex == 4)
			{
				for (byte i = 0; i < 4; ++i)
				{
					if (room.prevRoom.doors[i] > 0 && room.prevRoom.doorsTaken[i] == false)
					{
						doorIndex = i;
						room.prevRoom.doorsTaken[i] = true;

						break;
					}
				}

				if (doorIndex == 4)
				{
					if(++prevNum == num) { return false; }
					else { room.prevRoom = rooms[prevNum]; }
				}
			}

			room.position = room.prevRoom.position;

			switch (doorIndex)
			{
				case 0: //NORTH
					{
						room.doors[1] = (sbyte)Random.Range(1, room.size.x - 1);
						room.dist = Random.Range(3, 10);
						room.dir = 0;
						room.position.z += (room.size.y + room.prevRoom.size.y) / 2.0f + room.dist;
						room.position.x += (room.size.x - room.prevRoom.size.x) / 2.0f - (room.doors[1] - room.prevRoom.doors[0]);
						room.doorsTaken[1] = true;

						if (CollisionCheck(room, 0)) { room.prevRoom.doors[0] = 0; return false; }
					}
					break;
				case 1: //SOUTH
					{
						room.doors[0] = (sbyte)Random.Range(1, room.size.x - 1);
						room.dist = Random.Range(3, 10);
						room.dir = 1;
						room.position.z -= (room.size.y + room.prevRoom.size.y) / 2.0f + room.dist;
						room.position.x += (room.size.x - room.prevRoom.size.x) / 2.0f - (room.doors[0] - room.prevRoom.doors[1]);
						room.doorsTaken[0] = true;

						if (CollisionCheck(room, 1)) { room.prevRoom.doors[1] = 0; return false; }
					}
					break;
				case 2: //EAST
					{
						room.doors[3] = (sbyte)Random.Range(1, room.size.y - 1);
						room.dist = Random.Range(3, 10);
						room.dir = 2;
						room.position.x += (room.size.x + room.prevRoom.size.x) / 2.0f + room.dist;
						room.position.z += (room.size.y - room.prevRoom.size.y) / 2.0f - (room.doors[3] - room.prevRoom.doors[2]);
						room.doorsTaken[3] = true;

						if (CollisionCheck(room, 2)) { room.prevRoom.doors[2] = 0; return false; }
					}
					break;
				case 3: //WEST
					{
						room.doors[2] = (sbyte)Random.Range(1, room.size.y - 1);
						room.dist = Random.Range(3, 10);
						room.dir = 3;
						room.position.x -= (room.size.x + room.prevRoom.size.x) / 2.0f + room.dist;
						room.position.z += (room.size.y - room.prevRoom.size.y) / 2.0f - (room.doors[2] - room.prevRoom.doors[3]);
						room.doorsTaken[2] = true;

						if (CollisionCheck(room, 3)) { room.prevRoom.doors[3] = 0; return false; }
					}
					break;
			}

			if (roomCount < ROOM_COUNT - 1 && room.doors[0] < 1) { room.doors[0] = (sbyte)Random.Range(-room.size.x, room.size.x - 1); if (room.doors[0] > 0) { ++roomCount; } }
			if (roomCount < ROOM_COUNT - 1 && room.doors[1] < 1) { room.doors[1] = (sbyte)Random.Range(-room.size.x, room.size.x - 1); if (room.doors[1] > 0) { ++roomCount; } }
			if (roomCount < ROOM_COUNT - 1 && room.doors[2] < 1) { room.doors[2] = (sbyte)Random.Range(-room.size.y, room.size.y - 1); if (room.doors[2] > 0) { ++roomCount; } }
			if (roomCount < ROOM_COUNT - 1 && room.doors[3] < 1) { room.doors[3] = (sbyte)Random.Range(-room.size.y, room.size.y - 1); if (room.doors[3] > 0) { ++roomCount; } }

			return true;
		}
	}

	private void SetupWalls(Room room)
	{
		GameObject floorInstance = Instantiate(floorPrefab, new Vector3(room.position.x, 0.0f, room.position.z), Quaternion.identity, floor);
		floorInstance.transform.localScale = new Vector3(room.size.x, 1.0f, room.size.y);

		if (room.doors[0] > 0) //Setup walls with a door
		{
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3((room.doors[0] - room.size.x - 2.0f) / 2.0f, 0.0f, (room.size.y + 1) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.doors[0], 2.0f, 1.0f);

			instance = Instantiate(wallPrefab, room.position + new Vector3((room.doors[0] + 2.0f) / 2.0f, 0.0f, (room.size.y + 1.0f) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.size.x - room.doors[0], 2.0f, 1.0f);
		}
		else //No Door
		{
			room.doors[0] = 0;
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3(0.0f, 0.0f, (room.size.y + 1.0f) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.size.x + 2.0f, 2.0f, 1.0f);
		}

		if (room.doors[1] > 0) //Setup walls with a door
		{
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3((room.doors[1] - room.size.x - 2.0f) / 2.0f, 0.0f, -(room.size.y + 1) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.doors[1], 2.0f, 1.0f);

			instance = Instantiate(wallPrefab, room.position + new Vector3((room.doors[1] + 2.0f) / 2.0f, 0.0f, -(room.size.y + 1) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.size.x - room.doors[1], 2.0f, 1.0f);
		}
		else //No Door
		{
			room.doors[1] = 0;
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3(0.0f, 0.0f, -(room.size.y + 1) * 0.5f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(room.size.x + 2, 2.0f, 1.0f);
		}

		if (room.doors[2] > 0) //Setup walls with a door
		{
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
			room.doors[2] = 0;
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3((room.size.x + 1.0f) * 0.5f, 0.0f, 0.0f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(1.0f, 2.0f, room.size.y);
		}

		if (room.doors[3] > 0) //Setup walls with a door
		{
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
			room.doors[3] = 0;
			GameObject instance = Instantiate(wallPrefab, room.position + new Vector3(-(room.size.x + 1.0f) * 0.5f, 0.0f, 0.0f), Quaternion.identity, walls);
			instance.transform.localScale = new Vector3(1.0f, 2.0f, room.size.y);
		}

		if (room.prevRoom != null)
		{
			switch (room.dir)
			{
				case 0:
					{
						float x = -room.prevRoom.size.x / 2.0f + room.prevRoom.doors[0];
						float z = (room.prevRoom.size.y + room.dist) / 2.0f;

						GameObject hInstance = Instantiate(floorPrefab, room.prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(2.0f, 1.0f, room.dist);

						hInstance = Instantiate(wallPrefab, room.prevRoom.position + new Vector3(x + 1.5f, 0.0f, z), Quaternion.identity, walls);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, room.dist - 2);

						hInstance = Instantiate(wallPrefab, room.prevRoom.position + new Vector3(x - 1.5f, 0.0f, z), Quaternion.identity, walls);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, room.dist - 2);
					}
					break;
				case 1:
					{
						float x = -room.prevRoom.size.x / 2.0f + room.prevRoom.doors[1];
						float z = -(room.prevRoom.size.y + room.dist) / 2.0f;

						GameObject hInstance = Instantiate(floorPrefab, room.prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(2.0f, 1.0f, room.dist);

						hInstance = Instantiate(wallPrefab, room.prevRoom.position + new Vector3(x + 1.5f, 0.0f, z), Quaternion.identity, walls);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, room.dist - 2);

						hInstance = Instantiate(wallPrefab, room.prevRoom.position + new Vector3(x - 1.5f, 0.0f, z), Quaternion.identity, walls);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, room.dist - 2);
					}
					break;
				case 2:
					{
						float x = (room.prevRoom.size.x + room.dist) / 2.0f;
						float z = -room.prevRoom.size.y / 2.0f + room.prevRoom.doors[2];

						GameObject hInstance = Instantiate(floorPrefab, room.prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(room.dist, 1.0f, 2.0f);

						hInstance = Instantiate(wallPrefab, room.prevRoom.position + new Vector3(x, 0.0f, z + 1.5f), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(room.dist - 2, 2.0f, 1.0f);

						hInstance = Instantiate(wallPrefab, room.prevRoom.position + new Vector3(x, 0.0f, z - 1.5f), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(room.dist - 2, 2.0f, 1.0f);
					}
					break;
				case 3:
					{
						float x = -(room.prevRoom.size.x + room.dist) / 2.0f;
						float z = -room.prevRoom.size.y / 2.0f + room.prevRoom.doors[3];

						GameObject hInstance = Instantiate(floorPrefab, room.prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(room.dist, 1.0f, 2.0f);

						hInstance = Instantiate(wallPrefab, room.prevRoom.position + new Vector3(x, 0.0f, z + 1.5f), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(room.dist - 2, 2.0f, 1.0f);

						hInstance = Instantiate(wallPrefab, room.prevRoom.position + new Vector3(x, 0.0f, z - 1.5f), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(room.dist - 2, 2.0f, 1.0f);
					}
					break;
			}
		}
	}

	bool CollisionCheck(Room room, int direction)
	{
		Vector2 halfSizeA = (Vector2)room.size / 2.0f;

		foreach (Room r in rooms)
		{
			if (r != null && r != room)
			{
				Vector2 halfSizeB = (Vector2)r.size / 2.0f;

				if (room.position.x - halfSizeA.x <= r.position.x + halfSizeB.x && room.position.x + halfSizeA.x >= r.position.x - halfSizeB.x &&
					room.position.z - halfSizeA.y <= r.position.z + halfSizeB.y && room.position.z + halfSizeA.y >= r.position.z - halfSizeB.y)
				{
					return true;
				}
			}
		}

		return false;
	}
}
