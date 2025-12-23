using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    public NetworkVariable<int> currentFrame = new NetworkVariable<int>(0);
    int completedFrames = 0;
    public NetworkVariable<civID> playerCivs = new(new civID { values = new int[1] { -1 } });
    [SerializeField] public NetworkObject netArmyPrefab,netBattlePrefab,netFleetPrefab,netNavalBattlePrefab,netWarPrefab;
    public override void OnNetworkSpawn()
    {
        Game.main.multiplayerManager = this;
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        if (!NetworkManager.Singleton.IsServer)
        {           
            return;
        }
        int count = NetworkManager.Singleton.ConnectedClients.Count;
        int[] civs = playerCivs.Value.values;
        int[] newCivs = new int[count];
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            if (i < civs.Length)
            {
                newCivs[i] = civs[i];
            }
            else
            {
                newCivs[i] = -1;
            }
        }
        playerCivs.Value = new civID { values = newCivs };
    }

    public struct civID : INetworkSerializable
    {
        public int[] values;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref values);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void StartGameClientRpc()
    {
        int[] civs = playerCivs.Value.values;
        for (int i = 0; i < civs.Length; i++)
        {
            if (civs[i] > -1) 
            {
                if ((int)NetworkManager.Singleton.LocalClientId == i)
                {
                    Player.myPlayer.myCivID = civs[i];
                }
                Game.main.civs[civs[i]].isPlayer = true;
            }
        }
        Game.main.StartGame();
        UIManager.main.saveGames.SetActive(false);
        UIManager.main.settings.SetActive(false);
        UIManager.main.startGameButton.SetActive(false);
    }
    [Rpc(SendTo.Server)]
    public void TogglePauseRpc()
    {
        Game.main.paused = !Game.main.paused;
        SetPauseRpc(Game.main.paused);
    }
    [Rpc(SendTo.NotServer)]
    void SetPauseRpc(bool paused)
    {
        Game.main.paused = paused;
    }
    [Rpc(SendTo.Server)]
    public void ChangeSpeedValRpc(int value)
    {
        Game.main.gameSpeed = value;
        ChangeSpeedValClientRpc(value);
    }
    [Rpc(SendTo.NotServer)]
    void ChangeSpeedValClientRpc(int value)
    {
        Game.main.gameSpeed = value;
    }
    [Rpc(SendTo.Server)]
    public void CompletedFrameRpc()
    {
        completedFrames += 1;
        if(completedFrames >= NetworkManager.Singleton.ConnectedClients.Count)
        {
            completedFrames = 0;
            currentFrame.Value += 1;
        }
    }


    [Rpc(SendTo.Server)]
    public void SelectCivRpc(int newCiv, int playerID)
    {        
        if (newCiv > -1 && playerCivs.Value.values.Contains(newCiv)) { return; }
        playerCivs.Value.values[playerID] = newCiv;
        playerCivs.SetDirty(true);
        SelectCivClientRpc(newCiv, playerID);
    }

    [Rpc(SendTo.NotServer)]
    public void SelectCivClientRpc(int newCiv, int playerID)
    {
        if((int)NetworkManager.Singleton.LocalClientId == playerID)
        {
            Player.myPlayer.myCivID = newCiv;
        }
    }

    [Rpc(SendTo.Server)]
    public void SetPathRpc(Vector3Int armyTile,int tileindex, Vector3Int[] path)
    {
        TileData tile = Map.main.GetTile(armyTile);
        if(tile.armiesOnTile.Count <= tileindex) { return; }
        Army army = tile.armiesOnTile[tileindex];
        army.path = path.ToList();
        SetPathClientRpc(armyTile,tileindex, path);
    }
    [Rpc(SendTo.NotServer)]
    void SetPathClientRpc(Vector3Int armyTile, int tileindex, Vector3Int[] path)
    {
        TileData tile = Map.main.GetTile(armyTile);
        if (tile.armiesOnTile.Count <= tileindex) { return; }
        Army army = tile.armiesOnTile[tileindex];
        army.path = path.ToList();     
    }

    [Rpc(SendTo.Server)]
    public void NewUnitRpc(Vector3Int tilePos, int type, int mode)
    {
        TileData tile = Map.main.GetTile(tilePos);
        if(tile.civID == -1) { return; }
        if(mode == 0)
        {
            NetworkArmy netArmy = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(netArmyPrefab,position:tile.worldPos()).GetComponent<NetworkArmy>();
            netArmy.pos = tilePos;
            netArmy.civID = tile.civID;
            List<Regiment> regiments = new List<Regiment> { new Regiment(CivID: netArmy.civID, Type: type) };
            netArmy.regiments = regiments.ToArray();
            netArmy.init = true;
        }
        else if (mode == 1)
        {
            NetworkArmy netArmy = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(netArmyPrefab, position: tile.worldPos()).GetComponent<NetworkArmy>();
            netArmy.pos = tilePos;
            MercenaryGroup merc = Map.main.mercenaries[type];            
            List<Regiment> regiments = new List<Regiment>();
            int regimentCount = merc.baseRegiments + merc.regimentsPerYearExtra * Game.main.gameTime.y;
            int cavCount = (int)(regimentCount * merc.cavalryPercent);
            for (int i = 0; i < regimentCount; i++)
            {
                regiments.Add(new Regiment(tile.civID, 1000, 1000, i >= cavCount ? 0 : 1, true));
            }
            netArmy.civID = tile.civID;
            netArmy.regiments = regiments.ToArray();
            netArmy.init = true;
        }
        else if (mode == 2)
        {
            NetworkFleet netFleet = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(netFleetPrefab, position: tile.worldPos()).GetComponent<NetworkFleet>();
            netFleet.pos = tilePos;
            netFleet.civID = tile.civID;
            List<Boat> boats = new List<Boat> { new Boat(CivID: netFleet.civID, Type: type) };
            netFleet.boats = boats.ToArray();
            netFleet.init = true;
        }
    }

    [Rpc(SendTo.Server)]
    public void TileActionRpc(Vector3Int tilePos, TileActions actionId, int action)
    {
        TileData tile = Map.main.GetTile(tilePos);
        TileAction(tile, actionId, action);
        TileActionClientRpc(tilePos, actionId, action);
    }
    [Rpc(SendTo.NotServer)]
    void TileActionClientRpc(Vector3Int tilePos, TileActions actionId, int action)
    {
        TileData tile = Map.main.GetTile(tilePos);
        TileAction(tile, actionId, action);
    }
    void TileAction(TileData tile, TileActions actionId,int action)
    {
        switch (actionId)
        {
            case TileActions.Develop:
                {
                    tile.AddDevelopment(action);
                    break;
                }
            case TileActions.CoreConvertStatus:
                {
                    if (action == 0)
                    {
                        tile.StartCore();
                    }
                    else if (action == 1)
                    {
                        tile.StartConvert();
                    }
                    else if (action == 2)
                    {
                        tile.PromoteStatus();
                    }
                    break;
                }
            case TileActions.Recruit:
                {
                    tile.StartRecruiting(action);
                    break;
                }
            case TileActions.RecruitBoat:
                {
                    tile.StartRecruitingBoat(action);
                    break;
                }
            case TileActions.RecruitMerc:
                {
                    tile.StartRecruitingMercenary(action);
                    break;
                }
            case TileActions.Build:
                {
                    tile.StartBuilding(action);
                    break;
                }
            case TileActions.Occupy:
                {
                    tile.TransferOccupation(action);
                    break;
                }
            case TileActions.OccupyIntegrated:
                {
                    tile.TransferOccupation(action,true);
                    break;
                }
            case TileActions.ChangeControl:
                {
                    tile.ChangeControl(action == 0);
                    break;
                }
            case TileActions.RemoveBuilding:
                {
                    tile.RemoveBuilding(action);
                    break;
                }
            case TileActions.UpgradeGreatProj:
                {
                    Civilisation civ = Game.main.civs[action];
                    if (tile.greatProject != null)
                    {
                        float cost = tile.greatProject.GetCost(tile, civ);
                        if (civ.coins >= cost)
                        {
                            tile.greatProject.isBuilding = true;
                            civ.coins -= cost;
                        }
                    }
                    break;
                }
            case TileActions.MoveCapital:
                {
                    if (tile.civID > -1)
                    {
                        tile.civ.MoveCapitalToSaveGame(tile.pos);
                    }
                    break;
                }
            case TileActions.ChangeInfrastructure:
                {
                    if(tile.civID  == -1) { return; }
                    bool up = action == 0;
                    Civilisation civ = Game.main.civs[tile.civID];
                    if (up)
                    {
                        if (tile.infrastructureLevel < tile.totalDev / 15 && civ.adminPower >= 50)
                        {
                            civ.adminPower -= 50;
                            tile.infrastructureLevel++;
                        }
                    }
                    else
                    {
                        if (tile.infrastructureLevel > 0)
                        {
                            tile.infrastructureLevel--;
                        }
                    }
                    tile.UpdateInfrastructureModifiers();
                    break;
                }
        }
    }

    public enum TileActions
    {
        Develop,
        CoreConvertStatus,
        Recruit,
        Build,
        Occupy,
        OccupyIntegrated,
        RecruitBoat,
        RecruitMerc,
        ChangeControl,
        RemoveBuilding,
        UpgradeGreatProj,
        MoveCapital,
        ChangeInfrastructure,
    }

    [Rpc(SendTo.Server)]
    public void CivActionRpc(int civId, CivActions actionId, int action)
    {
        Civilisation civ = Game.main.civs[civId];
        CivAction(civ, actionId, action);
        //SyncCivResourcesRpc(civId);
        CivActionClientRpc(civId, actionId, action);
    }
    [Rpc(SendTo.NotServer)]
    void CivActionClientRpc(int civId, CivActions actionId, int action)
    {
        Civilisation civ = Game.main.civs[civId];
        CivAction(civ, actionId, action);
    }
    void CivAction(Civilisation civ, CivActions actionId, int action)
    {
        switch (actionId)
        {
            case CivActions.Subjugate:
                {
                    Civilisation target = Game.main.civs[action];
                    target.overlordID = -1;
                    civ.Subjugate(target);
                    break;
                }
            case CivActions.RemoveSubjugation:
                {
                    civ.RemoveSubjugation();
                    break;
                }
            case CivActions.Alliance:
                {
                    civ.OfferAlliance(action);
                    break;
                }
            case CivActions.BreakAlliance:
                {
                    civ.BreakAlliance(action);
                    break;
                }
            case CivActions.SetRival:
                {
                    civ.SetRival(action);
                    break;
                }
            case CivActions.BuyIdea:
                {
                    IdeasUI.BuyIdea(action, civ.ideaGroups[action].unlockedLevel, civ);
                    break;
                }
            case CivActions.BuyTech:
                {
                    Tech tech = action == 0 ? Map.main.TechA[civ.adminTech + 1] : action == 1 ? Map.main.TechD[civ.diploTech + 1] : Map.main.TechM[civ.milTech + 1];
                    int realCost = TechnologyUI.GetTechCost(tech, civ);
                    tech.TakeTech(civ.CivID);
                    if (action == 0)
                    {
                        civ.adminTech++;
                        civ.adminPower -= realCost;
                    }
                    else if(action == 1)
                    {
                        civ.diploTech++;
                        civ.diploPower -= realCost;
                    }
                    else if (action == 2)
                    {
                        civ.milTech++;
                        civ.milPower -= realCost;
                    }
                    break;
                }
            case CivActions.DisbandArmy:
                {
                    foreach (var army in civ.armies.ToList())
                    {
                        army.DisbandMercs();
                        if (army.ArmyMaxSize() / 1000f <= action)
                        {
                            action -= (int)(army.ArmyMaxSize() / 1000f);
                            army.Disband();
                        }
                        else if (army.ArmyMaxSize() / 1000f > action)
                        {
                            int remain = (int)(army.ArmyMaxSize() / 1000f - action);
                            action = 0;
                            army.SplitOff(remain);
                            army.Disband();
                        }
                        if (action <= 0)
                        {
                            break;
                        }
                    }
                    break;
                }
            case CivActions.RulerDeath:
                {
                    if(action == 0)
                    {
                        civ.ruler.Kill();
                    }
                    else
                    {
                        civ.heir.Kill();
                    }
                    break;
                }
            case CivActions.RulerTrait:
                {
                    civ.ruler.AddTrait(action);
                    break;
                }
            case CivActions.HeirTrait:
                {
                    civ.heir.AddTrait(action);
                    break;
                }
            case CivActions.PickReform:
                {
                    civ.reforms.Add(action);
                    break;
                }
            case CivActions.AddStability:
                {
                    civ.AddStability(action);
                    break;
                }
            case CivActions.SpendAdmin:
                {
                    civ.adminPower -= action;
                    break;
                }
            case CivActions.SpendDiplo:
                {
                    civ.diploPower -= action;
                    break;
                }
            case CivActions.SpendMil:
                {
                    civ.milPower -= action;
                    break;
                }
            case CivActions.GeneralDeath:
                {
                    civ.generals.RemoveAt(action);
                    break;
                }
            case CivActions.RemoveIdeaGroup:
                {
                    IdeaGroupData ideaGroup = civ.ideaGroups[action];
                    if (ideaGroup.active)
                    {
                        ideaGroup.active = false;
                        int points = ideaGroup.unlockedLevel * 40;
                        if (ideaGroup.type == 0) { civ.adminPower += points; }
                        else if (ideaGroup.type == 1) { civ.diploPower += points; }
                        else { civ.milPower += points; }
                    }
                    break;
                }
            case CivActions.RecieveEvent:
                {
                    EventData ev = Map.main.pulseEvents[civ.eventHistory.Last()];
                    EventManager.TakeOption(ev, ev.optionEffects[action], civ);
                    break;
                }
            case CivActions.Bankruptcy:
                {
                    civ.DeclareBankruptcy();
                    break;
                }
            case CivActions.ReleaseSubject:
                {
                    Civilisation released = Game.main.civs[action];
                    ReleaseSubjectUI.ReleaseCiv(civ,released);
                    break;
                }
            case CivActions.ChangeFocus:
                {
                    civ.focus = action;
                    civ.focusCD = 6;
                    break;
                }
            case CivActions.AbdicateDisinherit:
                {
                    if (action == 0)
                    {
                        civ.ruler.active = false;
                        civ.AddPrestige(-25);
                    }
                    else
                    {
                        civ.heir.active = false;
                        civ.AddPrestige(-50);
                    }
                    break;
                }
        }
    }
    public enum CivActions
    {
        Subjugate,
        Alliance,
        BreakAlliance,
        SetRival,
        BuyIdea,
        BuyTech,
        DisbandArmy,
        RulerDeath,
        RulerTrait,
        HeirTrait,
        PickReform,
        AddStability,
        SpendAdmin,
        SpendDiplo,
        SpendMil,
        GeneralDeath,
        RemoveIdeaGroup,
        RecieveEvent,
        Bankruptcy,
        ReleaseSubject,
        ChangeFocus,
        AbdicateDisinherit,
        ChangePop,
        RemoveSubjugation,
    }


    [Rpc(SendTo.Server)]
    public void SyncCivResourcesRpc(int civId)
    {
        Civilisation civ = Game.main.civs[civId];
        SyncCivResourcesClientRpc(civId, civ.adminPower, civ.diploPower, civ.milPower, civ.coins);
    }
    [Rpc(SendTo.NotServer)]
    void SyncCivResourcesClientRpc(int civId, int admin, int diplo, int mil, float coins)
    {
        Civilisation civ = Game.main.civs[civId];
        civ.adminPower = admin;
        civ.diploPower = diplo;
        civ.milPower = mil;
        civ.coins = coins;
    }



    [Rpc(SendTo.Server)]
    public void DeclareWarRpc(int civId, int targetCiv, Vector3Int warGoal,int cbIndex)
    {
        Civilisation civ = Game.main.civs[civId];
        War war = civ.DeclareWar(targetCiv, warGoal, Map.main.casusBellis[cbIndex]);
        if(war == null) { return; }
        NetworkWar netWar = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(netWarPrefab, position: civ.capitalIndicator.transform.position).GetComponent<NetworkWar>();
        netWar.war = war;
        war.networkWar = netWar;
        Game.main.ongoingWars.Add(war);
        netWar.init = true;
    }
    
    public void SendPeaceDeal(PeaceDeal peaceDeal, Civilisation target, bool mainTarget = true)
    {
        int casusBelliID = Map.main.casusBellis.ToList().IndexOf(peaceDeal.war.casusBelli);
        //Debug.Log("Send Peace Deal to " + peaceDeal.target.civName + " war score " + peaceDeal.warScore);
        AcceptPeaceDealRpc(casusBelliID, target.CivID, peaceDeal.taker.CivID, peaceDeal.subjugation, peaceDeal.provinces.ToArray(), peaceDeal.civTo.ToArray(), peaceDeal.numLoans, peaceDeal.fullAnnexation,peaceDeal.warScore, mainTarget);
    }
    [Rpc(SendTo.Server)]
    void AcceptPeaceDealRpc(int casusBelliID, int targetCivID, int takerCivID, bool subjugation, Vector3Int[] provinces, int[] civTo,int numLoans,bool fullAnnexation, float warScore, bool mainTarget = true)
    {
        Civilisation target = Game.main.civs[targetCivID];
        CasusBelli casusBelli = Map.main.casusBellis[casusBelliID];
        PeaceDeal peaceDeal = new PeaceDeal(null, target, Game.main.civs[takerCivID]);

        peaceDeal.subjugation = subjugation;
        peaceDeal.provinces = provinces.ToList();
        peaceDeal.civTo = civTo.ToList();
        peaceDeal.numLoans = numLoans;
        peaceDeal.fullAnnexation = fullAnnexation;
        peaceDeal.warScore = warScore;

        target.AcceptPeaceDeal(peaceDeal, mainTarget);



        AcceptPeaceDealClientRpc(casusBelliID, targetCivID, takerCivID, subjugation,provinces,  civTo,  numLoans,  fullAnnexation,warScore,  mainTarget);
    }
    [Rpc(SendTo.NotServer)]
    void AcceptPeaceDealClientRpc(int casusBelliID, int targetCivID, int takerCivID, bool subjugation, Vector3Int[] provinces, int[] civTo, int numLoans, bool fullAnnexation,float warScore, bool mainTarget = true)
    {
        Civilisation target = Game.main.civs[targetCivID];
        CasusBelli casusBelli = Map.main.casusBellis[casusBelliID];
        PeaceDeal peaceDeal = new PeaceDeal(target, Game.main.civs[takerCivID]);

        peaceDeal.subjugation = subjugation;
        peaceDeal.provinces = provinces.ToList();
        peaceDeal.civTo = civTo.ToList();
        peaceDeal.numLoans = numLoans;
        peaceDeal.fullAnnexation = fullAnnexation;
        peaceDeal.warScore = warScore;

        target.AcceptPeaceDeal(peaceDeal, mainTarget);

    }



    [Rpc(SendTo.Server)]
    public void CivExtraActionRpc(int civId, CivExtraActions actionId, int a0 = -1, int a1 = -1,int a2 = -1, int a3 = -1,int a4 = -1)
    {
        Civilisation civ = Game.main.civs[civId];
        CivExtraAction(civ, actionId, a0,a1,a2,a3,a4);
        //SyncCivResourcesRpc(civId);
        CivExtraActionClientRpc(civId, actionId, a0, a1, a2,a3,a4);
    }
    [Rpc(SendTo.NotServer)]
    void CivExtraActionClientRpc(int civId, CivExtraActions actionId, int a0 = -1, int a1 = -1, int a2 = -1, int a3 = -1,int a4 = -1)
    {
        Civilisation civ = Game.main.civs[civId];
        CivExtraAction(civ, actionId, a0, a1, a2,a3,a4);
    }
    void CivExtraAction(Civilisation civ, CivExtraActions actionId, int a0 = -1, int a1 = -1, int a2 = -1,int a3 = -1,int a4 = -1)
    {
        switch (actionId)
        {
            case CivExtraActions.SelectIdeaGroup:
                {
                    civ.ideaGroups[a0] = new IdeaGroupData(a1, a2, 0);
                    break;
                }
            case CivExtraActions.AssignAdvisor:
                {                    
                    List<Advisor> advisors = a0 == 0 ? civ.advisorsA : a0 == 1 ? civ.advisorsD : civ.advisorsM;
                    if (a1 < advisors.Count)
                    {
                        Advisor advisor = advisors[a1];
                        civ.AssignAdvisor(advisor);
                    }
                    else { Debug.Log("Advisor at: " + a1 + " not found"); }
                    break;
                }
            case CivExtraActions.PromoteAdvisor:
                {
                    Advisor advisor = a0 == 0 ? civ.advisorA : a0 == 1 ? civ.advisorD : civ.advisorM;
                    civ.coins -= advisor.HireCost(civ, 1) * 5;
                    advisor.skillLevel += 1;
                    break;
                }
            case CivExtraActions.FireAdvisor:
                {
                    Advisor advisor = a0 == 0 ? civ.advisorA : a0 == 1 ? civ.advisorD : civ.advisorM;
                    advisor.active = false;
                    civ.RemoveAdvisor(advisor);
                    break;
                }
            case CivExtraActions.FillAdvisorPool:
                {
                    List<Advisor> advisors = a0 == 0 ? civ.advisorsA : a0 == 1 ? civ.advisorsD : civ.advisorsM;
                    Advisor advisor = new Advisor(0, Age.zero, -1, -1);
                    if (a0 == 0)
                    {                        
                        advisor = new Advisor(Map.main.advisorsA[a1]);
                    }
                    else if (a0 == 1)
                    {                        
                        advisor = new Advisor(Map.main.advisorsD[a1]);
                    }
                    else if (a0 == 2)
                    {
                        advisor = new Advisor(Map.main.advisorsM[a1]);
                    }
                    advisor.age = new Age(0, 0, 0, a3);
                    advisor.skillLevel = a2;                   
                    advisor.civID = civ.CivID;
                    advisor.active = true;
                    advisor.Activate();
                    advisors.Add(advisor);
                    break;
                }
            case CivExtraActions.RemoveFromAdvisorPool:
                {
                    List<Advisor> advisors = a0 == 0 ? civ.advisorsA : a0 == 1 ? civ.advisorsD : civ.advisorsM;
                    advisors.RemoveAt(a1);
                    break;
                }
            case CivExtraActions.ReligiousMechanic:
                {
                    if (civ.religion == 1)
                    {
                        DjinnWorshipUI.DjinnFavor(civ, a0);                      
                    }
                    else if (civ.religion == 2)
                    {
                        LocalDeitiesUI.SelectDeity(civ,a0);
                    }
                    else if (civ.religion == 3)
                    {
                        if(a0 == 0)
                        {
                            GreatSwampUI.Feed(civ, a1);
                        }                        
                        else if(a0 == 1)
                        {
                            GreatSwampUI.Ask(civ, a1);
                        }
                    }
                    break;
                }
            case CivExtraActions.NewGeneral:
                {
                    General general = new General(Age.zero);                   
                    general.combatSkill = a0;
                    general.siegeSkill = a1;
                    general.maneuverSkill = a2;              
                    civ.generals.Add(general);
                    break;
                }
            case CivExtraActions.TakeGovReform:
                {
                    GovernmentType governmentType = Map.main.governmentTypes[civ.government];
                    if (governmentType.BaseReforms.Length == 0) { return; }
                    GovernmentReformTier[] tiers = governmentType.BaseReforms;
                    if (civ.reforms.Count == a0)
                    {
                        int cost = (40 + 40 * a0);
                        civ.reformProgress -= cost;
                        civ.reforms.Add(a1);
                        GovernmentUI.BuyReform(civ, tiers[a0].Reforms[a1]);
                    }
                    else if (civ.reforms.Count > a0)
                    {
                        int cost = 50;
                        civ.reformProgress -= cost;
                        GovernmentUI.RemoveReform(civ, tiers[a0].Reforms[civ.reforms[a0]]);
                        GovernmentUI.BuyReform(civ, tiers[a0].Reforms[a1]);
                        civ.reforms[a0] = a1;
                    }
                    break;
                }
            case CivExtraActions.SendEvent:
                {
                    if (a0 != -1)
                    {
                        EventData eventData = Map.main.pulseEvents[a0];
                        if (eventData.affectsCapital)
                        {
                            eventData.province = Map.main.GetTile(civ.capitalPos);
                        }
                        else if (eventData.affectsRandomProvince)
                        {
                            eventData.province = civ.GetAllCivTileDatas()[a1];
                        }
                        civ.SendEvent(eventData);
                        civ.eventHistory.Add(Map.main.pulseEvents.ToList().IndexOf(eventData));
                    }
                    break;
                }
            case CivExtraActions.SubjectInteraction:
                {
                    switch (a0)
                    {
                        case 0: //Send Gift
                            {
                                //SubjectsUI.SendGift(civ, Game.main.civs[a1]);
                                break;
                            }
                        case 1: //Grant Province
                            {
                                Civilisation selected = Game.main.civs[a1];
                                selected.libertyDesireTemp.IncreaseModifier("Granted Province", -1f * a2, EffectType.Flat, Decay: true);
                                break;
                            }
                        case 2: //Seize Land
                            {
                                Civilisation selected = Game.main.civs[a1];
                                selected.libertyDesireTemp.IncreaseModifier("Seized Land", 5f * a2, EffectType.Flat, Decay: true);
                                break;
                            }
                        case 3: //Placate Ruler
                            {
                                Civilisation selected = Game.main.civs[a1];
                                if (civ.prestige >= 0)
                                {
                                    civ.AddPrestige(-20);
                                    selected.libertyDesireTemp.IncreaseModifier("Placated Rulers", -10f, EffectType.Flat, Decay: true);
                                    selected.SetLibertyDesire();
                                }
                                break;
                            }
                        case 4: //Pay Off Loans
                            {
                                Civilisation selected = Game.main.civs[a1];
                                float amount = 0;
                                int loanNum = selected.loans.Count;
                                selected.loans.ForEach(i => amount += i.value);
                                if (civ.coins >= amount)
                                {
                                    selected.loans.Clear();
                                    civ.coins -= amount;
                                    selected.libertyDesireTemp.IncreaseModifier("Paid off our Loans", -5f * loanNum, EffectType.Flat, Decay: true);
                                    selected.SetLibertyDesire();
                                }
                                break;
                            }
                        case 5: //Begin Integration
                            {
                                Game.main.civs[a1].integrating = !Game.main.civs[a1].integrating;
                                break;
                            }                            
                    }
                    break;
                }
            case CivExtraActions.NewHeir:
                {
                    string name = Ruler.GenerateName(a3 == -1 ? civ : Game.main.civs[a3]);
                    civ.heir = new Ruler(a0, a1, a2, Age.zero, civ.CivID, name);
                    break;
                }
        }
    }
    public enum CivExtraActions
    {
        SelectIdeaGroup,
        AssignAdvisor,
        PromoteAdvisor,
        FireAdvisor,
        FillAdvisorPool,
        RemoveFromAdvisorPool,
        ReligiousMechanic,
        NewGeneral,
        TakeGovReform,
        SendEvent,
        SubjectInteraction,
        NewHeir,
    }


    


    [Rpc(SendTo.Server)]
    public void SiegeDiceRollRpc(Vector3Int tilePos, int diceRoll)
    {
        TileData tile = Map.main.GetTile(tilePos);
        if (tile.underSiege)
        {
            tile.siege.AddProgress(diceRoll);
        }
        SiegeDiceRollClientRpc(tilePos,diceRoll);
    }
    [Rpc(SendTo.NotServer)]
    void SiegeDiceRollClientRpc(Vector3Int tilePos, int diceRoll)
    {
        TileData tile = Map.main.GetTile(tilePos);
        if (tile.underSiege)
        {
            tile.siege.AddProgress(diceRoll);
        }
    }

    [Rpc(SendTo.Server)]
    public void CivRequestAccessRpc(int civId, int target,bool remove = false)
    {
        Civilisation civ = Game.main.civs[civId];
        if (!remove)
        {
            civ.AccessRequest(target);
        }
        else
        {
            civ.RemoveAccess(target);
        }
        CivRequestAccessClientRpc(civId, target,remove);
    }
    [Rpc(SendTo.NotServer)]
    void CivRequestAccessClientRpc(int civId, int target, bool remove = false)
    {
        Civilisation civ = Game.main.civs[civId];
        civ.AccessRequest(target);
        if (!remove)
        {
            civ.AccessRequest(target);
        }
        else
        {
            civ.RemoveAccess(target);
        }
    }


    [Rpc(SendTo.Server)]
    public void CivClaimRpc (int civId, Vector3Int pos)
    {
        Civilisation civ = Game.main.civs[civId];
        if (!civ.claims.Contains(pos))
        {
            civ.claims.Add(pos);
        }
        CivClaimClientRpc(civId, pos);
    }
    [Rpc(SendTo.NotServer)]
    void CivClaimClientRpc(int civId, Vector3Int pos)
    {
        Civilisation civ = Game.main.civs[civId];
        if (!civ.claims.Contains(pos))
        {
            civ.claims.Add(pos);
        }
    }

    [Rpc(SendTo.Server)]
    public void CivOccupyRpc(int civId, Vector3Int pos)
    {
        Civilisation civ = Game.main.civs[civId];
        TileData tile = Map.main.GetTile(pos);       
        if (civ.atWarWith.Contains(tile.civID))
        {
            tile.occupied = true;
            tile.occupiedByID = civId;
        }
        else if (civ.atWarTogether.Contains(tile.civID))
        {
            tile.occupied = false;
            tile.occupiedByID = civId;
        }
        else
        {
            tile.occupied = false;
            tile.occupiedByID = tile.civID;
        }
        CivOccupyClientRpc(civId, pos);
    }
    [Rpc(SendTo.NotServer)]
    void CivOccupyClientRpc(int civId, Vector3Int pos)
    {
        Civilisation civ = Game.main.civs[civId];
        TileData tile = Map.main.GetTile(pos);
        if (civ.atWarWith.Contains(tile.civID))
        {
            tile.occupied = true;
            tile.occupiedByID = civId;
        }
        else if (civ.atWarTogether.Contains(tile.civID))
        {
            tile.occupied = false;
            tile.occupiedByID = civId;
        }
        else
        {
            tile.occupied = false;
            tile.occupiedByID = tile.civID;
        }
    }
}