using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustomPropTile : MonoBehaviour
{
    public Vector3 cellScale = new Vector3(1, 1, 1); // meshes scale
    public Vector3 tileScaler = new Vector3(1, 1, 1); // to change the size of the tile
    public Vector3 offset = new Vector3(0, 0, 0);
    public Vector2Int gridSize = new Vector2Int(1, 1);
    public GameObject tilePrefab;

    [ContextMenu("UpdateTile")]
    public void UpdateTile()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab is null");
            return;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.transform.localScale = tileScaler; // change the size of the tile
                // set the position of the tile considering the new size
                tile.transform.localPosition = new Vector3(x * cellScale.x * tileScaler.x, 0, z * cellScale.z * tileScaler.z) + offset;
            }
        }

    }
}
