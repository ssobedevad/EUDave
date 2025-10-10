using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AIManager : MonoBehaviour
{
    [SerializeField] int armyMoveUpdateTime;
    private void Start()
    {
        Game.main.tenMinTick.AddListener(TenMinTick);
        Game.main.dayTick.AddListener(DayTick);
    }
    public void Update()
    {
        UpdateCivTiles();
        UpdateMoveArmies();
        UpdateCivWars();
        UpdateMoveRebels();
    }
    public void TenMinTick()
    {        
        SpendAdminMana();
        SpendDiploMana();
        SpendMilMana();
    }
    public void DayTick()
    {
        DeclareCivWars();
    }
    public void UpdateMoveRebels()
    {
        if (Time.frameCount % armyMoveUpdateTime == 0)
        {
            foreach (var rebel in Game.main.rebelFactions)
            {
                if(rebel.path.Count > 0 || rebel.tile.underSiege) { continue; }
                List<Vector3Int> provs = new List<Vector3Int>();
                provs.AddRange(rebel.tile.civ.GetAllCivTiles());
                provs.AddRange(rebel.tile.GetNeighbors());
                provs.RemoveAll(i => Map.main.GetTile(i).occupied);
                provs.Sort((x, y) => AIMoveArmiesWar.ProvinceScore(y, rebel.pos).CompareTo(AIMoveArmiesWar.ProvinceScore(x, rebel.pos)));
                int loops = 0;
                while (provs.Count > 0 && loops < 100)
                {
                    if (!rebel.SetPath(provs[0]))
                    {
                        provs.RemoveAt(0);
                    }
                    else
                    {
                        provs.RemoveAt(0);
                        break;
                    }
                    loops++;
                }              
            }
        }
    }
    public void SpendAdminMana()
    {
        for (int i = Game.main.gameTime.totalTicks() % armyMoveUpdateTime; i < Game.main.civs.Count; i += armyMoveUpdateTime)
        {
            if (i < Game.main.civs.Count)
            {
                Civilisation civ = Game.main.civs[i];
                if (civ.isPlayer||!civ.isActive()) { continue; }
                List<TileData> tiles = civ.GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
                tiles.Sort((x,y)=> x.GetDevCost(i).CompareTo(y.GetDevCost(i)));
                bool needCores = false;
                foreach(var tile in tiles)
                {
                    if (tile.needsCoring())
                    {
                        if (civ.adminPower >= tile.GetCoreCost())
                        {
                            tile.StartCore();
                        }
                        else
                        {
                            needCores = true;
                        }
                    }
                }
                int cost = 600;
                if (Map.main.TechA.Length > civ.adminTech + 1)
                {
                    Tech tech = Map.main.TechA[civ.adminTech + 1];
                    cost = TechnologyUI.GetTechCost(tech, civ);
                    float monthDiff = (tech.expectedDate.totalTicks() - Game.main.gameTime.totalTicks()) / (6 * 24 * 30);
                    if (monthDiff <= 1)
                    {
                        if (civ.adminPower >= cost)
                        {
                            tech.TakeTech(i);
                            civ.adminTech++;
                            civ.adminPower -= cost;
                        }
                    }
                }
                if (!needCores && civ.GetWars().Count == 0 && civ.adminPower > 600 + tiles[0].GetDevCost(i))
                {
                    if (civ.adminPower >= tiles[0].GetDevCost(i))
                    {
                        tiles[0].AddDevelopment(0, i);
                    }
                }                                
            }
        }
    }
    public void SpendDiploMana()
    {
        for (int i = Game.main.gameTime.totalTicks() % armyMoveUpdateTime; i < Game.main.civs.Count; i += armyMoveUpdateTime)
        {
            if (i < Game.main.civs.Count)
            {
                Civilisation civ = Game.main.civs[i];
                if (civ.isPlayer || !civ.isActive()) { continue; }
                int cost = 600;
                if (Map.main.TechD.Length > civ.diploTech + 1)
                {
                    Tech tech = Map.main.TechD[civ.diploTech + 1];
                    cost = TechnologyUI.GetTechCost(tech, civ);
                    float monthDiff = (tech.expectedDate.totalTicks() - Game.main.gameTime.totalTicks()) / (6 * 24 * 30);
                    if(monthDiff <= 1)
                    {
                        if (civ.diploPower >= cost)
                        {
                            tech.TakeTech(i);
                            civ.diploTech++;
                            civ.diploPower -= cost; 
                        }
                    }
                }
                List<TileData> tiles = civ.GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
                tiles.Sort((x, y) => x.GetDevCost(i).CompareTo(y.GetDevCost(i)));
                if (civ.diploPower >= tiles[0].GetDevCost(i) && civ.diploPower > 600 + tiles[0].GetDevCost(i))
                {
                    tiles[0].AddDevelopment(1,i);                    
                }
            }
        }
    }
    public void SpendMilMana()
    {
        for (int i = Game.main.gameTime.totalTicks() % armyMoveUpdateTime; i < Game.main.civs.Count; i += armyMoveUpdateTime)
        {
            if (i < Game.main.civs.Count)
            {
                Civilisation civ = Game.main.civs[i];
                if (civ.isPlayer || !civ.isActive()) { continue; }
                List<TileData> tiles = civ.GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
                tiles.Sort((x, y) => x.GetDevCost(i).CompareTo(y.GetDevCost(i)));
                int cost = 600;
                if (Map.main.TechM.Length > civ.milTech + 1)
                {
                    Tech tech = Map.main.TechM[civ.milTech + 1];
                    cost = TechnologyUI.GetTechCost(tech, civ);
                    float monthDiff = (tech.expectedDate.totalTicks() - Game.main.gameTime.totalTicks()) / (6 * 24 * 30);
                    if (monthDiff <= 1)
                    {
                        if (civ.milPower >= cost)
                        {
                            tech.TakeTech(i);
                            civ.milTech++;
                            civ.milPower -= cost;
                        }
                    }
                }
                if (civ.milPower >= tiles[0].GetDevCost(i) && civ.milPower > 600 + tiles[0].GetDevCost(i))
                {
                    tiles[0].AddDevelopment(2, i);
                }
            }
        }
    }
    public void DeclareCivWars()
    {
        for (int i = Game.main.gameTime.totalTicks() % armyMoveUpdateTime; i < Game.main.civs.Count; i += armyMoveUpdateTime)
        {
            if (i < Game.main.civs.Count )
            {
                Civilisation civ = Game.main.civs[i];
                if (civ.isPlayer || !civ.isActive()) { continue; }
                if(civ.atWarWith.Count == 0 && civ.GetTotalTilePopulation() > civ.GetTotalMaxPopulation() * 0.65f && civ.overextension <= 1f)
                {
                    foreach (var n in civ.civNeighbours)
                    {
                        Civilisation neighbor = Game.main.civs[n];
                        if(civ.truces[neighbor.CivID] == 0 && neighbor.TotalMilStrength() < civ.TotalMilStrength() * 2f && neighbor.TotalArmySize() < civ.TotalArmySize())
                        {
                            civ.DeclareWar(n);
                            break;
                        }
                    }
                }
            }
        }
    }
    public void UpdateCivWars() 
    {
        for (int i = Time.frameCount % armyMoveUpdateTime; i < Game.main.ongoingWars.Count; i += armyMoveUpdateTime)
        {
            if (i < Game.main.ongoingWars.Count)
            {
                War war = Game.main.ongoingWars[i];
                if (war.warScore == 100 && !war.attackerCiv.isPlayer) 
                {
                    war.defenderCiv.AcceptPeaceDeal(PeaceDealUI.Suggested(war.defenderCiv, war));
                    war.EndWar();
                    continue; 
                }
                if (war.warScore == -100 && !war.defenderCiv.isPlayer) 
                {
                    war.attackerCiv.AcceptPeaceDeal(PeaceDealUI.Suggested(war.attackerCiv, war));
                    war.EndWar();
                    continue; 
                }
                if (!war.attackerCiv.isPlayer && !war.defenderCiv.isPlayer)
                {
                    if(war.warScore >= 98f)
                    {
                        PeaceDeal peaceDeal = PeaceDealUI.Suggested(war.defenderCiv, war);
                        if (PeaceDealUI.WillAccept(peaceDeal,war.defenderCiv,war))
                        {
                            war.defenderCiv.AcceptPeaceDeal(peaceDeal);
                            war.EndWar();
                            continue;
                        }
                    }
                    if(war.warScore <= -98f)
                    {
                        PeaceDeal peaceDeal = PeaceDealUI.Suggested(war.attackerCiv, war);
                        if (PeaceDealUI.WillAccept(peaceDeal, war.attackerCiv, war))
                        {
                            war.attackerCiv.AcceptPeaceDeal(peaceDeal);
                            war.EndWar();
                            continue;
                        }
                    }
                }
            }
        }
    }
    public void UpdateCivTiles()
    {
        if(Time.frameCount % armyMoveUpdateTime == 0) 
        {
            foreach(var civ in Game.main.civs)
            {
                civ.civTiles.Clear();
                civ.civNeighbours.Clear();
                civ.totalIdeas = 0;
                foreach (var civIdea in civ.ideaGroups)
                {
                    if (civIdea!= null && civIdea.active)
                    {
                        civ.totalIdeas += civIdea.unlockedLevel;
                    }
                }
            }
            foreach (TileData td in Map.main.tiles)
            {
                if (td != null && td.civID > -1)
                {
                    if(td.totalDev > Game.main.highestDevelopment)
                    {
                        Game.main.highestDevelopment = td.totalDev;
                    }
                    Civilisation civ = Game.main.civs[td.civID];
                    if (!civ.civTiles.Contains(td.pos))
                    {
                        civ.civTiles.Add(td.pos);
                        List<Vector3Int> nbs = td.GetNeighbors();
                        foreach(var n in nbs)
                        {
                            TileData ntd = Map.main.GetTile(n);
                            if(ntd.civID != -1 && ntd.civID != td.civID)
                            {

                                if (!civ.civNeighbours.Contains(ntd.civID))
                                { 
                                    civ.civNeighbours.Add(ntd.civID);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var civ in Game.main.civs)
            {
                if (!civ.hasUpdatedStartingResources)
                {
                    civ.UpdateStartingResources();
                    civ.hasUpdatedStartingResources = true;
                }
            }
        }

    }
    public void UpdateMoveArmies()
    {
        for(int i = Time.frameCount % armyMoveUpdateTime; i < Game.main.civs.Count; i+= armyMoveUpdateTime) 
        {
            if(i < Game.main.civs.Count)
            {
                if (Game.main.civs[i].isPlayer) { continue; }
                if (Game.main.civs[i].isActive())
                {
                    UpdateMoveUnits(i);
                }
            }
        }
    }
    public void UpdateMoveUnits(int civID)
    {
        Civilisation civ = Game.main.civs[civID];
        if(civ.atWarWith.Count > 0)
        {
            AIMoveArmiesWar.MoveAtWar(civID);
        }
        else
        {
            AIMoveArmiesPeace.MoveAtPeace(civID);
        }
    }
}
