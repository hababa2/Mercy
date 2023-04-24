using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator2 : MonoBehaviour
{
	class Door
	{
		public byte side = 4;
		public byte position;
		public Room otherRoom;
	}

	struct Room
	{
		public Vector2 position;
		public Vector2 size;
		public int doorCount;
		public Door[] doors;
	}

	[SerializeField] private Transform floor;
	[SerializeField] private Transform walls;
	[SerializeField] private GameObject floorPrefab;
	[SerializeField] private GameObject wallPrefab;

	private static readonly byte ROOM_COUNT = 8;
	Room[] rooms = new Room[ROOM_COUNT];

	private void Awake()
	{
		int prevRoom = 0;

		for(int i = 0; i < ROOM_COUNT; ++i)
		{
			GenerateRoom(i, ref prevRoom);
		}
	}

	void GenerateRoom(int roomNum, ref int prevRoom)
	{
		Room room = new Room();

		room.size = new Vector2(Random.Range(3, 11), Random.Range(3, 11)) * 2 + Vector2.one;

		if (roomNum == 0)
		{
			//No other rooms, this is spawn
			//TODO: Add player spawner
			room.position = Vector2.zero;
			room.doorCount = Random.Range(1, 5);
			room.doors = new Door[room.doorCount];

			rooms[roomNum] = room;
		}
		else
		{
			Room prev = rooms[prevRoom];

			foreach(Door door in room.doors)
			{
				if(door.side == 4)
				{
					//TODO: Get door side
				}
			}

			if (prev.doors[prev.doorCount - 1] != null) { prev = rooms[++prevRoom]; }


			//TODO: Find a good spot for this room and connect it
		}
	}
}
