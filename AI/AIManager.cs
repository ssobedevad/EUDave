using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static MultiplayerManager;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AIManager : MonoBehaviour
{
    [SerializeField] int armyMoveUpdateTime;
    private void Start()
    {
        Game.main.tenMinTick.AddListener(TenMinTick);
        Game.main.hourTick.AddListener(HourTick);
    }
    public void Update()
    {       
        UpdateCivTiles();
        if (!Game.main.Started) { return; }
        if (Game.main.isMultiplayer && !NetworkManager.Singleton.IsServer) { return; }
        UpdateMoveArmies();
        UpdateCivWars();
    }
    public void TenMinTick()
    {
        if (Game.main.gameTime.totalTicks() < 6 * 25) { return; }
        if (Game.main.isMultiplayer && !NetworkManager.Singleton.IsServer) { return; }
        for (int i = Game.main.gameTime.totalTicks() % armyMoveUpdateTime; i < Game.main.civs.Count; i += armyMoveUpdateTime)
        {
            if (i < Game.main.civs.Count)
            {
                Civilisation civ = Game.main.civs[i];
                if (civ.isPlayer || !civ.isActive()) { continue; }
                ManageReligiousMechanics(civ);
                TakeIdeaGroups(civ);
                SpendAdminMana(civ);
                SpendDiploMana(civ);
                SpendMilMana(civ);
                ManageCoins(civ);
                TakeGovernmentReform(civ);
                PromoteStatus(civ);
                UpdateDiplomats(civ);
                CompleteMissions(civ);
            }
        }
    }

    void CompleteMissions(Civilisation civ)
    {
        for(int i = 0; i < civ.missions.Length; i++)
        {
            if (!civ.missionProgress[i])
            {
                Mission mission = civ.missions[i];
                if (mission.CanTake(civ))
                {
                    MissionsUI.TakeMission(mission, civ);
                    civ.missionProgress[i] = true;
                }
            }
        }
    }
    public void HourTick()
    {
        if (Game.main.isMultiplayer && !NetworkManager.Singleton.IsServer) { return; }
        OfferAlliances();
        if (Game.main.gameTime.totalTicks() < 6 * 24 * 7) { return; }
        DeclareCivWars();
    }
    public void PromoteStatus(Civilisation civ)
    {
        if(civ.adminPower < 50)
        {
            return;
        }

        if (civ.maxSettlements.v > civ.controlCentres.Count)
        {
            List<TileData> tiles = civ.GetAllCivTileDatas().ToList();
            tiles.RemoveAll(i => i.civID != civ.CivID);
            if (tiles.Count == 0) { return; }
            tiles.Sort((x, y) => ((100f - y.maxControl) * y.totalDev).CompareTo((100f - x.maxControl) * x.totalDev));
            if (tiles[0].maxControl < 40)
            {
                TileData tile = tiles[0];
                if (tile.CanPromoteStatus())
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.TileActionRpc(tile.pos, MultiplayerManager.TileActions.CoreConvertStatus, 2);
                    }
                    else
                    {
                        tile.PromoteStatus();
                    }

                }
            }
        }
        else
        {
            foreach(var centre in civ.controlCentres.ToList())
            {
                if(centre.Value < 3)
                {
                    TileData tile = Map.main.GetTile(centre.Key);
                    if (tile.CanPromoteStatus())
                    {
                        if (Game.main.isMultiplayer)
                        {
                            Game.main.multiplayerManager.TileActionRpc(tile.pos, MultiplayerManager.TileActions.CoreConvertStatus, 2);
                        }
                        else
                        {
                            tile.PromoteStatus();
                        }
                    }
                }
            }
        }
        
    }
    public void UpdateDiplomats(Civilisation civ)
    {
        foreach(var diplomat in civ.deployedDiplomats)
        {
            if(diplomat.Action == DiplomatAction.Establishing)
            {
                Civilisation target = Game.main.civs[diplomat.targetCivId];
                float relations = 0f;
                if (target.opinionOfThem[civ.CivID].ms.Exists(i => i.n == "Improved Relations"))
                {
                    relations = target.opinionOfThem[civ.CivID].ms.Find(i => i.n == "Improved Relations").v;
                }
                if(relations >= 99f)
                {
                    diplomat.Action = DiplomatAction.Travelling;
                }
            }
            if (diplomat.Action == DiplomatAction.Spying)
            {
                if (civ.spyNetwork[diplomat.targetCivId] > 20f)
                {
                    diplomat.Action = DiplomatAction.Travelling;
                }
            }
        }
        if(civ.avaliableDiplomats > 1)
        {
            List<int> possibleCivs = new List<int>();
            possibleCivs.AddRange(civ.subjects);
            possibleCivs.AddRange(civ.allies);
            possibleCivs.AddRange(civ.civNeighbours);
            int targetID = -1;
            if (possibleCivs.Count > 0)
            {
                targetID = WeightedChoiceManager.Shuffle(possibleCivs)[0];
                if (targetID > -1)
                {
                    civ.deployedDiplomats.Add(new DiplomatStatus(Game.main.civs[targetID], civ, DiplomatAction.Establishing));
                }
            }
        }
    }
    public void TakeGovernmentReform(Civilisation civ)
    {
        if (civ.reformProgress >= civ.reforms.Count * 40 + 40)
        {
            
            GovernmentType governmentType = Map.main.governmentTypes[civ.government];
            if (governmentType.BaseReforms.Length == 0 || governmentType.BaseReforms.Length <= civ.reforms.Count) { return; }
            GovernmentReformTier tier = governmentType.BaseReforms[civ.reforms.Count];            
            List<GovernmentReform> tierReforms = tier.Reforms.ToList();
            for (int a = 0; a < tier.Reforms.Length; a++)
            {
                if (!tier.Reforms[a].CanTake(civ))
                {
                    tierReforms.Remove(tier.Reforms[a]);
                }
            }
            if(tierReforms.Count == 0) { return; }
            int id = UnityEngine.Random.Range(0, tierReforms.Count);
            GovernmentReform reform = tierReforms[id];
            int cost = (40 + 40 * civ.reforms.Count);
            civ.reformProgress -= cost;
            GovernmentUI.BuyReform(civ, reform);
            int reformIndex = tier.Reforms.ToList().IndexOf(reform);
            if (Game.main.isMultiplayer)
            {
                Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.PickReform, reformIndex);
            }
            else
            {
                civ.reforms.Add(reformIndex);
            }
        }
    }
    public void ManageReligiousMechanics(Civilisation civ)
    {
        if(civ.religion == 1)
        {
            if(civ.religiousPoints >= 50)
            {
                if (civ.religiousUnity < 1f)
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.ReligiousMechanic, 2);
                    }
                    else
                    {
                        DjinnWorshipUI.DjinnFavor(civ, 2);
                    }                   
                }
                if (civ.GetWars().Count > 0)
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.ReligiousMechanic, 0);
                    }
                    else
                    {
                        DjinnWorshipUI.DjinnFavor(civ, 0);
                    }
                }
                if (civ.adminPower > 600 || civ.diploPower > 600 || civ.milPower > 600)
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.ReligiousMechanic, 1);
                    }
                    else
                    {
                        DjinnWorshipUI.DjinnFavor(civ, 1);
                    }
                }
            }
        }
        else if (civ.religion == 2)
        {
            if (civ.religiousPoints == -1)
            {
                int index = UnityEngine.Random.Range(0, Map.main.religions[2].religiousMechanicEffects.Length);
                if (Game.main.isMultiplayer)
                {
                    Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.ReligiousMechanic, index);
                }
                else
                {
                    LocalDeitiesUI.SelectDeity(civ, index);
                }                
            }
        }
        else if (civ.religion == 3)
        {
            if (civ.religiousPoints >= 30)
            {
                int index = 0;
                if(civ.subjects.Count > 0)
                {
                    if (Game.main.civs[civ.subjects[0]].libertyDesire < 20)
                    {
                        index = 1;
                    }
                }
                if(civ.heir.active && civ.heir.adminSkill + civ.heir.diploSkill + civ.heir.milSkill < 6 && civ.stability > 0)
                {
                    index = 2;
                }               
                if (Game.main.isMultiplayer)
                {
                    Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.ReligiousMechanic,0, index);
                }
                else
                {
                    GreatSwampUI.Feed(civ, index);
                }
            }
            else
            {
                if (civ.GetWars().Count > 0)
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.ReligiousMechanic, 1, 0);
                    }
                    else
                    {
                        GreatSwampUI.Ask(civ, 0);
                    }
                }
                if (civ.subjects.Count > 0)
                {
                    if (Game.main.civs[civ.subjects[0]].libertyDesire > 50)
                    {
                        if (Game.main.isMultiplayer)
                        {
                            Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.ReligiousMechanic, 1, 1);
                        }
                        else
                        {
                            GreatSwampUI.Ask(civ, 1);
                        }
                    }
                }
                if (civ.GetWars().Count > 0)
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.ReligiousMechanic, 1, 2);
                    }
                    else
                    {
                        GreatSwampUI.Ask(civ, 2);
                    }
                }

            }
        }
    }
    public void ManageCoins(Civilisation civ)
    {       
        float balance = civ.GetBalance();
        if (balance < 0)
        {           
            if(civ.TotalMaxArmySize()/1000f > civ.forceLimit.v && civ.atWarWith.Count == 0)
            {
                int armiesToRemove = (int)(civ.TotalMaxArmySize() / 1000f - civ.forceLimit.v);
                if (Game.main.isMultiplayer)
                {
                    Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.DisbandArmy, armiesToRemove);
                }
                else
                {
                    foreach (var army in civ.armies.ToList())
                    {
                        army.DisbandMercs();
                        if (army.ArmyMaxSize() / 1000f <= armiesToRemove)
                        {
                            armiesToRemove -= (int)(army.ArmyMaxSize() / 1000f);
                            army.Disband();
                        }
                        else if (army.ArmyMaxSize() / 1000f > armiesToRemove)
                        {
                            int remain = (int)(army.ArmyMaxSize() / 1000f - armiesToRemove);
                            armiesToRemove -= armiesToRemove;
                            army.SplitOff(remain);
                            army.Disband();
                        }
                        if (armiesToRemove <= 0)
                        {
                            break;
                        }
                    }
                }               
            }
            if (civ.FortMaintenance() > 1 && civ.atWarWith.Count == 0 && civ.overlordID == -1)
            {
                List<Vector3Int> forts = civ.GetAllCivTiles().FindAll(i => Map.main.GetTile(i).hasFort);
                for (int i = 0; i < forts.Count; i++)
                {
                    if (balance < -1)
                    {
                        TileData data = Map.main.GetTile(forts[i]);
                        if (Game.main.isMultiplayer)
                        {
                            Game.main.multiplayerManager.TileActionRpc(data.pos, MultiplayerManager.TileActions.RemoveBuilding, 0);
                        }
                        else
                        {
                            data.RemoveBuilding(0);
                        }
                        balance += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            int advisorCheck = UnityEngine.Random.Range(0, 3);
            Advisor advisor = advisorCheck == 0 ? civ.advisorA : advisorCheck == 1 ? civ.advisorD : civ.advisorM;
            if (advisor != null && advisor.active)
            {
                if (Game.main.isMultiplayer)
                {
                    Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.FireAdvisor, advisorCheck);
                }
                else
                {
                    advisor.active = false;
                    civ.RemoveAdvisor(advisor);
                }
            }
            
        }   
        else
        {
            if (balance > 0f)
            {
                if (civ.loans.Count > 0)
                {
                    if (civ.coins >= civ.loans[0].value)
                    {
                        civ.coins -= civ.loans[0].value;
                        civ.loans.RemoveAt(0);
                    }
                }
                
                int advisorCheck = UnityEngine.Random.Range(0, 3);
                Advisor advisor = advisorCheck == 0 ? civ.advisorA : advisorCheck == 1 ? civ.advisorD : civ.advisorM;
                List<Advisor> advisors = advisorCheck == 0 ? civ.advisorsA : advisorCheck == 1 ? civ.advisorsD : civ.advisorsM;
                if (advisor == null || !advisor.active)
                {
                    for (int i = 0; i < advisors.Count;i++)
                    {
                        Advisor ad = advisors[i];
                        if (ad.active)
                        {
                            if (civ.coins >= ad.HireCost(civ) && balance >= ad.Salary(civ))
                            {
                                if (Game.main.isMultiplayer)
                                {
                                    Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.AssignAdvisor, advisorCheck,i);
                                }
                                else
                                {
                                    civ.AssignAdvisor(ad);                                   
                                }
                                balance -= ad.Salary(civ);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (civ.coins >= advisor.HireCost(civ, 1) * 2 && advisor.Salary(civ, 1) - advisor.Salary(civ, 0) < balance)
                    {
                        if (Game.main.isMultiplayer)
                        {
                            Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.PromoteAdvisor, advisorCheck);
                        }
                        else
                        {
                            civ.coins -= advisor.HireCost(civ, 1) * 2;
                            advisor.skillLevel += 1;
                        }                      
                    }
                }

                foreach (var buildingID in GetBestBuildingOrder(civ))
                {
                    Building building = Map.main.Buildings[buildingID];
                    if (building.fortLevel > 0)
                    {
                        if (balance > building.fortLevel && civ.coins >= building.baseCost)
                        {
                            List<TileData> possible = civ.GetAllCivTileDatas();
                            foreach(var possibleItem in possible.ToList())
                            {
                                if(possibleItem.buildings.Contains(buildingID) || possibleItem.buildQueue.Contains(buildingID)) { possible.Remove(possibleItem); }
                                var neighbors = possibleItem.GetNeighborTiles();
                                bool valid = false;
                                foreach (var neighborTile in neighbors)
                                {
                                    if(neighborTile.civID == civ.CivID && (neighborTile.hasFort || neighborTile.buildQueue.Contains(buildingID)))
                                    {
                                        valid = false;
                                        break;
                                    }
                                    if(neighborTile.civID > -1 && neighborTile.civID != civ.CivID && !civ.allies.Contains(neighborTile.civID) && !civ.subjects.Contains(neighborTile.civID))
                                    {
                                        valid = true;
                                    }
                                }
                                if (!valid) { possible.Remove(possibleItem); }
                            }
                            if (possible.Count == 0) { continue; }
                            if (Game.main.isMultiplayer) 
                            {
                                Game.main.multiplayerManager.TileActionRpc(possible[0].pos, MultiplayerManager.TileActions.Build, buildingID);
                            }
                            else
                            {
                                possible[0].StartBuilding(buildingID);
                            }
                        }
                    }
                    else if (civ.coins >= building.baseCost)
                    {
                        List<TileData> possible = civ.GetAllCivTileDatas();
                        possible.RemoveAll(i => i.buildings.Contains(buildingID) || i.buildQueue.Contains(buildingID));
                        if (possible.Count == 0) { continue; }
                        if (building.Name == "Temple" || building.Name == "Cathedral")
                        {
                            possible.Sort((x, y) => y.GetDailyTax().CompareTo(x.GetDailyTax()));
                        }
                        else if (building.Name == "Workshop" || building.Name == "Factory" || building.Name == "Market" || building.Name == "Marketplace")
                        {
                            possible.Sort((x, y) => y.GetDailyProductionValue().CompareTo(x.GetDailyProductionValue()));
                        }
                        else if (building.Name == "Infirmary" || building.Name == "Hospital")
                        {
                            possible.Sort((x, y) => y.populationGrowth.CompareTo(x.populationGrowth));
                        }
                        else if (building.Name == "Town Hall" || building.Name == "City Hall")
                        {
                            possible.Sort((x, y) => y.GetGoverningCost().CompareTo(x.GetGoverningCost()));
                        }
                        else if (building.Name == "School" || building.Name == "University")
                        {
                            possible.Sort((x, y) => y.totalDev.CompareTo(x.totalDev));
                        }
                        if (Game.main.isMultiplayer)
                        {
                            Game.main.multiplayerManager.TileActionRpc(possible[0].pos, MultiplayerManager.TileActions.Build, buildingID);
                        }
                        else
                        {
                            possible[0].StartBuilding(buildingID);
                        }
                    }
                }
            }
        }
    }
    public List<int> GetBestBuildingOrder(Civilisation civ)
    {
        List<int> unlocked = civ.unlockedBuildings.ToList();
        List<int> best = new List<int>()
        {
            4,5,6,7,
            12,13,
            2,3,
            0,1,
            8,9,10,11
        };
        best.RemoveAll(i => !unlocked.Contains(i));
        return best;
    }
    public void TakeIdeaGroups(Civilisation civ)
    {
        if(civ.focusCD == 0)
        {
            int id = -1;
            if(civ.AIAdministrative >= 50f)
            {
                id = 0;
            }
            else if (civ.AIDiplomatic >= 50f)
            {
                id = 1;
            }
            else if (civ.AIMilitary >= 50f)
            {
                id = 2;
            }
            else
            {
                List<WeightedChoice> focus = new List<WeightedChoice>();
                focus.Add(new WeightedChoice(0, (int)civ.AIAdministrative));
                focus.Add(new WeightedChoice(1, (int)civ.AIAdministrative));
                focus.Add(new WeightedChoice(2, (int)civ.AIAdministrative));
                id = WeightedChoiceManager.getChoice(focus).choiceID;
            }
            if (id > -1)
            {
                if (Game.main.isMultiplayer)
                {
                    Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.ChangeFocus, id);
                }
                else
                {
                    civ.focus = id;
                    civ.focusCD = 6;
                }
            }
        }
        List<IdeaGroupData> taken = new List<IdeaGroupData>();
        for (int i = 0; i < civ.unlockedIdeaGroupSlots; i++)
        {
            if (civ.ideaGroups[i] == null || !civ.ideaGroups[i].active)
            {
                List<WeightedChoice> ideaGroups = new List<WeightedChoice>();
                for(int a = 0; a < Map.main.IdeasA.Length;a++)
                {
                    if (taken.Exists(i => i.id == a && i.type == 0)) { continue; }
                    int weight = 100;
                    if (Map.main.IdeasA[a].name == "Economic Ideas") { weight = weight + civ.loans.Count; }
                    if (Map.main.IdeasA[a].name == "Admin Ideas") { weight = (civ.governingCapacity > civ.governingCapacityMax.v ? weight * 3 : 1); }
                    if (Map.main.IdeasA[a].name == "Innovative Ideas") { weight *= civ.GetAllCivTiles().Count < 10 ? 5 : 1; }
                    if (Map.main.IdeasA[a].name == "Infrastructure Ideas") { weight *= civ.GetAllCivTiles().Count < 10 ? 2 : 1; }
                    if (Map.main.IdeasA[a].name == "Humanist Ideas") { weight *= civ.religiousUnity < 0.7f ? 2 : 1; }
                    if (civ.ruler.adminSkill + civ.advisorA.skillLevel + 3 < 7 ) { weight = weight/2; }
                    if (civ.adminTech < civ.diploTech && civ.adminTech < civ.milTech) { weight = weight / 2; }
                    weight = (int)(weight * (1f + civ.AIAdministrative / 100f));
                    ideaGroups.Add(new WeightedChoice(a, weight, "0"));
                }
                for (int a = 0; a < Map.main.IdeasD.Length; a++)
                {
                    if (taken.Exists(i => i.id == a && i.type == 1)) { continue; }
                    int weight = 100;
                    if (Map.main.IdeasD[a].name == "Influence Ideas") { weight = weight * civ.subjects.Count; }
                    if (Map.main.IdeasD[a].name == "Religious Ideas") { weight = (civ.government == 4 ? weight * 2 : weight/2); }
                    if (Map.main.IdeasD[a].name == "Government Ideas") { weight *= (civ.governingCapacity > civ.governingCapacityMax.v ? 2 : 1); }
                    if (Map.main.IdeasD[a].name == "Government Ideas") { weight *= (civ.diplomaticCapacity > civ.diplomaticCapacityMax.v ?  2 : 1); }
                    if (civ.ruler.diploSkill + civ.advisorD.skillLevel + 3 < 7) { weight = weight / 2; }
                    if (civ.diploTech < civ.adminTech && civ.diploTech < civ.milTech) { weight = weight / 2; }
                    weight = (int)(weight * (1f + civ.AIDiplomatic / 100f));
                    ideaGroups.Add(new WeightedChoice(a, weight, "1"));
                }
                for (int a = 0; a < Map.main.IdeasM.Length; a++)
                {
                    if (taken.Exists(i => i.id == a && i.type == 2)) { continue; }
                    int weight = 100;
                    if (Map.main.IdeasM[a].name == "Steppes Ideas") { weight = (civ.government == 3 ? weight * 2 : 0); }
                    if (Map.main.IdeasM[a].name == "General Ideas") { weight = (civ.government == 2 ? weight * 2 : 1); }
                    if (Map.main.IdeasM[a].name == "Quality Ideas") { weight = (civ.discipline.v > 1.1f ? weight * 2 : 1); }
                    if (Map.main.IdeasM[a].name == "Quantity Ideas") { weight = (civ.GetTotalDev() > 300 ? weight * 2 : 1); }
                    if (civ.ruler.milSkill + civ.advisorM.skillLevel + 3 < 7) { weight = weight / 2; }
                    if (civ.milTech < civ.diploTech && civ.milTech < civ.adminTech) { weight = weight / 2; }
                    weight = (int)(weight * (1f + civ.AIMilitary / 100f));
                    ideaGroups.Add(new WeightedChoice(a, weight, "2"));
                }
                if (ideaGroups.Count > 0)
                {
                    WeightedChoice choice = WeightedChoiceManager.getChoice(ideaGroups);
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.SelectIdeaGroup, i,choice.choiceID, int.Parse(choice.choiceName));
                    }
                    else
                    {
                        civ.ideaGroups[i] = new IdeaGroupData(choice.choiceID, int.Parse(choice.choiceName), 0);
                    }
                    
                }
            }
            else
            {
                taken.Add(civ.ideaGroups[i]);
            }
        }     
    }

    public void SpendAdminMana(Civilisation civ)
    {
        int i = civ.CivID;
        List<TileData> tiles = civ.GetAllCivTileDatas();
        tiles.Sort((x,y)=> x.GetDevCost(i).CompareTo(y.GetDevCost(i)));
        bool needCores = false;
        foreach(var tile in tiles)
        {
            if (tile.needsCoring())
            {
                if (civ.adminPower >= tile.GetCoreCost())
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.TileActionRpc(tiles[0].pos, MultiplayerManager.TileActions.CoreConvertStatus, 0);
                    }
                    else
                    {
                        tile.StartCore();
                    }
                    
                }
                else
                {
                    needCores = true;
                }
            }
        }
        if (civ.stability < 0 && !needCores && civ.GetStabilityCost() <= 130f)
        {
            if (civ.adminPower >= civ.GetStabilityCost() && civ.stability < 3)
            {
                if (Game.main.isMultiplayer)
                {
                    Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.SpendAdmin, civ.GetStabilityCost());
                    Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.AddStability, 1);
                }
                else
                {
                    civ.adminPower -= civ.GetStabilityCost();
                    civ.AddStability(1);
                }
                
            }
        }
        tiles.RemoveAll(i => !i.canDev(0));
        int cost = 600;
        float monthDiff = 0;
        if (Map.main.TechA.Length > civ.adminTech + 1)
        {
            Tech tech = Map.main.TechA[civ.adminTech + 1];
            cost = TechnologyUI.GetTechCostNoAhead(tech, civ);
            int realCost = TechnologyUI.GetTechCost(tech, civ);
            monthDiff = (tech.expectedDate.totalTicks() - Game.main.gameTime.totalTicks()) / (6 * 24 * 30);
            if (monthDiff <= 1 || civ.governingCapacity > civ.governingCapacityMax.v)
            {
                if (civ.adminPower >= realCost)
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.BuyTech, 0);
                    }
                    else
                    {
                        tech.TakeTech(i);
                        civ.adminTech++;
                        civ.adminPower -= realCost;
                    }
                }
            }
        }
        else
        {
            monthDiff = 100;
        }
        if (monthDiff > 0)
        {
            for (int j = 0; j < civ.unlockedIdeaGroupSlots; j++)
            {
                if (civ.ideaGroups[j] != null && civ.ideaGroups[j].active)
                {
                    if (civ.ideaGroups[j].type == 0 && civ.ideaGroups[j].unlockedLevel < 7)
                    {
                        if (civ.adminPower >= IdeasUI.GetIdeaCost(civ))
                        {
                            if (Game.main.isMultiplayer)
                            {
                                Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.BuyIdea, j);
                            }
                            else
                            {
                                IdeasUI.BuyIdea(j, civ.ideaGroups[j].unlockedLevel, civ);
                            }
                        }
                    }
                }
            }
        }
        if (monthDiff > 4 || civ.adminPower > 900)
        {
            if ((!needCores && civ.GetWars().Count == 0 && civ.adminPower > Mathf.Min(cost, 600)) || civ.adminPower > 900)
            {
                if (civ.stability < civ.AIAdministrative * 3f)
                {
                    if (civ.adminPower >= civ.GetStabilityCost() && civ.stability < 3)
                    {
                        if (Game.main.isMultiplayer)
                        {
                            Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.SpendAdmin, civ.GetStabilityCost());
                            Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.AddStability, 1);
                        }
                        else
                        {
                            civ.adminPower -= civ.GetStabilityCost();
                            civ.AddStability(1);
                        }
                    }
                }
                else if (tiles.Count > 0 && civ.adminPower >= tiles[0].GetDevCost(i))
                {

                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.TileActionRpc(tiles[0].pos, MultiplayerManager.TileActions.Develop, 0);
                    }
                    else
                    {
                        tiles[0].AddDevelopment(0, i);
                    }
                }
            }
        }                              
    }
    public void SpendDiploMana(Civilisation civ)
    {
        int i = civ.CivID;
        int cost = 600;
        float monthDiff = 0;
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        if (civSubjects.Count > 0 && civ.diploRep.v >= 0)
        {
            civSubjects.Sort((x,y) => x.GetTotalDev().CompareTo(y.GetTotalDev()));
            if (!civSubjects[0].integrating && civSubjects[0].libertyDesire < 50f)
            {
                civSubjects[0].integrating = true;
            }
        }
        if (Map.main.TechD.Length > civ.diploTech + 1)
        {
            Tech tech = Map.main.TechD[civ.diploTech + 1];
            cost = TechnologyUI.GetTechCostNoAhead(tech, civ);
            int realCost = TechnologyUI.GetTechCost(tech, civ);
            monthDiff = (tech.expectedDate.totalTicks() - Game.main.gameTime.totalTicks()) / (6 * 24 * 30);
            if(monthDiff <= 1|| civ.diplomaticCapacity > civ.diplomaticCapacityMax.v)
            {
                if (civ.diploPower >= realCost)
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.BuyTech, 1);
                    }
                    else
                    {
                        tech.TakeTech(i);
                        civ.diploTech++;
                        civ.diploPower -= realCost;
                    }
                }
            }
        }
        else
        {
            monthDiff = 100;
        }
        if (monthDiff > 0)
        {
            for (int j = 0; j < civ.unlockedIdeaGroupSlots; j++)
            {
                if (civ.ideaGroups[j] != null && civ.ideaGroups[j].active)
                {
                    if (civ.ideaGroups[j].type == 1 && civ.ideaGroups[j].unlockedLevel < 7)
                    {
                        if (civ.diploPower >= IdeasUI.GetIdeaCost(civ))
                        {
                            if (Game.main.isMultiplayer)
                            {
                                Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.BuyIdea, j);
                            }
                            else
                            {
                                IdeasUI.BuyIdea(j, civ.ideaGroups[j].unlockedLevel, civ);
                            }
                        }
                    }
                }
            }
        }
        List<TileData> tiles = civ.GetAllCivTileDatas();
        if (civ.avaliableMissionaries > 0)
        {            
            tiles.RemoveAll(i => i.religion == civ.religion && i.isConverting);
            foreach (var tile in tiles)
            {
                if (civ.missionaryStrength.v > tile.GetConvertResistance())
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.TileActionRpc(tiles[0].pos, MultiplayerManager.TileActions.CoreConvertStatus, 1);
                    }
                    else
                    {
                        tile.StartConvert();
                    }
                }
            }
        }
        if (monthDiff > 4 || civ.diploPower > 900)
        {
            tiles = civ.GetAllCivTileDatas();
            tiles.Sort((x, y) => x.GetDevCost(i).CompareTo(y.GetDevCost(i)));
            tiles.RemoveAll(i => !i.canDev(1));
            if (tiles.Count == 0) { return; }
            if (civ.diploPower >= tiles[0].GetDevCost(i) && civ.diploPower > Mathf.Min(cost,600) + tiles[0].GetDevCost(i))
            {
                if (Game.main.isMultiplayer)
                {
                    Game.main.multiplayerManager.TileActionRpc(tiles[0].pos, MultiplayerManager.TileActions.Develop, 1);
                }
                else
                {
                    tiles[0].AddDevelopment(1, i);
                }
            }
        }

    }
    public void SpendMilMana(Civilisation civ)
    {
        int i = civ.CivID;
        List<TileData> tiles = civ.GetAllCivTileDatas();
        tiles.Sort((x, y) => x.GetDevCost(i).CompareTo(y.GetDevCost(i)));
        int cost = 600;
        float monthDiff = 0;
        if (Map.main.TechM.Length > civ.milTech + 1)
        {
            Tech tech = Map.main.TechM[civ.milTech + 1];
            cost = TechnologyUI.GetTechCostNoAhead(tech, civ);
            int realCost = TechnologyUI.GetTechCost(tech, civ);
            monthDiff = (tech.expectedDate.totalTicks() - Game.main.gameTime.totalTicks()) / (6 * 24 * 30);
            if (monthDiff <= 1 || civ.GetWars().Count > 0)
            {
                if (civ.milPower >= realCost)
                {
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.BuyTech, 2);
                    }
                    else
                    {
                        tech.TakeTech(i);
                        civ.milTech++;
                        civ.milPower -= realCost;
                    }
                }
            }
        }
        else
        {
            monthDiff = 100;
        }
        if (monthDiff > 0)
        {
            for (int j = 0; j < civ.unlockedIdeaGroupSlots; j++)
            {
                if (civ.ideaGroups[j] != null && civ.ideaGroups[j].active)
                {
                    if (civ.ideaGroups[j].type == 2 && civ.ideaGroups[j].unlockedLevel < 7)
                    {
                        if (civ.milPower >= IdeasUI.GetIdeaCost(civ))
                        {
                            if (Game.main.isMultiplayer)
                            {
                                Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.BuyIdea, j);
                            }
                            else
                            {
                                IdeasUI.BuyIdea(j, civ.ideaGroups[j].unlockedLevel, civ);
                            }                           
                        }
                    }
                }
            }
        }
        if(civ.generals.Count < 1 + (civ.TotalMaxArmySize()/(1000f * civ.combatWidth.v)) && civ.milPower >= 50)
        {
            if (Game.main.isMultiplayer)
            {
                Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.SpendMil, 50);
            }
            else
            {
                civ.milPower -= 50;
            }
            civ.BuyGeneral();
        }
        if (monthDiff > 4 || civ.milPower > 900)
        {
            tiles.RemoveAll(i => !i.canDev(2));
            if (tiles.Count == 0) { return; }
            if (civ.milPower >= tiles[0].GetDevCost(i) && civ.milPower > Mathf.Min(cost, 600) + tiles[0].GetDevCost(i))
            {
                if (Game.main.isMultiplayer)
                {
                    Game.main.multiplayerManager.TileActionRpc(tiles[0].pos, MultiplayerManager.TileActions.Develop, 2);
                }
                else
                {
                    tiles[0].AddDevelopment(2, i);
                }
            }
        }
    }
    public float GetWarDeclareDesire(Civilisation attacker, Civilisation defender)
    {
        float score = attacker.AIAggressiveness;
        score -= 100f * attacker.GetWars().Count;
        score -= 100f * defender.GetWars().Count;
        score += 200f * ((float)attacker.GetTotalTilePopulation() / (float)attacker.GetTotalMaxPopulation() - 0.8f);
        score += attacker.overextension >= attacker.AIAggressiveness ? -1000f : 0f;
        score += attacker.governingCapacity > attacker.governingCapacityMax.v ? ((attacker.governingCapacity - attacker.governingCapacityMax.v)/attacker.governingCapacityMax.v) * -1000f : 0f;
        score += Mathf.Clamp(attacker.GetBalance(),-100f,100f);
        score += Mathf.Min(attacker.claims.FindAll(i => defender.civTiles.Contains(i)).Count * 5f,20f);
        score += Mathf.Min(attacker.cores.FindAll(i => defender.civTiles.Contains(i)).Count * 20f,100f);
        score += attacker.truces[defender.CivID] > 0 ? -1000f : 0f;
        score += attacker.atWarTogether.Contains(defender.CivID) ? -1000f : 0f;
        score += attacker.atWarWith.Contains(defender.CivID) ? -1000f : 0f;
        score += attacker.overlordID > -1 && attacker.libertyDesire < 50f ? -1000f : 0f;
        score += attacker.overlordID > -1 && attacker.libertyDesire > 50f && defender.CivID == attacker.overlordID ? 20f : 0f;
        score += attacker.overlordID > -1 && defender.CivID != attacker.overlordID ? -1000f : 0f;
        score += attacker.allies.Contains(defender.CivID) ? -1000f : 0f;
        score += attacker.subjects.Contains(defender.CivID) ? -1000f : 0f;
        score += attacker.militaryAccess.Contains(defender.CivID) ? -1000f : 0f;

        float armyStrength = attacker.TotalMilStrength();
        float targetStrength = defender.TotalMilStrength();
        foreach(var war in attacker.GetWars())
        {
            Civilisation target = war.GetOpposingLeader(attacker.CivID);
            List<Civilisation> targetAllies = war.attackerCiv == target ? war.attackerAllies : war.defenderAllies;
            targetStrength += target.TotalMilStrength();
            if (target.subjects.Count > 0)
            {
                foreach (var subject in defender.subjects)
                {
                    Civilisation subjectCiv = Game.main.civs[subject];
                    targetStrength += subjectCiv.TotalMilStrength();
                }
            }
            foreach (var allyCiv in targetAllies)
            {
                targetStrength += allyCiv.TotalMilStrength();
                if (defender.subjects.Count > 0)
                {
                    foreach (var subject in defender.subjects)
                    {
                        Civilisation subjectCiv = Game.main.civs[subject];
                        targetStrength += subjectCiv.TotalMilStrength();
                    }
                }
            }
        }
        if (attacker.subjects.Count > 0)
        {
            foreach (var subject in attacker.subjects)
            {
                Civilisation subjectCiv = Game.main.civs[subject];
                if (DeclareWarPanelUI.CallToArms(defender, attacker, subjectCiv, false) > 0)
                {
                    armyStrength += subjectCiv.TotalMilStrength();
                }
            }
        }
        foreach (var ally in attacker.allies)
        {
            Civilisation allyCiv = Game.main.civs[ally];
            if (DeclareWarPanelUI.CallToArms(defender, attacker, allyCiv, false) > 0)
            {
                armyStrength += allyCiv.TotalMilStrength();

                if (attacker.subjects.Count > 0)
                {
                    foreach (var subject in attacker.subjects)
                    {
                        Civilisation subjectCiv = Game.main.civs[subject];
                        if (DeclareWarPanelUI.CallToArms(defender, attacker, subjectCiv, false) > 0)
                        {
                            armyStrength += subjectCiv.TotalMilStrength();
                        }
                    }
                }
            }
        }
        if (defender.subjects.Count > 0)
        {
            foreach (var subject in defender.subjects)
            {
                Civilisation subjectCiv = Game.main.civs[subject];
                if (DeclareWarPanelUI.CallToArms(attacker, defender, subjectCiv, true) > 0)
                {
                    targetStrength += subjectCiv.TotalMilStrength();
                }
            }
        }
        foreach (var ally in defender.allies)
        {
            Civilisation allyCiv = Game.main.civs[ally];
            if (DeclareWarPanelUI.CallToArms(attacker, defender, allyCiv, true) > 0)
            {
                targetStrength += allyCiv.TotalMilStrength();

                if (defender.subjects.Count > 0)
                {
                    foreach (var subject in defender.subjects)
                    {
                        Civilisation subjectCiv = Game.main.civs[subject];
                        if (DeclareWarPanelUI.CallToArms(attacker, defender, subjectCiv, true) > 0)
                        {
                            targetStrength += subjectCiv.TotalMilStrength();
                        }
                    }
                }
            }
        }
        float strengthRatio = (1f + armyStrength) / (1f + targetStrength);
        score += strengthRatio > 5f ? 100f : 0f;
        score += strengthRatio > 2f ? 10f : 0f;
        score += strengthRatio > (2f - attacker.AIAggressiveness/100f) ? 0f : -1000f;
        return score;
    }
    public void DeclareCivWars()
    {
        int i = Game.main.gameTime.totalTicks()/6 % Game.main.civs.Count;
        Civilisation civ = Game.main.civs[i];
        if (civ.isPlayer || !civ.isActive() || (civ.overlordID > -1 && civ.libertyDesire < 50f) || civ.avaliableDiplomats <= 0) { return; }
        int target = -1;
        float score = -1f;
        foreach (var n in civ.civNeighbours)
        {

            Civilisation neighbor = Game.main.civs[n];
            if (neighbor.overlordID > -1)
            {
                neighbor = Game.main.civs[neighbor.overlordID];
                if (neighbor == civ) { continue; }
            }
            float targetScore = GetWarDeclareDesire(civ, neighbor);
            if(targetScore > score)
            {
                score = targetScore;
                target = neighbor.CivID;
            }
        }
        if (score < 0)
        {
            
            return;
        }
        Civilisation targetCiv = Game.main.civs[target];
        
        if (civ.truces[targetCiv.CivID] == 0)
        {
            List<WarGoal> goals = DeclareWarPanelUI.GetPossibleWarGoals(civ, targetCiv);
            foreach (var goal in goals)
            {
                if (goal.cb.canTakeProvinces)
                {
                    if (Game.main.isMultiplayer)
                    {
                        if (NetworkManager.Singleton.IsServer)
                        {
                            int cbIndex = Map.main.casusBellis.ToList().IndexOf(goal.cb);
                            Game.main.multiplayerManager.DeclareWarRpc(i, target, goal.target, cbIndex);
                        }
                    }
                    else
                    {
                        civ.DeclareWar(targetCiv.CivID, goal.target, goal.cb);
                        civ.deployedDiplomats.Add(new DiplomatStatus(targetCiv, civ, DiplomatAction.Travelling));
                    }
                    break;
                }
            }
            if (goals.Count > 0)
            {
                return;
            }
        }
    }
        
    
    public void OfferAlliances()
    {
        
        int i = Game.main.gameTime.totalTicks() / 6 % Game.main.civs.Count;
        Civilisation civ = Game.main.civs[i];
        if (civ.isPlayer || !civ.isActive() || (civ.overlordID > -1 && civ.libertyDesire < 50f)) { return; }
        if (civ.diplomaticCapacity < civ.diplomaticCapacityMax.v)
        {
            for (int j = UnityEngine.Random.Range(0,10); j < Game.main.civs.Count; j+= 10)
            {
                if (j == i || j == Player.myPlayer.myCivID) { continue; }
                Civilisation target = Game.main.civs[j];
                if (civ.diplomaticCapacity + target.governingCapacity * 1.5f <= civ.diplomaticCapacityMax.v && civ.opinionOfThem[j].v > 0 && target.opinionOfThem[i].v > 0 && civ.religion == target.religion)
                {
                    if (Game.main.isMultiplayer)
                    {
                        if (NetworkManager.Singleton.IsServer)
                        {
                            Game.main.multiplayerManager.CivActionRpc(i, MultiplayerManager.CivActions.Alliance, j);
                        }
                    }
                    else
                    {
                        civ.OfferAlliance(j);
                    }
                }
            }
        }
            
        
    }
    public void UpdateCivWars() 
    {
        if(Game.main.isMultiplayer && !NetworkManager.Singleton.IsServer) { return; }
        for (int i = Time.frameCount % armyMoveUpdateTime; i < Game.main.ongoingWars.Count; i += armyMoveUpdateTime)
        {
            if (i < Game.main.ongoingWars.Count)
            {
                War war = Game.main.ongoingWars[i];
                if (war.warScore == 100 && !war.attackerCiv.isPlayer) 
                {
                    
                    PeaceDeal peaceDeal = PeaceDealUI.Suggested(war.defenderCiv, war);
                    if (Game.main.isMultiplayer)
                    {
                        if (war.networkWar != null)
                        {
                            war.networkWar.EndWarRpc();
                            Game.main.multiplayerManager.SendPeaceDeal(peaceDeal, peaceDeal.target, true);
                        }
                    }
                    else
                    {
                        war.EndWar();
                        war.defenderCiv.AcceptPeaceDeal(peaceDeal, true);
                    }
                    
                    continue; 
                }
                if (war.warScore == -100 && !war.defenderCiv.isPlayer) 
                {

                    
                    PeaceDeal peaceDeal = PeaceDealUI.Suggested(war.attackerCiv, war);
                    if (Game.main.isMultiplayer)
                    {
                        if (war.networkWar != null)
                        {
                            war.networkWar.EndWarRpc();
                            Game.main.multiplayerManager.SendPeaceDeal(peaceDeal, peaceDeal.target, true);
                        }
                    }
                    else
                    {
                        war.EndWar();
                        war.attackerCiv.AcceptPeaceDeal(peaceDeal, true);
                    }
                    
                    continue; 
                }
                if (!war.attackerCiv.isPlayer && !war.defenderCiv.isPlayer)
                {
                    if(war.warScore > 0 && war.defenderCiv.GetWars().Count > 1)
                    {
                        PeaceDeal peaceDeal = PeaceDealUI.Suggested(war.defenderCiv, war);
                        if (PeaceDealUI.WillAccept(peaceDeal,war.defenderCiv,war) && peaceDeal.provinces.Count > 0)
                        {
                            if (Game.main.isMultiplayer)
                            {
                                if (war.networkWar != null)
                                {
                                    war.networkWar.EndWarRpc();
                                    Game.main.multiplayerManager.SendPeaceDeal(peaceDeal, peaceDeal.target, true);
                                }
                            }
                            else
                            {
                                war.EndWar();
                                war.defenderCiv.AcceptPeaceDeal(peaceDeal, true);
                            }
                            
                            continue;
                        }
                    }
                    if(war.warScore < 0 && war.attackerCiv.GetWars().Count > 1)
                    {
                        PeaceDeal peaceDeal = PeaceDealUI.Suggested(war.attackerCiv, war);
                        if (PeaceDealUI.WillAccept(peaceDeal, war.attackerCiv, war) && peaceDeal.provinces.Count > 0)
                        {
                            if (Game.main.isMultiplayer)
                            {
                                if (war.networkWar != null)
                                {
                                    war.networkWar.EndWarRpc();
                                    Game.main.multiplayerManager.SendPeaceDeal(peaceDeal, peaceDeal.target, true);
                                }
                            }
                            else
                            {
                                war.EndWar();
                                war.attackerCiv.AcceptPeaceDeal(peaceDeal, true);
                            }
                            
                            continue;
                        }
                    }                    
                }
                if (war.lengthOfWar > 180)
                {
                    if (!war.attackerCiv.isPlayer)
                    {
                        foreach (var ally in war.defenderAllies.ToList())
                        {
                            if(ally.overlordID > -1) { continue; }
                            PeaceDeal peaceDeal = PeaceDealUI.Suggested(ally, war);
                            if (PeaceDealUI.WillAccept(peaceDeal, ally, war))
                            {
                                if (Game.main.isMultiplayer)
                                {
                                    if (war.networkWar != null)
                                    {
                                        war.networkWar.EndWarRpc(ally.CivID);
                                        Game.main.multiplayerManager.SendPeaceDeal(peaceDeal, peaceDeal.target, false);
                                    }
                                }
                                else
                                {
                                    war.LeaveWar(ally.CivID);
                                    ally.AcceptPeaceDeal(peaceDeal, false);
                                }
                                
                            }
                        }
                    }
                    if (!war.defenderCiv.isPlayer)
                    {
                        foreach (var ally in war.attackerAllies.ToList())
                        {
                            if (ally.overlordID > -1) { continue; }
                            PeaceDeal peaceDeal = PeaceDealUI.Suggested(ally, war);
                            if (PeaceDealUI.WillAccept(peaceDeal, ally, war))
                            {
                                if (Game.main.isMultiplayer)
                                {
                                    if (war.networkWar != null)
                                    {
                                        war.networkWar.EndWarRpc(ally.CivID);
                                        Game.main.multiplayerManager.SendPeaceDeal(peaceDeal, peaceDeal.target, false);
                                    }
                                }
                                else
                                {
                                    war.LeaveWar(ally.CivID);
                                    ally.AcceptPeaceDeal(peaceDeal, false);
                                }
                                
                            }
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
            Game.main.highestIncome = 0f;
            Game.main.highestDevelopment = 0;
            foreach (var civ in Game.main.civs)
            {
                civ.tradeRegions = new bool[Map.main.tradeRegions.Count];           
                civ.civTiles.Clear();
                civ.civTileDatas.Clear();
                civ.civNeighbours.Clear();
                civ.civCoastalTiles.Clear();
                civ.cores.Clear();
                civ.totalIdeas = 0;
                civ.governingCapacity = 0;
                civ.avaliablePopulation = 0;
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
                    if (td.GetDailyTax() + td.GetDailyProductionValue() > Game.main.highestIncome)
                    {
                        Game.main.highestIncome = td.GetDailyTax() + td.GetDailyProductionValue();
                    }
                    Civilisation civ = Game.main.civs[td.civID];
                    foreach(var core in td.cores)
                    {
                        Game.main.civs[core].cores.Add(td.pos);
                    }
                    civ.tradeRegions[td.tradeRegionID] = true;
                    civ.civTiles.Add(td.pos);
                    civ.governingCapacity += td.GetGoverningCost();
                    civ.civTileDatas.Add(td);
                    if (td.isCoastal) { civ.civCoastalTiles.Add(td); }
                    if (!td.occupied && !td.underSiege)
                    {
                        civ.avaliablePopulation += td.avaliablePopulation;
                    }
                    List<Vector3Int> nbs = td.GetNeighbors();
                    foreach (var n in nbs)
                    {
                        TileData ntd = Map.main.GetTile(n);
                        if (ntd.civID != -1 && ntd.civID != td.civID)
                        {
                            if (Game.main.Started && !civ.isPlayer && !civ.subjects.Contains(ntd.civID) && !civ.claims.Contains(ntd.pos) && !civ.allies.Contains(ntd.civID))
                            {
                                if (civ.spyNetwork[ntd.civID] >= 20)
                                {
                                    if (Game.main.isMultiplayer)
                                    {
                                        Game.main.multiplayerManager.CivClaimRpc(civ.CivID, ntd.pos);
                                    }
                                    else
                                    {
                                        civ.spyNetwork[ntd.civID] -= 20;
                                        civ.claims.Add(ntd.pos);
                                    }
                                }
                                else if(civ.avaliableDiplomats > 1 && !civ.deployedDiplomats.Exists(i => i.targetCivId == ntd.civID && i.Action == DiplomatAction.Spying))
                                {
                                    civ.deployedDiplomats.Add(new DiplomatStatus(ntd.civ, civ, DiplomatAction.Spying));
                                }
                            }
                            if (!civ.civNeighbours.Contains(ntd.civID))
                            {
                                civ.civNeighbours.Add(ntd.civID);
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
                if (civ.updateBorders)
                {                   
                    civ.SetupBorderLine();
                    civ.updateBorders = false;
                    if(civ.capitalIndicator == null)
                    {
                        civ.NewCapital(new List<Vector3Int>());
                    }
                }
                float overGovCap = 0f;
                if(civ.governingCapacity > civ.governingCapacityMax.v)
                {
                    overGovCap = (civ.governingCapacity - civ.governingCapacityMax.v)/ civ.governingCapacityMax.v;
                }
                civ.stabilityCost.UpdateModifier("Over Governing Capacity",overGovCap, EffectType.Flat);
                civ.controlDecay.UpdateModifier("Over Governing Capacity", overGovCap /2f, EffectType.Flat);
                civ.improveRelations.UpdateModifier("Over Governing Capacity", -overGovCap/2f, EffectType.Flat);
                civ.aggressiveExpansionImpact.UpdateModifier("Over Governing Capacity", overGovCap / 2f, EffectType.Flat);
                civ.warScoreCost.UpdateModifier("Over Governing Capacity", overGovCap / 2f, EffectType.Flat);
            }
        }

    }
    public void UpdateMoveArmies()
    {
        int i = Time.frameCount % Game.main.civs.Count;
        if (i < Game.main.civs.Count)
        {
            if (Game.main.civs[i].isPlayer) { return; }
            if (Game.main.civs[i].isActive())
            {
                UpdateMoveUnits(i);
            }
        }
        
    }
    public void UpdateMoveUnits(int civID)
    {
        Civilisation civ = Game.main.civs[civID];
        if(civ.atWarWith.Count > 0 && (civ.overlordID == -1 || civ.libertyDesire < 50f))
        {
            AIMoveFleetsWar.MoveAtWar(civID);
            AIMoveArmiesWar.MoveAtWar(civID);
        }
        else
        {
            AIMoveFleetsPeace.MoveAtPeace(civID);
            AIMoveArmiesPeace.MoveAtPeace(civID);
        }
    }
}
