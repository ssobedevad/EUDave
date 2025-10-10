using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDataInit : MonoBehaviour
{
    [SerializeField] string Name,Region;
    [SerializeField] int armies;
    [SerializeField] bool capital,fort;
    [SerializeField] int developmentA,developmentB,developmentC;
    [SerializeField] float control;

    private void Start()
    {
        Tilemap tmp = Map.main.tileMapManager.tilemap;
        Vector3Int pos = tmp.WorldToCell(transform.position);
        TileData data = Map.main.GetTile(pos);
        if(data != null)
        {
            data.Name = Name;
            data.region = Region;
            data.developmentA = developmentA;
            data.developmentB = developmentB;
            data.developmentC = developmentC;
            if(control != 0) { data.control = control; }
            else 
            { 
            data.control = 100f;
            }
            data.SetDevCost();
            data.population = (int)(0.65f * data.maxPopulation);
            data.avaliablePopulation = (int)(0.65f * data.avaliableMaxPopulation);
            if (data.civID > -1)
            {
                Civilisation civ = Game.main.civs[data.civID];
                if (capital && civ.capitalPos == Vector3Int.zero)
                {
                    civ.capitalPos = pos;
                    data.fortLevel += 1;
                    civ.capitalIndicator = Instantiate(Map.main.capitalIndicatorPrefab, transform.position, Quaternion.identity);
                }
                if (fort)
                {
                    Instantiate(Map.main.fortPrefab, transform.position, Quaternion.identity);
                    data.fortLevel += 1;
                    data.hasFort = true;
                    data.buildings.Add(0);
                    data.ApplyZOC();
                }
                if (armies > 0)
                {
                    List<Regiment> regiments = new List<Regiment>();
                    for (int i = 0; i < armies; i++)
                    {
                        regiments.Add(new Regiment(data.civID));
                    }
                    Army.NewArmy(data, data.civID, regiments);
                }
            }                         
        }
        Destroy(gameObject);
    }
    
}
