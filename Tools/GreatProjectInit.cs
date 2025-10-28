using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GreatProjectInit : MonoBehaviour
{
    [SerializeField] string Name;
    [SerializeField] Sprite icon;
    [SerializeField] int startTier;
    [SerializeField] Effect[] T1, T2, T3;
    [SerializeField] Condition[] conditions;
    private void Start()
    {
        Tilemap tmp = Map.main.tileMapManager.tilemap;
        Vector3Int pos = tmp.WorldToCell(transform.position);
        TileData data = Map.main.GetTile(pos);
        if(data != null)
        {
            GreatProject greatProject = new GreatProject();
            greatProject.Name = Name;
            greatProject.icon = icon;
            greatProject.T1 = T1;
            greatProject.T2 = T2;
            greatProject.T3 = T3;
            greatProject.baseCost = 500;
            greatProject.baseTime = 360;
            greatProject.conditions = conditions;
            greatProject.tile = data;
            greatProject.tier = startTier;
            data.greatProject = greatProject;
        }
        Destroy(gameObject);
    }
    
}
