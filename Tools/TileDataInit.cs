using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDataInit : MonoBehaviour
{
    [SerializeField] string Name,Region,TradeRegion,Continent;
    [SerializeField] int armies,horses,artillery,marketLevel,status;
    [SerializeField] bool capital,fort,hasMarket;
    [SerializeField] int developmentA,developmentB,developmentC;
    [SerializeField] float control;
    [SerializeField] int transport,supply,warship;

    private void Start()
    {
        Tilemap tmp = Map.main.tileMapManager.tilemap;
        Vector3Int pos = tmp.WorldToCell(transform.position);
        TileData data = Map.main.GetTile(pos);
        if(data != null)
        {
            data.tileText = Instantiate(Map.main.tileTextPrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(0,60)), UIManager.main.worldCanvasText).GetComponent<TextMeshProUGUI>();
            data.tileText.text = Name;
            data.Name = Name;
            data.region = Region;
            data.tradeRegion = TradeRegion;
            if (Map.main.tradeRegions.ContainsKey(TradeRegion))
            {
                Map.main.tradeRegions[TradeRegion].AddTile(data);
                data.tradeRegionID = Map.main.tradeRegions[TradeRegion].id;
            }
            else
            {
                TradeRegion region = new TradeRegion();
                region.id = Map.main.tradeRegions.Count;
                Map.main.tradeRegions.Add(TradeRegion, region);
                region.name = TradeRegion;
                region.AddTile(data);
                data.tradeRegionID = region.id;
            }
            
            data.continent = Continent;
            data.developmentA = developmentA;
            data.developmentB = developmentB;
            data.developmentC = developmentC;
            if (hasMarket && marketLevel > 0)
            {
                data.hasMarket = true;
                data.marketLevel = marketLevel;
            }
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
                    civ.capitalIndicator = Instantiate(Map.main.capitalIndicatorPrefab, transform.position, Quaternion.identity,Map.main.capitalTransform);
                }
                data.settlementSprite = Instantiate(Map.main.settlementPrefab, transform.position, Quaternion.identity, Map.main.buildingTransform).GetComponent<SpriteRenderer>();
                if (status > 0)
                {
                    data.status = status;
                    civ.controlCentres.Add(pos, status);
                    data.UpdateStatusModifiers();                   
                }
                
                if (fort)
                {
                    data.fort = Instantiate(Map.main.fortPrefab, transform.position, Quaternion.identity, Map.main.fortTransform);
                    data.fortLevel += 1;
                    data.hasFort = true;
                    data.buildings.Add(0);
                    data.ApplyZOC();
                }
                if (armies > 0||horses > 0||artillery > 0)
                {
                    List<Regiment> regiments = new List<Regiment>();
                    for (int i = 0; i < armies; i++)
                    {
                        regiments.Add(new Regiment(data.civID));
                    }
                    for (int i = 0; i < horses; i++)
                    {
                        regiments.Add(new Regiment(data.civID,Type:1));
                    }
                    for (int i = 0; i < artillery; i++)
                    {
                        regiments.Add(new Regiment(data.civID, Type: 2));
                    }
                    Army.NewArmy(data, data.civID, regiments);
                }
                if (transport > 0|| supply > 0 || warship > 0)
                {
                    List<Boat> boatlist = new List<Boat>();
                    for (int i = 0; i < transport; i++)
                    {
                        boatlist.Add(new Boat(data.civID));
                    }
                    for (int i = 0; i < supply; i++)
                    {
                        boatlist.Add(new Boat(data.civID, Type: 1));
                    }
                    for (int i = 0; i < warship; i++)
                    {
                        boatlist.Add(new Boat(data.civID, Type: 2));
                    }
                    Fleet.NewFleet(data, data.civID, boatlist);
                }
            }                         
        }
        Destroy(gameObject);
    }
    
}
