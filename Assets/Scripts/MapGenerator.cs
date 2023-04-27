using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[SerializeField] private Room roomPrefab;

	private static readonly byte ROOM_COUNT = MapHandler.ROOM_COUNT;
	private byte roomCount;

	public void Generate(ref Room[] rooms)
	{
		int prevRoom;
		bool success;

		do
		{
			if (rooms != null) { foreach (Room room in rooms) { if (room != null) { Destroy(room.gameObject); } } }

			roomCount = 1;
			prevRoom = 0;
			success = true;
			rooms = new Room[ROOM_COUNT];

			for (int i = 0; i < ROOM_COUNT && prevRoom != ROOM_COUNT;)
			{
				rooms[i] = Instantiate(roomPrefab);
				if (GenerateRoom(rooms, i, ref prevRoom)) { ++i; }
				if (prevRoom >= i)
				{
					success = false;
					break;
				}
			}

			if(roomCount != ROOM_COUNT - 1) { success = false; }

		} while (!success);
	}

	private bool GenerateRoom(Room[] rooms, int num, ref int prevNum)
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
					}
					break;
			}

			if (CollisionCheck(rooms, room)) { room.prevRoom.doors[doorIndex] = 0; return false; }

			if (roomCount < ROOM_COUNT - 1 && room.doors[0] < 1) { room.doors[0] = (sbyte)Random.Range(-room.size.x, room.size.x - 1); if (room.doors[0] > 0) { ++roomCount; } }
			if (roomCount < ROOM_COUNT - 1 && room.doors[1] < 1) { room.doors[1] = (sbyte)Random.Range(-room.size.x, room.size.x - 1); if (room.doors[1] > 0) { ++roomCount; } }
			if (roomCount < ROOM_COUNT - 1 && room.doors[2] < 1) { room.doors[2] = (sbyte)Random.Range(-room.size.y, room.size.y - 1); if (room.doors[2] > 0) { ++roomCount; } }
			if (roomCount < ROOM_COUNT - 1 && room.doors[3] < 1) { room.doors[3] = (sbyte)Random.Range(-room.size.y, room.size.y - 1); if (room.doors[3] > 0) { ++roomCount; } }

			return true;
		}
	}

	bool CollisionCheck(Room[] rooms, Room room)
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
