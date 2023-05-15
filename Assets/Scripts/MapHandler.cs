using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler : MonoBehaviour
{
	private MapGenerator generator;
	private Room[] rooms;

	[SerializeField] private GameObject enemy;
	[SerializeField] private GameObject boss;

	public static readonly byte ROOM_COUNT = 8;

	private void Awake()
    {
		generator = FindFirstObjectByType<MapGenerator>();
		generator.Generate(ref rooms);

		rooms[0].Setup();

		for (int i = 1; i < ROOM_COUNT - 1; ++i)
		{
			rooms[i].Setup();
			SpawnEnemies(rooms[i]);
		}

		rooms[ROOM_COUNT - 1].finalRoom = true;
		rooms[ROOM_COUNT - 1].Setup();

		SpawnBoss(rooms[ROOM_COUNT - 1]);
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

	void SpawnBoss(Room room)
	{
		Instantiate(boss, room.position + Vector3.up * 2.0f, Quaternion.identity);
	}
}
