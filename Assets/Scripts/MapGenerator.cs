using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[SerializeField] private Transform floor;
	[SerializeField] private Transform walls;
	[SerializeField] private GameObject floorPrefab;
	[SerializeField] private GameObject wallPrefab;

    private void Start()
    {
		Vector2 floorSize = new Vector2(Random.Range(5, 20), Random.Range(5, 20));

		GameObject instance = Instantiate(floorPrefab, new Vector3(0.0f, -0.5f, 0.0f), Quaternion.identity, floor);
		instance.transform.localScale = new Vector3(floorSize.x, 1, floorSize.y);

		instance = Instantiate(wallPrefab, new Vector3((floorSize.x + 1) * 0.5f, 0.0f, 0.0f), Quaternion.identity, walls);
		instance.transform.localScale = new Vector3(1, 2, floorSize.y);

		instance = Instantiate(wallPrefab, new Vector3(-(floorSize.x + 1) * 0.5f, 0.0f, 0.0f), Quaternion.identity, walls);
		instance.transform.localScale = new Vector3(1, 2, floorSize.y);

		instance = Instantiate(wallPrefab, new Vector3(0.0f, 0.0f, (floorSize.y + 1) * 0.5f), Quaternion.identity, walls);
		instance.transform.localScale = new Vector3(floorSize.x + 2, 2, 1);

		instance = Instantiate(wallPrefab, new Vector3(0.0f, 0.0f, -(floorSize.y + 1) * 0.5f), Quaternion.identity, walls);
		instance.transform.localScale = new Vector3(floorSize.x + 2, 2, 1);
	}
}
