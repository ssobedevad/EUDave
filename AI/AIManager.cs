using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
        if (!Game.main.Started) { return; }
        UpdateMoveArmies();
        UpdateCivWars();
        UpdateMoveRebels();
    }
    public void TenMinTick()
    {
        if (Game.main.gameTime.totalTicks() < 6 * 25) { return; }
        OfferAlliances();
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
            }
        }       
    }
    public void DayTick()
    {
        DeclareCivWars();
    }
    public void UpdateMoveRebels()
    {
        if (Time.frameCount % armyMoveUpdateTime == 1)
        {
            foreach (var rebel in Game.main.rebelFactions)
            {
                if(rebel.path.Count > 0 || rebel.tile.underSiege) { continue; }
                List<Vector3Int> provs = new List<Vector3Int>();
                provs.AddRange(rebel.tile.civ.GetAllCivTiles());
                provs.AddRange(rebel.tile.GetNeighbors());
                provs.RemoveAll(i => Map.main.GetTile(i).occupied);
                provs.RemoveAll(i => Map.main.GetTile(i).civID == -1);
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
    public void TakeGovernmentReform(Civilisation civ)
    {
        if (civ.reformProgress >= civ.reforms.Count * 40 + 40)
        {
            GovernmentType governmentType = Map.main.governmentTypes[civ.government];
            if (governmentType.BaseReforms.Length == 0) { return; }
            GovernmentReformTier tier = governmentType.BaseReforms[civ.reforms.Count];
            int id = UnityEngine.Random.Range(0, tier.Reforms.Length);
            GovernmentReform reform = tier.Reforms[id];
            int cost = (40 + 40 * civ.reforms.Count);
            civ.reformProgress -= cost;
            GovernmentUI.BuyReform(civ, reform);
            civ.reforms.Add(id);                     
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
                    DjinnWorshipUI.DjinnFavor(civ, 2);
                }
                if (civ.GetWars().Count > 0)
                {
                    DjinnWorshipUI.DjinnFavor(civ, 0);
                }
                if (civ.adminPower > 600 || civ.diploPower > 600 || civ.milPower > 600)
                {
                    DjinnWorshipUI.DjinnFavor(civ, 1);
                }
            }
        }
        else if (civ.religion == 2)
        {
            if (civ.religiousPoints == -1)
            {
                LocalDeitiesUI.SelectDeity(civ, UnityEngine.Random.Range(0, Map.main.religions[2].religiousMechanicEffects.Length));
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
                GreatSwampUI.Feed(civ, index);
            }
            else
            {
                if (civ.overextension > 0)
                {
                    GreatSwampUI.Ask(civ, 0);
                }
                if (civ.subjects.Count > 0)
                {
                    if (Game.main.civs[civ.subjects[0]].libertyDesire > 50)
                    {
                        GreatSwampUI.Ask(civ, 1);
                    }
                }
                if (civ.GetWars().Count > 0)
                {
                    GreatSwampUI.Ask(civ, 2);
                }

            }
        }
    }
    public void ManageCoins(Civilisation civ)
    {       
        float balance = civ.GetBalance();
        if (balance < 0)
        {           
            if(civ.TotalMaxArmySize()/1000f > civ.forceLimit.value && civ.atWarWith.Count == 0)
            {
                int armiesToRemove = (int)(civ.TotalMaxArmySize() / 1000f - civ.forceLimit.value);
                foreach (var army in civ.armies)
                {
                    if(army.ArmyMaxSize() <= armiesToRemove)
                    {
                        armiesToRemove -= (int)(army.ArmyMaxSize()/1000f);
                        army.Disband();
                    }
                    else if(army.ArmyMaxSize() > armiesToRemove)
                    {
                        int remain = (int)(army.ArmyMaxSize() / 1000f - armiesToRemove);
                        armiesToRemove -= armiesToRemove;
                        army.SplitOff(remain);
                        army.Disband();
                    }
                    if(armiesToRemove <= 0)
                    {
                        break;
                    }
                }
            }
            if (civ.FortMaintenance() >= 1 && civ.atWarWith.Count == 0 && civ.overlordID == -1)
            {
                List<Vector3Int> forts = civ.GetAllCivTiles().FindAll(i => Map.main.GetTile(i).hasFort);
                for (int i = 0; i < forts.Count; i++)
                {
                    if (balance <= -1)
                    {
                        TileData data = Map.main.GetTile(forts[i]);
                        data.RemoveBuilding(0);
                        balance += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            if(balance > 0f)
            {
                if(civ.loans.Count > 0)
                {
                    if (civ.coins >= civ.loans[0].value)
                    {
                        civ.coins -= civ.loans[0].value;
                        civ.loans.RemoveAt(0);
                    }
                }
               if(civ.advisorA == null || !civ.advisorA.active)
               {
                    foreach(var advisor in civ.advisorsA)
                    {
                        if (advisor.active)
                        {
                            if(civ.coins >= advisor.HireCost(civ) && balance >= advisor.Salary(civ))
                            {
                                civ.AssignAdvisor(advisor);
                                balance -= advisor.Salary(civ);
                                break;
                            }
                        }
                    }
                    
               }
                if (civ.advisorD == null || !civ.advisorD.active)
                {
                    foreach (var advisor in civ.advisorsD)
                    {
                        if (advisor.active)
                        {
                            if (civ.coins >= advisor.HireCost(civ) && balance >= advisor.Salary(civ))
                            {
                                civ.AssignAdvisor(advisor);
                                balance -= advisor.Salary(civ);
                                break;
                            }
                        }
                    }

                }
                if (civ.advisorM == null || !civ.advisorM.active)
                {
                    foreach (var advisor in civ.advisorsM)
                    {
                        if (advisor.active)
                        {
                            if (civ.coins >= advisor.HireCost(civ) && balance >= advisor.Salary(civ))
                            {
                                civ.AssignAdvisor(advisor);
                                balance -= advisor.Salary(civ);
                                break;
                            }
                        }
                    }

                }
            }
            foreach(var buildingID in civ.unlockedBuildings)
            {
                Building building = Map.main.Buildings[buildingID];
                if(building.fortLevel > 0)
                {
                    if(balance > building.fortLevel && civ.coins >= building.baseCost)
                    {
                        List<TileData> possible = civ.GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
                        possible.RemoveAll(i => i.buildings.Contains(buildingID) || i.buildQueue.Contains(buildingID));
                        if (possible.Count == 0) { continue; }
                        possible.Sort((x, y) => y.hasZOC.CompareTo(x.hasZOC));
                        possible[0].StartBuilding(buildingID);
                    }
                }
                else if(civ.coins >= building.baseCost)
                {
                    List<TileData> possible = civ.GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
                    possible.RemoveAll(i => i.buildings.Contains(buildingID) || i.buildQueue.Contains(buildingID));
                    if(possible.Count == 0) { continue; }
                    if (building.Name == "Temple")
                    {
                        possible.Sort((x, y) => y.GetDailyTax().CompareTo(x.GetDailyTax()));
                    }
                    else if (building.Name == "Workshop" || building.Name == "Market")
                    {
                        possible.Sort((x, y) => y.GetDailyProductionValue().CompareTo(x.GetDailyProductionValue()));                    
                    }
                    possible[0].StartBuilding(buildingID);
                }
            }
            
        }
    }
    public void TakeIdeaGroups(Civilisation civ)
    {
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
                    if(Map.main.IdeasA[a].name == "Economic Ideas") { weight = weight += civ.loans.Count; }
                    if (civ.ruler.adminSkill + civ.advisorA.skillLevel + 3 < 7) { weight = weight/2; }
                    ideaGroups.Add(new WeightedChoice(a, weight, "0"));
                }
                for (int a = 0; a < Map.main.IdeasD.Length; a++)
                {
                    if (taken.Exists(i => i.id == a && i.type == 1)) { continue; }
                    int weight = 100;
                    if (Map.main.IdeasD[a].name == "Influence Ideas") { weight = weight * civ.subjects.Count; }
                    if (Map.main.IdeasD[a].name == "Religious Ideas") { weight = (civ.government == 4 ? weight * 2 : weight/2); }
                    if (civ.ruler.diploSkill + civ.advisorD.skillLevel + 3 < 7) { weight = weight / 2; }
                    ideaGroups.Add(new WeightedChoice(a, weight, "1"));
                }
                for (int a = 0; a < Map.main.IdeasM.Length; a++)
                {
                    if (taken.Exists(i => i.id == a && i.type == 2)) { continue; }
                    int weight = 100;
                    if (Map.main.IdeasM[a].name == "Steppes Ideas") { weight = (civ.government == 3 ? weight * 2 : 0); }
                    if (civ.ruler.milSkill + civ.advisorM.skillLevel + 3 < 7) { weight = weight / 2; }
                    ideaGroups.Add(new WeightedChoice(a, weight, "2"));
                }
                if (ideaGroups.Count > 0)
                {
                    WeightedChoice choice = WeightedChoiceManager.getChoice(ideaGroups);
                    civ.ideaGroups[i] = new IdeaGroupData(choice.choiceID, int.Parse(choice.choiceName), 0);
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
                    tile.StartCore();
                }
                else
                {
                    needCores = true;
                }
            }
        }
        if (civ.stability < 0 && !needCores && civ.overextension <= 10f)
        {
            if (civ.adminPower >= civ.GetStabilityCost() && civ.stability < 3)
            {
                civ.adminPower -= civ.GetStabilityCost();
                civ.AddStability(1);
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
            if (monthDiff <= 1)
            {
                if (civ.adminPower >= realCost)
                {
                    tech.TakeTech(i);
                    civ.adminTech++;
                    civ.adminPower -= realCost;
                }
            }
        }
        else
        {
            monthDiff = 100;
        }
        for (int j = 0; j < civ.unlockedIdeaGroupSlots; j++)
        {
            if (civ.ideaGroups[j] != null && civ.ideaGroups[j].active)
            {
                if (civ.ideaGroups[j].type == 0 && civ.ideaGroups[j].unlockedLevel < 7)
                {
                    if (civ.adminPower >= IdeasUI.GetIdeaCost(civ))
                    {
                        IdeasUI.BuyIdea(j, civ.ideaGroups[j].unlockedLevel, civ);
                    }
                }
            }
        }
        if (monthDiff > 4 || civ.adminPower > 900)
        {
            if (!needCores && civ.GetWars().Count == 0 && civ.adminPower > cost)
            {
                if (civ.stability < 1)
                {
                    if (civ.adminPower >= civ.GetStabilityCost() && civ.stability < 3)
                    {
                        civ.adminPower -= civ.GetStabilityCost();
                        civ.AddStability(1);
                    }
                }
                else if (tiles.Count > 0 && civ.adminPower >= tiles[0].GetDevCost(i))
                {
                    tiles[0].AddDevelopment(0, i);
                }
            }
        }                              
    }
    public void SpendDiploMana(Civilisation civ)
    {
        int i = civ.CivID;
        int cost = 600;
        float monthDiff = 0;
        if (Map.main.TechD.Length > civ.diploTech + 1)
        {
            Tech tech = Map.main.TechD[civ.diploTech + 1];
            cost = TechnologyUI.GetTechCostNoAhead(tech, civ);
            int realCost = TechnologyUI.GetTechCost(tech, civ);
            monthDiff = (tech.expectedDate.totalTicks() - Game.main.gameTime.totalTicks()) / (6 * 24 * 30);
            if(monthDiff <= 1)
            {
                if (civ.diploPower >= realCost)
                {
                    tech.TakeTech(i);
                    civ.diploTech++;
                    civ.diploPower -= realCost; 
                }
            }
        }
        else
        {
            monthDiff = 100;
        }
        for (int j = 0; j < civ.unlockedIdeaGroupSlots; j++)
        {
            if (civ.ideaGroups[j] != null && civ.ideaGroups[j].active)
            {
                if (civ.ideaGroups[j].type == 1 && civ.ideaGroups[j].unlockedLevel < 7)
                {
                    if (civ.diploPower >= IdeasUI.GetIdeaCost(civ))
                    {
                        IdeasUI.BuyIdea(j, civ.ideaGroups[j].unlockedLevel, civ);
                    }
                }
            }
        }
        List<TileData> tiles = civ.GetAllCivTileDatas();
        tiles.RemoveAll(i => i.religion == civ.religion && i.control <= i.GetConvertControl() && i.religionTimer > -1);
        foreach (var tile in tiles)
        {
            if (civ.diploPower >= tile.GetConvertCost())
            {
                tile.StartConvert();
            }
        }
        if (monthDiff > 4 || civ.diploPower > 900)
        {
            tiles = civ.GetAllCivTileDatas();
            tiles.Sort((x, y) => x.GetDevCost(i).CompareTo(y.GetDevCost(i)));
            tiles.RemoveAll(i => !i.canDev(1));
            if (tiles.Count == 0) { return; }
            if (civ.diploPower >= tiles[0].GetDevCost(i) && civ.diploPower > cost + tiles[0].GetDevCost(i))
            {
                tiles[0].AddDevelopment(1, i);
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
            if (monthDiff <= 1)
            {
                if (civ.milPower >= realCost)
                {
                    tech.TakeTech(i);
                    civ.milTech++;
                    civ.milPower -= realCost;
                }
            }
        }
        else
        {
            monthDiff = 100;
        }
        for (int j = 0; j < civ.unlockedIdeaGroupSlots; j++)
        {
            if (civ.ideaGroups[j] != null && civ.ideaGroups[j].active)
            {
                if (civ.ideaGroups[j].type == 2 && civ.ideaGroups[j].unlockedLevel < 7)
                {
                    if (civ.milPower >= IdeasUI.GetIdeaCost(civ))
                    {
                        IdeasUI.BuyIdea(j, civ.ideaGroups[j].unlockedLevel, civ);
                    }
                }
            }
        }
        if(civ.generals.Count < 1 + (civ.TotalMaxArmySize()/(1000f * civ.combatWidth.value)) && civ.milPower >= 50)
        {
            civ.milPower -= 50;
            civ.BuyGeneral();
        }
        if (monthDiff > 4 || civ.milPower > 900)
        {
            tiles.RemoveAll(i => !i.canDev(2));
            if (tiles.Count == 0) { return; }
            if (civ.milPower >= tiles[0].GetDevCost(i) && civ.milPower > cost + tiles[0].GetDevCost(i))
            {
                tiles[0].AddDevelopment(2, i);
            }
        }
    }
    public void DeclareCivWars()
    {
        for (int i = (int)(Game.main.gameTime.totalTicks()/144f) % armyMoveUpdateTime; i < Game.main.civs.Count; i += armyMoveUpdateTime)
        {
            if (i < Game.main.civs.Count )
            {
                Civilisation civ = Game.main.civs[i];
                if (civ.isPlayer || !civ.isActive() || (civ.overlordID > -1 && civ.libertyDesire < 50f)) { continue; }
                if(civ.atWarWith.Count == 0 && civ.GetTotalTilePopulation() > civ.GetTotalMaxPopulation() * 0.75f && civ.overextension <= 50f && civ.GetBalance() > 0)
                {
                    foreach (var n in WeightedChoiceManager.Shuffle(civ.civNeighbours))
                    {
                        Civilisation neighbor = Game.main.civs[n];
                        if (neighbor.overlordID > -1)
                        {
                            neighbor = Game.main.civs[neighbor.overlordID];       
                            if(neighbor == civ) { continue; }
                        }
                        float armyStrength = civ.TotalMilStrength();
                        float targetStrength = neighbor.TotalMilStrength();
                        if (civ.subjects.Count > 0)
                        {
                            foreach (var subject in civ.subjects)
                            {
                                Civilisation subjectCiv = Game.main.civs[subject];
                                if (DeclareWarPanelUI.CallToArms(neighbor, civ, subjectCiv, false) > 0)
                                {
                                    armyStrength += subjectCiv.TotalMilStrength();
                                }
                            }
                        }
                        foreach (var ally in civ.allies)
                        {
                            Civilisation allyCiv = Game.main.civs[ally];
                            if (DeclareWarPanelUI.CallToArms(neighbor, civ, allyCiv, false) > 0)
                            {
                                armyStrength += allyCiv.TotalMilStrength();

                                if (civ.subjects.Count > 0)
                                {
                                    foreach (var subject in civ.subjects)
                                    {
                                        Civilisation subjectCiv = Game.main.civs[subject];
                                        if (DeclareWarPanelUI.CallToArms(neighbor, civ, subjectCiv, false) > 0)
                                        {
                                            armyStrength += subjectCiv.TotalMilStrength();
                                        }
                                    }
                                }
                            }
                        }


                        if (neighbor.subjects.Count > 0)
                        {
                            foreach (var subject in neighbor.subjects)
                            {
                                Civilisation subjectCiv = Game.main.civs[subject];
                                if (DeclareWarPanelUI.CallToArms(civ, neighbor, subjectCiv, true) > 0)
                                {
                                    targetStrength += subjectCiv.TotalMilStrength();
                                }
                            }
                        }
                        foreach (var ally in neighbor.allies)
                        {
                            Civilisation allyCiv = Game.main.civs[ally];
                            if (DeclareWarPanelUI.CallToArms(civ, neighbor, allyCiv, true) > 0)
                            {
                                targetStrength += allyCiv.TotalMilStrength();

                                if (neighbor.subjects.Count > 0)
                                {
                                    foreach (var subject in neighbor.subjects)
                                    {
                                        Civilisation subjectCiv = Game.main.civs[subject];
                                        if (DeclareWarPanelUI.CallToArms(civ, neighbor, subjectCiv, true) > 0)
                                        {
                                            targetStrength += subjectCiv.TotalMilStrength();
                                        }
                                    }
                                }
                            }
                        }

                        if (civ.truces[neighbor.CivID] == 0 && targetStrength < armyStrength * 2f)
                        {
                            List<WarGoal> goals = DeclareWarPanelUI.GetPossibleWarGoals(civ, neighbor);
                            foreach (var goal in goals)
                            {
                                if (goal.cb.canTakeProvinces)
                                {
                                    civ.DeclareWar(n, goal.target, goal.cb);
                                    break;
                                }
                            }
                            if (goals.Count > 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    public void OfferAlliances()
    {
        for (int i = Time.frameCount % armyMoveUpdateTime; i < Game.main.civs.Count; i += armyMoveUpdateTime)
        {
            if (i < Game.main.civs.Count)
            {
                Civilisation civ = Game.main.civs[i];
                if (civ.isPlayer || !civ.isActive() || (civ.overlordID > -1 && civ.libertyDesire < 50f)) { continue; }
                if (civ.remainingDiploRelations > 0)
                {
                    for (int j = UnityEngine.Random.Range(0,10); j < Game.main.civs.Count; j+= 10)
                    {
                        if (j == i || j == Player.myPlayer.myCivID) { continue; }
                        Civilisation target = Game.main.civs[j];
                        if (civ.opinionOfThem[j].value > 20 && target.opinionOfThem[i].value > 20 && civ.religion == target.religion)
                        {
                            civ.OfferAlliance(j);
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
                    //Debug.Log("Peace Out Defender By 100% " + war.GetName());
                    war.EndWar();
                    war.defenderCiv.AcceptPeaceDeal(PeaceDealUI.Suggested(war.defenderCiv, war),true);
                    continue; 
                }
                if (war.warScore == -100 && !war.defenderCiv.isPlayer) 
                {
                    
                    //Debug.Log("Peace Out Attacker By 100% " + war.GetName());
                    war.EndWar();
                    war.attackerCiv.AcceptPeaceDeal(PeaceDealUI.Suggested(war.attackerCiv, war), true);
                    continue; 
                }
                if (!war.attackerCiv.isPlayer && !war.defenderCiv.isPlayer)
                {
                    if(war.warScore >= 100f - (float)war.lengthOfWar/180f)
                    {
                        PeaceDeal peaceDeal = PeaceDealUI.Suggested(war.defenderCiv, war);
                        if (PeaceDealUI.WillAccept(peaceDeal,war.defenderCiv,war))
                        {
                            
                            //Debug.Log("Peace Out Defender By Time " + war.GetName());
                            war.EndWar();
                            war.defenderCiv.AcceptPeaceDeal(peaceDeal, true);
                            continue;
                        }
                    }
                    if(war.warScore <= -100f + (float)war.lengthOfWar / 180f)
                    {
                        PeaceDeal peaceDeal = PeaceDealUI.Suggested(war.attackerCiv, war);
                        if (PeaceDealUI.WillAccept(peaceDeal, war.attackerCiv, war))
                        {
                            
                            //Debug.Log("Peace Out Attacker By Time " + war.GetName());
                            war.EndWar();
                            war.attackerCiv.AcceptPeaceDeal(peaceDeal, true);
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
                                //Debug.Log("Peace Out Defender Ally By Time " + war.GetName());
                                war.LeaveWar(ally.CivID);
                                ally.AcceptPeaceDeal(peaceDeal, false);
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
                                //Debug.Log("Peace Out Attacker Ally By Time " + war.GetName());
                                war.LeaveWar(ally.CivID);
                                ally.AcceptPeaceDeal(peaceDeal, false);
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
                civ.tradeRegions.Clear();
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
                    if (!civ.civTiles.Contains(td.pos))
                    {
                        if (!civ.tradeRegions.Contains(td.tradeRegion))
                        {
                            civ.tradeRegions.Add(td.tradeRegion);
                        }
                        civ.civTiles.Add(td.pos);
                        civ.governingCapacity += td.GetGoverningCost();
                        civ.civTileDatas.Add(td);
                        if (td.isCoastal) { civ.civCoastalTiles.Add(td); }
                        if (!td.occupied && !td.underSiege)
                        {
                            civ.avaliablePopulation += td.avaliablePopulation;
                        }
                        List<Vector3Int> nbs = td.GetNeighbors();
                        foreach(var n in nbs)
                        {
                            TileData ntd = Map.main.GetTile(n);
                            if(ntd.civID != -1 && ntd.civID != td.civID)
                            {
                                if (Game.main.Started && !civ.isPlayer && !civ.subjects.Contains(ntd.civID) &&!civ.claims.Contains(ntd.pos) && !civ.allies.Contains(ntd.civID))
                                {
                                    int dev = ntd.totalDev;
                                    if (civ.diploPower >= dev)
                                    {
                                        civ.diploPower -= dev;
                                        civ.claims.Add(ntd.pos);
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
                    civ.SetupCountryName();
                    civ.updateBorders = false;
                    if(civ.capitalIndicator == null)
                    {
                        civ.NewCapital(new List<Vector3Int>());
                    }
                }
                float overGovCap = 0f;
                if(civ.governingCapacity > civ.governingCapacityMax.value)
                {
                    overGovCap = (civ.governingCapacity - civ.governingCapacityMax.value)/ civ.governingCapacityMax.value;
                }
                civ.stabilityCost.UpdateModifier("Over Governing Capacity",overGovCap,1);
                civ.advisorCosts.UpdateModifier("Over Governing Capacity", overGovCap, 1);
                civ.improveRelations.UpdateModifier("Over Governing Capacity", -overGovCap/2f, 1);
                civ.aggressiveExpansionImpact.UpdateModifier("Over Governing Capacity", overGovCap / 2f, 1);
                civ.warScoreCost.UpdateModifier("Over Governing Capacity", overGovCap / 2f, 1);
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
            AIMoveArmiesWar.MoveAtWar(civID);
        }
        else
        {
            AIMoveArmiesPeace.MoveAtPeace(civID);
        }
    }
}
