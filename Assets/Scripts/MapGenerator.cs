using Unity.VisualScripting;
using UnityEngine;

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
	private byte roomCount = 1;

	private Room[] rooms = new Room[ROOM_COUNT];

	private void Start()
	{
		int prevRoom = 0;

		for (int i = 0; i < ROOM_COUNT && prevRoom != ROOM_COUNT; ++i)
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

			spawn.doors[0] = (sbyte)Random.Range(-spawn.size.x, spawn.size.x - 1);
			if(spawn.doors[0] > 0) { ++roomCount; }
			spawn.doors[1] = (sbyte)Random.Range(-spawn.size.x, spawn.size.x - 1);
			if(spawn.doors[1] > 0) { ++roomCount; }
			spawn.doors[3] = (sbyte)Random.Range(-spawn.size.y, spawn.size.y - 1);
			if(spawn.doors[3] > 0) { ++roomCount; }

			spawn.doors[2] = (sbyte)Random.Range(1, spawn.size.y - 1);

			GameObject instance = Instantiate(floorPrefab, new Vector3(spawn.position.x, 0.0f, spawn.position.z), Quaternion.identity, floor);
			instance.transform.localScale = new Vector3(spawn.size.x, 1.0f, spawn.size.y);

			SetupWalls(spawn, 4);

			//TODO: Spawn enemies

			prevNum = num;
		}
		else
		{
			Room prevRoom = rooms[prevNum];
			Room room = rooms[num];

			room.size = new Vector2Int(Random.Range(3, 11), Random.Range(3, 11)) * 2 + Vector2Int.one;

			byte doorIndex = 4;

			while (doorIndex == 4)
			{
				for (byte i = 0; i < 4; ++i)
				{
					if (prevRoom.doors[i] > 0)
					{
						doorIndex = i;

						break;
					}
				}

				if (doorIndex == 4)
				{
					if (++prevNum == ROOM_COUNT) { return; }
					else { prevRoom = rooms[prevNum]; }
				}
			}

			room.position = prevRoom.position;

			switch (doorIndex)
			{
				case 0: //NORTH
					{
						room.doors[1] = (sbyte)Random.Range(1, room.size.x - 1);
						int dist = Random.Range(3, 10);
						room.position.z += (room.size.y + prevRoom.size.y) / 2.0f + dist;
						room.position.x += (room.size.x - prevRoom.size.x) / 2.0f - (room.doors[1] - prevRoom.doors[0]);

						float x = -prevRoom.size.x / 2.0f + prevRoom.doors[0];
						float z = (prevRoom.size.y + dist) / 2.0f;

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(2.0f, 1.0f, dist);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x + 1.5f, 0.0f, z), Quaternion.identity, walls);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x - 1.5f, 0.0f, z), Quaternion.identity, walls);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						prevRoom.doors[0] = 0;
					}
					break;
				case 1: //SOUTH
					{
						room.doors[0] = (sbyte)Random.Range(1, room.size.x - 1);
						int dist = Random.Range(3, 10);
						room.position.z -= (room.size.y + prevRoom.size.y) / 2.0f + dist;
						room.position.x += (room.size.x - prevRoom.size.x) / 2.0f - (room.doors[0] - prevRoom.doors[1]);

						float x = -prevRoom.size.x / 2.0f + prevRoom.doors[1];
						float z = -(prevRoom.size.y + dist) / 2.0f;

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(2.0f, 1.0f, dist);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x + 1.5f, 0.0f, z), Quaternion.identity, walls);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x - 1.5f, 0.0f, z), Quaternion.identity, walls);
						hInstance.transform.localScale = new Vector3(1.0f, 2.0f, dist - 2);

						prevRoom.doors[1] = 0;
					}
					break;
				case 2: //EAST
					{
						room.doors[3] = (sbyte)Random.Range(1, room.size.y - 1);
						int dist = Random.Range(3, 10);
						room.position.x += (room.size.x + prevRoom.size.x) / 2.0f + dist;
						room.position.z += (room.size.y - prevRoom.size.y) / 2.0f - (room.doors[3] - prevRoom.doors[2]);

						float x = (prevRoom.size.x + dist) / 2.0f;
						float z = -prevRoom.size.y / 2.0f + prevRoom.doors[2];

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(dist, 1.0f, 2.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z + 1.5f), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z - 1.5f), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						prevRoom.doors[2] = 0;
					}
					break;
				case 3: //WEST
					{
						room.doors[2] = (sbyte)Random.Range(1, room.size.y - 1);
						int dist = Random.Range(3, 10);
						room.position.x -= (room.size.x + prevRoom.size.x) / 2.0f + dist;
						room.position.z += (room.size.y - prevRoom.size.y) / 2.0f - (room.doors[2] - prevRoom.doors[3]);

						float x = -(prevRoom.size.x + dist) / 2.0f;
						float z = -prevRoom.size.y / 2.0f + prevRoom.doors[3];

						GameObject hInstance = Instantiate(floorPrefab, prevRoom.position + new Vector3(x, -0.5f, z), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(dist, 1.0f, 2.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z + 1.5f), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						hInstance = Instantiate(wallPrefab, prevRoom.position + new Vector3(x, 0.0f, z - 1.5f), Quaternion.identity, floor);
						hInstance.transform.localScale = new Vector3(dist - 2, 2.0f, 1.0f);

						prevRoom.doors[3] = 0;
					}
					break;
			}

			GameObject instance = Instantiate(floorPrefab, new Vector3(room.position.x, 0.0f, room.position.z), Quaternion.identity, floor);
			instance.transform.localScale = new Vector3(room.size.x, 1.0f, room.size.y);

			if (roomCount < ROOM_COUNT - 1 && room.doors[0] < 1) { room.doors[0] = (sbyte)Random.Range(-room.size.x, room.size.x - 1); if (room.doors[0] > 0) { ++roomCount; } }
			if (roomCount < ROOM_COUNT - 1 && room.doors[1] < 1) { room.doors[1] = (sbyte)Random.Range(-room.size.x, room.size.x - 1); if (room.doors[1] > 0) { ++roomCount; } }
			if (roomCount < ROOM_COUNT - 1 && room.doors[2] < 1) { room.doors[2] = (sbyte)Random.Range(-room.size.y, room.size.y - 1); if (room.doors[2] > 0) { ++roomCount; } }
			if (roomCount < ROOM_COUNT - 1 && room.doors[3] < 1) { room.doors[3] = (sbyte)Random.Range(-room.size.y, room.size.y - 1); if (room.doors[3] > 0) { ++roomCount; } }

			SetupWalls(room, doorIndex);
		}
	}

	private void SetupWalls(Room room, byte doorIndex)
	{
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

		switch (doorIndex)
		{
			case 0: room.doors[1] = 0; break;
			case 1: room.doors[0] = 0; break;
			case 2: room.doors[3] = 0; break;
			case 3: room.doors[2] = 0; break;
		}
	}
}
