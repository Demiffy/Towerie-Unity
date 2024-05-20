using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[] maps;

    private GameObject currentMap;

    void Start()
    {
        int mapIndex = PlayerPrefs.GetInt("SelectedMapIndex", 0);
        LoadMap(mapIndex);
        Debug.Log($"Map with index {mapIndex} loaded!");
    }

    public void LoadMap(int index)
    {
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        if (index >= 0 && index < maps.Length)
        {
            currentMap = Instantiate(maps[index], Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Map index out of range!");
        }
    }
}
