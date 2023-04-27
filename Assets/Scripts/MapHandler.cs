using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler : MonoBehaviour
{
	private MapGenerator generator;
	private Room[] rooms;

	[SerializeField] private GameObject enemy;

	public static readonly byte ROOM_COUNT = 8;

	private void Awake()
    {
		generator = FindFirstObjectByType<MapGenerator>();
		generator.Generate(ref rooms);

		for (int i = 0; i < ROOM_COUNT; ++i)
		{
			rooms[i].Setup();
			if (i > 0) { SpawnEnemies(rooms[i]); }
		}
	}

	void SpawnEnemies(Room room)
	{
		int enemyCount = (room.size.x + room.size.y) / 6;
		float halfX = room.size.x / 2.0f - 1.0f;
		float halfY = room.size.y / 2.0f - 1.0f;

		for (int i = 0; i < enemyCount; ++i)
		{
			Instantiate(enemy, room.position + new Vector3(Random.Range(-halfX, halfX), 1.0f, Random.Range(-halfY, halfY)), Quaternion.identity);
		}
	}
}
