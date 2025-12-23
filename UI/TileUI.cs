using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using static MultiplayerManager;

public class TileUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI terrainName, regionName, devA, devB, devC, devT, resourceValue, devCostText;
    [SerializeField] Image tileImage, resourceImage, coreFill,religionFill,religionIcon,occupiedIcon;
    [SerializeField] Button openMercMenu, devAButton, devBButton, devCButton, startCore, toggleBuildingMenu,convertReligion,moveCapital,toggleGP,promoteStatus;
    [SerializeField] GameObject buildingMenu,mercenaryMenu,greatProjectMenu,grantProvinceMenu;
    [SerializeField] Sprite unknown;
    [SerializeField] TextMeshProUGUI forceLimit, unrest;
    [SerializeField] TextMeshProUGUI population,status,controlDecay;
    [SerializeField] TextMeshProUGUI productionIncome, taxIncome, control,govCap,totalIncome;
    [SerializeField] Button increaseControl,decreaseControl;
    [SerializeField] Button expandInf, reduceInf , Grant , Seize;
    [SerializeField] Transform coreBack,grantProvBack,unitQueueBack,buildQueueBack;
    [SerializeField] GameObject corePrefab,grantProvPrefab,unitQueuePrefab;
    List<GameObject> cores = new List<GameObject>();
    List<GameObject> grantProvs = new List<GameObject>();
    List<GameObject> unitQueue = new List<GameObject>();
    List<GameObject> buildQueue = new List<GameObject>();
    private void Start()
    {
        devAButton.onClick.AddListener(delegate { AddDev(0); });
        devBButton.onClick.AddListener(delegate { AddDev(1); });
        devCButton.onClick.AddListener(delegate { AddDev(2); });
        increaseControl.onClick.AddListener(delegate { ChangeControl(true); });
        decreaseControl.onClick.AddListener(delegate { ChangeControl(false); });
        expandInf.onClick.AddListener(delegate { ChangeInfrastructure(true); });
        reduceInf.onClick.AddListener(delegate { ChangeInfrastructure(false); });
        openMercMenu.onClick.AddListener(ToggleMercMenu);
        startCore.onClick.AddListener(StartCore);
        toggleBuildingMenu.onClick.AddListener(ToggleBuildingMenu);
        toggleGP.onClick.AddListener(ToggleGPMenu);
        convertReligion.onClick.AddListener(StartConvertReligion);
        occupiedIcon.GetComponent<Button>().onClick.AddListener(RequestOccupation);
        moveCapital.onClick.AddListener(MoveCapital);
        promoteStatus.onClick.AddListener(PromoteStatus);
        Seize.onClick.AddListener(SeizeLand);
        Grant.onClick.AddListener(ToggleGrantProvince);
    }
    private void OnDisable()
    {
        grantProvinceMenu.SetActive(false);
    }
    void ToggleGrantProvince()
    {
        grantProvinceMenu.SetActive(!grantProvinceMenu.activeSelf);
        if (grantProvinceMenu.activeSelf)
        {
            if (!Player.myPlayer.tileSelected || Player.myPlayer.myCivID == -1) { return; }
            TileData tile = Player.myPlayer.selectedTile;
            Civilisation civ = Player.myPlayer.myCiv;
            grantProvs.ForEach(i => Destroy(i));
            grantProvs.Clear();
            foreach(var subjectID in civ.subjects)
            {
                Civilisation subject = Game.main.civs[subjectID];
                if (subject.CanCoreTile(tile))
                {
                    GameObject item = Instantiate(grantProvPrefab, grantProvBack);
                    item.GetComponent<Button>().onClick.AddListener(delegate { GrantProv(subjectID); });
                    item.GetComponentInChildren<TextMeshProUGUI>().text = subject.civName;
                    item.GetComponentsInChildren<Image>()[1].color = subject.c;
                    item.GetComponent<HoverText>().text = "Liberty Desire " + tile.totalDev * (tile.cores.Contains(subjectID) ? -3f : -1f) + "%";
                    grantProvs.Add(item);
                }
            }
        }
    }
    void GrantProv(int toCivID)
    {
        if (!Player.myPlayer.tileSelected || Player.myPlayer.myCivID == -1) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        Civilisation civ = Player.myPlayer.myCiv;
        if(tile.civID != civ.CivID) { return; }
        Civilisation targetCiv = Game.main.civs[toCivID];
        if (targetCiv.overlordID == civ.CivID && civ.capitalPos != tile.pos)
        {
            targetCiv.libertyDesireTemp.IncreaseModifier("Granted Land", tile.totalDev * (tile.cores.Contains(toCivID) ? -3f : -1f), EffectType.Flat, Decay: true);
            tile.TransferOccupation(toCivID);
            targetCiv.SetLibertyDesire();
        }
        grantProvinceMenu.SetActive(false);
    }
    void SeizeLand()
    {
        if (!Player.myPlayer.tileSelected || Player.myPlayer.myCivID == -1) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        Civilisation oldCiv = tile.civ;
        Civilisation civ = Player.myPlayer.myCiv;
        if(oldCiv.overlordID == civ.CivID && oldCiv.libertyDesire <= 50f && oldCiv.capitalPos != tile.pos)
        {
            oldCiv.libertyDesireTemp.IncreaseModifier("Seized Land", tile.totalDev * (tile.hasCore ? 5f : 3f), EffectType.Flat, Decay: true);
            tile.TransferOccupation(civ.CivID);
            oldCiv.SetLibertyDesire();
        }
    }
    void PromoteStatus()
    {
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        
        if (Game.main.isMultiplayer)
        {
            Game.main.multiplayerManager.TileActionRpc(tile.pos, MultiplayerManager.TileActions.CoreConvertStatus, 2);
        }
        else
        {
            tile.PromoteStatus();
        }
    }
    void MoveCapital()
    {
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        if (tile.civID == Player.myPlayer.myCivID && Player.myPlayer.myCivID > -1)
        {
            Civilisation civ = Player.myPlayer.myCiv;
            if(tile.cores.Contains(civ.CivID) && tile.pos != civ.capitalPos)
            {
                int cost = 200;
                if (civ.adminPower >= cost)
                {
                    
                    if (Game.main.isMultiplayer)
                    {
                        Game.main.multiplayerManager.TileActionRpc(tile.pos, MultiplayerManager.TileActions.MoveCapital, civ.CivID);
                        Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.SpendAdmin, cost);
                    }
                    else 
                    {
                        civ.adminPower -= cost;
                        civ.MoveCapitalToSaveGame(tile.pos);
                    }
                }
            }
        }
    }
    void RequestOccupation()
    {
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        if (tile.civID > -1 && Player.myPlayer.myCivID > -1)
        {           
            Civilisation civ = Player.myPlayer.myCiv;
            Civilisation tileCiv = tile.civ;
            if (tile.occupied && tile.occupiedByID > -1)
            {
                Civilisation occupier = Game.main.civs[tile.occupiedByID];
                War war = tile.civ.GetWars().Find(i => i.Involving(occupier.CivID));

                if (war != null && war.attackerCiv.CivID != occupier.CivID && war.defenderCiv.CivID != occupier.CivID)
                {
                    if (war.attackerAllies.Exists(i => i.CivID == occupier.CivID))
                    {
                        if (civ == war.attackerCiv)
                        {
                            tile.occupiedByID = civ.CivID;
                        }
                    }
                    else if (war.defenderAllies.Exists(i => i.CivID == occupier.CivID))
                    {
                        if (civ == war.defenderCiv)
                        {
                            tile.occupiedByID = civ.CivID;
                        }
                    }
                }
            }
        }
    }

    void ToggleGPMenu()
    {
        greatProjectMenu.SetActive(!greatProjectMenu.activeSelf);
        buildingMenu.SetActive(false);
        mercenaryMenu.SetActive(false);
    }
    void ToggleMercMenu()
    {
        mercenaryMenu.SetActive(!mercenaryMenu.activeSelf);
        buildingMenu.SetActive(false);
        greatProjectMenu.SetActive(false);
    }
    void ToggleBuildingMenu()
    {
        buildingMenu.SetActive(!buildingMenu.activeSelf);
        mercenaryMenu.SetActive(false);
        greatProjectMenu.SetActive(false);
        toggleBuildingMenu.GetComponentsInChildren<Image>()[1].color = buildingMenu.activeSelf ? Color.white : Color.gray;
    }
    string StatusToString(int status)
    {
        switch (status)
        {
            case 0:
                return "Settlement";
            case 1:
                return "Town";
            case 2:
                return "City";
            case 3:
                return "Mega City";
            default:
                return "N/A";
        }
    }
    string StatusButtonHover(int status)
    {
        switch (status)
        {
            case 0:
                return "Promote To Town";
            case 1:
                return "Promote To City";
            case 2:
                return "Promote To Mega City";
            case 3:
                return "Maximum Status";
            default:
                return "N/A";
        }
    }

    string StatusHover(int status)
    {
        string text = "Current Status Level Gives:\n\n";
        text += "Governing Cost: " + ((1f + (status == 0 ? -0.5f : status == 1 ? 0f : status == 2 ? 1f : 2f)) * 100f) + "%\n";
        if (status > 1)
        {
            int level = status - 1;
            text += "Dev Cost Modifier: " + (-5 * level) + "%\n";
            text += "Construction Time: " + (-10 * level) + "%\n";
            text += "Construction Cost: " + (-5 * level) + "%\n";
            text += "Local Unrest: " + (-1 * level) + "\n";
            text += "Maximum Population: " + (50 * level) + "%";
        }
        return text;
    }
    private void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            Player.myPlayer.tileSelected = false;
            Player.myPlayer.selectedTile = null;
            buildingMenu.SetActive(false);
            mercenaryMenu.SetActive(false);
            toggleBuildingMenu.GetComponentsInChildren<Image>()[1].color = buildingMenu.activeSelf ? Color.white : Color.gray;
        }
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        toggleGP.gameObject.SetActive(tile.greatProject != null);
        toggleGP.interactable = (tile.greatProject != null);
        bool owned = tile.civID > -1;
        if(owned && tile.civ.overlordID == Player.myPlayer.myCivID && Player.myPlayer.myCivID > -1 && tile.civ.capitalPos != tile.pos)
        {
            Seize.gameObject.SetActive(true);
            Seize.GetComponent<HoverText>().text = "Seize Land\n\n" + "Liberty Desire +" + tile.totalDev * (tile.hasCore ? 5f : 3f) + "%";
        }
        else
        {
            Seize.gameObject.SetActive(false);
        }
        if (owned && tile.civID == Player.myPlayer.myCivID && Player.myPlayer.myCivID > -1 && tile.civ.subjects.Count > 0 && tile.civ.capitalPos != tile.pos)
        {
            Grant.gameObject.SetActive(true);
        }
        else
        {
            Grant.gameObject.SetActive(false);
            grantProvinceMenu.SetActive(false);
        }
        devAButton.enabled = tile.canDev(0);
        devBButton.enabled = tile.canDev(1);
        devCButton.enabled = tile.canDev(2);
        devA.text = tile.developmentA + "";
        devB.text = tile.developmentB + "";
        devC.text = tile.developmentC + "";
        devT.text = tile.totalDev + "";
        if (owned)
        {
            moveCapital.GetComponentsInChildren<Image>()[1].color = tile.civ.capitalPos == tile.pos ? Color.white : Color.gray;
            moveCapital.GetComponentInChildren<HoverText>().text = tile.civ.capitalPos == tile.pos ? "Already Capital ":"Move Capital For 200<sprite index=1>" ;
            promoteStatus.GetComponentsInChildren<Image>()[1].color = tile.CanPromoteStatus() ? Color.white : Color.gray;
            string text = StatusButtonHover(tile.status) + (tile.status < 3 ? "\nCost: " + tile.PromoteStatusCost() : "") ;
            text += (tile.status == 0 ? "\nPromotion Slots: " + tile.civ.controlCentres.Count + "/" + (int)tile.civ.maxSettlements.v : "");
            text += (tile.status > 0 ? "\nDevelopment Required: " + (tile.status * 10) : "");
            promoteStatus.GetComponentInChildren<HoverText>().text = text;
            govCap.text = "Gov Cap: "+ tile.GetGoverningCost();
            status.text = "Status: " + StatusToString(tile.status);
            status.transform.parent.GetComponent<HoverText>().text = StatusHover(tile.status);
        }
        else
        {
            govCap.text = "Unowned";
        }
        if (tile.civID > -1 && Player.myPlayer.myCivID > -1)
        {
            string text = "Cannot Request Occupation";
            Civilisation civ = Player.myPlayer.myCiv;
            Civilisation tileCiv = tile.civ;
            if (tile.occupied)
            {              
                occupiedIcon.color = tile.occupiedByID > -1 ? Game.main.civs[tile.occupiedByID].c : Color.black;
                if (tile.occupiedByID > -1)
                {
                    Civilisation occupier = Game.main.civs[tile.occupiedByID];
                   
                    War war = tile.civ.GetWars().Find(i => i.Involving(occupier.CivID));

                    if (war != null && war.attackerCiv.CivID != occupier.CivID && war.defenderCiv.CivID != occupier.CivID)
                    {
                        if (war.attackerAllies.Exists(i => i.CivID == occupier.CivID))
                        {
                            if (civ == war.attackerCiv)
                            {
                                text = "Request the Occupation of " + tile.Name;
                            }
                        }
                        else if (war.defenderAllies.Exists(i => i.CivID == occupier.CivID))
                        {
                            if (civ == war.defenderCiv)
                            {
                                text = "Request the Occupation of " + tile.Name;
                            }
                        }
                    }
                }               
                
            }
            else
            {
                occupiedIcon.color = tile.civID > -1 ? tile.civ.c : Color.black;
            }
            occupiedIcon.GetComponent<HoverText>().text = text;
        }
        SetDevelopText();
        SetCoreText();
        SetReligionConvertText();
        SetEconText();
        SetDevCostText();      
        SetPopulationText();
        SetForceLimitText();
        SetUnrestText();
        SetResourceText();
        if (tile.hasCore) { coreFill.fillAmount = 1f; startCore.enabled = false;startCore.GetComponent<Image>().color = Color.green; }
        else if (tile.needsCoring()) { coreFill.fillAmount = 0f; startCore.enabled = true; startCore.GetComponent<Image>().color = Color.red; }
        else { coreFill.fillAmount = 1f - (float)tile.coreTimer/(float)tile.GetCoreTime(); startCore.enabled = false; startCore.GetComponent<Image>().color = Color.yellow; }
        if (!owned || tile.religion == tile.civ.religion) 
        { 
            religionFill.fillAmount = 1f;
            convertReligion.enabled = false; 
            convertReligion.GetComponent<Image>().color = Color.green;
        }
        else if (tile.needsConverting()) 
        {
            religionFill.fillAmount = 0f;
            convertReligion.enabled = true;
            convertReligion.GetComponent<Image>().color = Color.red;
        }
        else 
        {
            religionFill.fillAmount = tile.conversionProgress;
            convertReligion.enabled = false;
            convertReligion.GetComponent<Image>().color = Color.yellow;
        }
        if (tile.religion > -1)
        {
            religionIcon.sprite = Map.main.religions[tile.religion].sprite;
        }
        else
        {
            religionIcon.sprite = unknown;
        }
        if (tile.civID > -1)
        {
            religionIcon.GetComponent<HoverText>().text = Map.main.religions[tile.religion].GetHoverText(tile.civ);
        }

        forceLimit.text = "Force Limit: " +Mathf.Round(tile.GetForceLimit() * 100f) / 100f + "<sprite index=0>";
        unrest.text = "Local Unrest: " + Mathf.Round(tile.unrest * 100f) / 100f + "<sprite index=11>";
        float prod = Mathf.Round(tile.GetDailyProductionValue() * 100f) / 100f;
        float tax = Mathf.Round(tile.GetDailyTax() * 100f) / 100f;
        productionIncome.text = "Production: " + prod + " <sprite index=0>";
        taxIncome.text = "Tax: " + tax + " <sprite index=0>";
        totalIncome.text = "Total: " + (prod + tax) + " <sprite index=0>";
        control.text = "Control: " + Mathf.Round(tile.control * 100f) / 100f + "%";
        Color c = Color.Lerp(Color.red, Color.green, (float)tile.avaliablePopulation / (float)(tile.avaliableMaxPopulation+1));    
        if(tile.avaliablePopulation >= tile.avaliableMaxPopulation)
        {
            c = Color.magenta;
        }
        population.text = "Population: " + "<#" + ColorUtility.ToHtmlStringRGB(c) + ">" +tile.avaliablePopulation + "<sprite index=0>";
        devCostText.text = tile.GetDevCost() + "";
        regionName.text = tile.region;
        terrainName.text = tile.Name;
        if (tile.terrain != null)
        {
            tileImage.sprite = tile.terrain.sprite;
            HoverText ht = tileImage.transform.parent.GetComponent<HoverText>();
            ht.text = tile.terrain.GetHoverText();
        }
        if(tile.tileResource != null  && tile.tileResource.sprite != null)
        {
            resourceImage.sprite = tile.tileResource.sprite;
            resourceValue.text = Mathf.Round(tile.tileResource.Value * 100f)/100f + " <sprite index=0>";
        }
        else
        {
            resourceImage.sprite = unknown;
        }
        while (cores.Count != tile.cores.Count)
        {
            if (cores.Count > tile.cores.Count)
            {
                int lastIndex = cores.Count - 1;
                Destroy(cores[lastIndex]);
                cores.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(corePrefab, coreBack);
                cores.Add(item);
            }
        }
        for (int i = 0; i < cores.Count; i++)
        {
            if (tile.cores[i] > -1)
            {
                Civilisation coreCiv = Game.main.civs[tile.cores[i]];
                cores[i].GetComponentInChildren<Image>().color = coreCiv.c;
            }
        }

        while (unitQueue.Count != tile.unitQueue.Count)
        {
            if (unitQueue.Count > tile.unitQueue.Count)
            {
                int lastIndex = unitQueue.Count - 1;
                Destroy(unitQueue[lastIndex]);
                unitQueue.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(unitQueuePrefab, unitQueueBack);
                unitQueue.Add(item);
            }
        }
        for (int i = 0; i < tile.unitQueue.Count; i++)
        {
            RecruitData data = tile.unitQueue[i];
            var texts = unitQueue[i].GetComponentsInChildren<TextMeshProUGUI>();
            var image = unitQueue[i].GetComponentInChildren<Image>();
            image.color = i == 0 ? Color.green : Color.red;
            texts[0].text = Player.myPlayer.myCivID > -1 ? data.GetUnitName(Player.myPlayer.myCiv) : "Unknown";
            texts[1].text = "" + (i == 0 ? tile.unitTimer : tile.GetRecruitTime());
        }

        while (buildQueue.Count != tile.buildQueue.Count)
        {
            if (buildQueue.Count > tile.buildQueue.Count)
            {
                int lastIndex = buildQueue.Count - 1;
                Destroy(buildQueue[lastIndex]);
                buildQueue.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(unitQueuePrefab, buildQueueBack);
                buildQueue.Add(item);
            }
        }
        for (int i = 0; i < tile.buildQueue.Count; i++)
        {
            Building building = Map.main.Buildings[tile.buildQueue[i]];
            var texts = buildQueue[i].GetComponentsInChildren<TextMeshProUGUI>();
            var image = buildQueue[i].GetComponentInChildren<Image>();
            image.color = i == 0 ? Color.green : Color.red;
            texts[0].text = building.Name;
            texts[1].text = "" + (i == 0 ? tile.buildTimer : (int)building.baseTime);
        }
    }
    void ChangeControl(bool up)
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID && civ.CivID != data.civ.overlordID) { return; }
        if (data.localUnrest.ms.Exists(i => i.n == "Control Increased" || i.n == "Control Decreased"))
        {
            return;
        }
        if (Game.main.isMultiplayer)
        {
            Game.main.multiplayerManager.TileActionRpc(data.pos,MultiplayerManager.TileActions.ChangeControl,up ? 0 : 1);
        }
        else
        {
            data.ChangeControl(up);
        }
    }
    void ChangeInfrastructure(bool up)
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID && civ.CivID != data.civ.overlordID) { return; }
        if (Game.main.isMultiplayer)
        {
            Game.main.multiplayerManager.TileActionRpc(data.pos,MultiplayerManager.TileActions.ChangeInfrastructure,up ? 0 : 1);
        }
        else
        {
            if (up)
            {
                if (data.infrastructureLevel < data.totalDev / 15 && civ.adminPower >= 50)
                {
                    civ.adminPower -= 50;
                    data.infrastructureLevel++;
                }
            }
            else
            {
                if (data.infrastructureLevel > 0)
                {
                    data.infrastructureLevel--;
                }
            }
            data.UpdateInfrastructureModifiers();
        }
    }
    void StartConvertReligion()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID && civ.CivID != data.civ.overlordID) { return; }
        if (Game.main.isMultiplayer)
        {
            Game.main.multiplayerManager.TileActionRpc(data.pos, MultiplayerManager.TileActions.CoreConvertStatus, 1);
        }
        else
        {
            data.StartConvert();
        }
    }
    void StartCore()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID && civ.CivID != data.civ.overlordID) { return; }       
        if (Game.main.isMultiplayer)
        {
            Game.main.multiplayerManager.TileActionRpc(data.pos, MultiplayerManager.TileActions.CoreConvertStatus, 0);
        }
        else
        {
            data.StartCore();
        }
    }
    void AddDev(int index)
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1|| data.civID == -1) { return; }       
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID && civ.CivID != data.civ.overlordID) { return; }
        if (Game.main.isMultiplayer)
        {
            Game.main.multiplayerManager.TileActionRpc(data.pos, MultiplayerManager.TileActions.Develop, index);
        }
        else
        {
            data.AddDevelopment(index, civ.CivID);
        }
    }    
    void SetPopulationText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1||data.civID == -1) { return; }
        Civilisation civ = data.civ; 
        HoverText hoverText = population.transform.parent.GetComponent<HoverText>();
        if (data.control == 100)
        {
            string text = "There are " + data.population + "<sprite index=4> living here\n\n";
            text += "There can be a maximum of " + data.maxPopulation + "<sprite index=4>\n";
            text += "Base: From Total Develoment " + data.totalDev + " * 200\n";
            text += "Local Bonuses: " + data.localMaxPopulation.ToString() + "\n";
            text += "Global Bonuses: " + civ.maximumPopulation.ToString() + "\n\n";
            text += data.populationGrowth + "<sprite index=4> join this tile every day\n";
            text += "Local Bonuses: " + data.localPopulationGrowth.ToString() + "\n";
            text += "Global Bonuses: " + civ.populationGrowth.ToString() + "\n\n";
            hoverText.text = text;
        }
        else
        {
            string text = "There are " + data.population + "<sprite index=4> living here\n";
            text += "Currently " + data.avaliablePopulation + "<sprite index=4> are faithful to you\n";
            text += "This is due to local control of " + data.control + "%\n\n";
            text += "There can be a maximum of " + data.maxPopulation + "<sprite index=4> ("+ data.avaliableMaxPopulation + ")\n";
            text += "Base: From Total Develoment " + data.totalDev + " * 200\n";
            text += "Local Bonuses: " + data.localMaxPopulation.ToString() + "\n";
            text += "Global Bonuses: " + civ.maximumPopulation.ToString() + "\n\n";
            text += data.populationGrowth + "<sprite index=4> ("+ (int)(data.populationGrowth * data.control/ 100f + data.population * (data.dailyControl.v + civ.dailyControl.v) / 100f) +") join this tile every day\n";
            text += "Local Bonuses: " + data.localPopulationGrowth.ToString() + "\n";
            text += "Global Bonuses: " + civ.populationGrowth.ToString() + "\n\n";
            hoverText.text = text;
        }

    }
    void SetResourceText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if(data.tileResource == null || data.tileResource.sprite == null) { return; }
        HoverText hoverText = resourceImage.GetComponent<HoverText>();
        string text = "This tile produces " + data.tileResource.name + "\n";
        text += "Which has a base value of " + data.tileResource.Value + "<sprite index=0>\n\n";
        text += "This resource has the following effect on the tile:\n";
        text += data.tileResource.localTileEffect + Modifier.ToString(data.tileResource.localTileEffectStrength, data.GetStat(data.tileResource.localTileEffect)) + "\n";
        hoverText.text = text;
    }
    void SetUnrestText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1 || data.civID == -1) { return; }
        Civilisation civ = data.civ;
        HoverText hoverText = unrest.transform.parent.GetComponent<HoverText>();
        string text = "Currently there is " + Mathf.Round(data.unrest * 100f) / 100f + " unrest here\n\n";
        text += "Due To:\n";
        text += "Local Bonuses: " + data.localUnrest.ToString() + "\n";
        text += "Global Bonuses: " + civ.globalUnrest.ToString() + "\n\n";

        float localUnrest = data.unrest;
        if (localUnrest > 0)
        {
            float Value = Mathf.Max(-1f, -localUnrest / 20f);
            text += "Positive Unrest Gives The Following Modifiers:\n\n";
            text += "Tax Efficiency: "+ Mathf.Round(Value * 100f) + "%\n";
            text += "Recruitment Time: +" + Mathf.Round(-Value * 100f) + "%\n";
            text += "Construction Time: +" + Mathf.Round(-Value * 100f) + "%\n";
            text += "Maximum Control Limit: " + Mathf.Round(Mathf.Max(25f, 100f * (1f - localUnrest / 20f))) + "%\n";
        }
        else
        {
            float Value = -localUnrest / 20f;
            text += "Negative Unrest Gives The Following Modifiers:\n\n";
            text += "Tax Efficiency: +" + Mathf.Round(Value * 100f) + "%\n";
        }

        hoverText.text = text;
    }
    void SetForceLimitText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1 || data.civID == -1) { return; }
        Civilisation civ = data.civ;
        HoverText hoverText = forceLimit.transform.parent.GetComponent<HoverText>();
        string text = "This tile adds " + Mathf.Round(data.GetForceLimit() * 100f) / 100f + "<sprite index=3> force limit\n\n";
        text += "This is due to:\n";
        text += "0.2 * " + Mathf.Round(data.avaliableMaxPopulation * 10f)/10000f + "k<sprite index=4>\n\n";
        text += "Local Bonuses: " + data.localForceLimit.ToString() + "\n";
        if (!data.hasCore)
        {
            text += "Reduced by 75% due to not being a core tile";
        }
        hoverText.text = text;
    }
    void SetCoreText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1 || data.civID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = startCore.GetComponent<HoverText>();
        if (data.needsCoring())
        {
            string text = "It will cost " + data.GetCoreCost() + " Admin Power<sprite index=1> to core this tile\n\n";
            text += "Base: " + data.totalDev * 5 + "<sprite index=1>\n";
            text += "Global Bonuses: " + civ.coreCost.ToString() + "\n\n";
            text += "This will take " + data.GetCoreTime() + " days";
            if (civ.claims.Contains(data.pos))
            {
                text += "Has A Claim: -25%";
            }
            hoverText.text = text;
        }
        else if(data.coreTimer != -1)
        {
            string text = "Currently coring this tile\n\n";
            text += "It will be complete in "+data.coreTimer+" days";
            hoverText.text = text;
        }
        else
        {          
            hoverText.text = "You already have a core here";
        }
    }
    void SetReligionConvertText()
    {
        TileData data = Player.myPlayer.selectedTile;
        HoverText hoverText = convertReligion.GetComponent<HoverText>();
        if (Player.myPlayer.myCivID == -1 || data.civID == -1) { hoverText.text = "Insufficient Knowledge"; return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        
        if (data.needsConverting())
        {
            string text = "Start Converting this tile\n\n";
            text += "Missionary Strength: " + Mathf.Round(civ.missionaryStrength.v * 100f) / 100f + "\n";
            text += "Conversion Resistance: " + Mathf.Round(data.GetConvertResistance() * 100f) / 100f + "\n\n";
            text += "Daily Progress: " + Mathf.Round(Mathf.Max(0, civ.missionaryStrength.v - data.GetConvertResistance()) * 100f) / 100f + "";
            hoverText.text = text;
        }
        else if (data.isConverting)
        {
            string text = "Currently converting this tile\n\n";
            text += Mathf.Round(data.conversionProgress * 100f) + "% complete\n\n";
            text += "Missionary Strength: " + Mathf.Round(civ.missionaryStrength.v * 100f)/100f + "\n";
            text += "Conversion Resistance: " + Mathf.Round(data.GetConvertResistance() * 100f) / 100f + "\n\n";
            text += "Daily Progress: " + Mathf.Round(Mathf.Max(0,civ.missionaryStrength.v - data.GetConvertResistance()) * 100f) / 100f + "";
            hoverText.text = text;
        }
        else
        {
            hoverText.text = "The people here follow the True Faith";
        }
    }
    void SetDevelopText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1 || data.civID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = devAButton.GetComponent<HoverText>();
        string text = "It will cost " + data.GetDevCost() + " Admin Power<sprite index=1> to develop this tile\n\n";
        text += "This will have the following effects: \n";
        text += "Maximum population +300<sprite index=4>\n";       
        text += "Force Limit +" + Mathf.Round(data.GetAnyDevForceLimitIncrease() * 100f) / 100f + "<sprite index=3>\n";
        text += "Max Control +1\n";
        text += "Control +1";
        hoverText.text = text;

        hoverText = devBButton.GetComponent<HoverText>();
        text = "It will cost " + data.GetDevCost() + " Diplo Power<sprite index=2> to develop this tile\n\n";
        text += "This will have the following effects: \n";
        text += "Maximum population +200<sprite index=4>\n";
        text += "Production Income +" + Mathf.Round(data.GetDevProdIncrease()*100f)/100f + "<sprite index=0>\n";
        text += "Force Limit +" + Mathf.Round(data.GetAnyDevForceLimitIncrease() * 100f) / 100f + "<sprite index=3>\n";
        text += "Max Control +1\n";
        text += "Control +1";
        hoverText.text = text;

        hoverText = devCButton.GetComponent<HoverText>();
        text = "It will cost " + data.GetDevCost() + " Military Power<sprite index=3> to develop this tile\n\n";
        text += "This will have the following effects: \n";
        text += "Maximum population +200<sprite index=4>\n";
        text += "Population Growth +" + Mathf.Round(6 * (1f + data.localPopulationGrowth.v + civ.populationGrowth.v) * 100f) / 100f + "<sprite index=4>\n";
        text += "Force Limit +" + Mathf.Round(data.GetAnyDevForceLimitIncrease() * 100f) / 100f + "<sprite index=3>\n";
        text += "Max Control +1\n";
        text += "Control +1";
        hoverText.text = text;
    }
    void SetEconText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1 || data.civID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = productionIncome.transform.parent.GetComponent<HoverText>();
        string text = "This tile generates " + Mathf.Round(data.GetDailyProductionValue() * 100f) / 100f + "<sprite index=0> per<sprite index=12>\n\n";
        text += "This is due to:\n";
        text += "Value of the good "+data.tileResource.Value + "<sprite index=0> * " +  Mathf.Round((1f + data.localProductionValue.v )*(1f + civ.productionValue.v) * 100f) /100f+" = "+ data.tileResource.Value * Mathf.Round((1f + data.localProductionValue.v) * (1f + civ.productionValue.v) * 100f) / 100f + "<sprite index=0>\n";
        text += "Local Bonuses: " +  data.localProductionValue.ToString() + "\n";
        text += "Multiplied by Global Bonuses: " + civ.productionValue.ToString() + "\n\n";
        text += "This good is produced "+Mathf.Round(7.2f * data.developmentB * (1f + data.localProductionQuantity.v) * (1f + civ.productionAmount.v) * 100f) / 100f + " times per year while population is above "+ (200f*data.developmentB) + "<sprite index=4>\n";
        text += "Local Bonuses: " + data.localProductionQuantity.ToString() + "\n";
        text += "Global Bonuses: " + civ.productionAmount.ToString() + "\n\n";
        if (!data.hasCore)
        {
            text += "Reduced by 75% due to not being a core tile";
        }
        hoverText.text = text;

        hoverText = taxIncome.transform.parent.GetComponent<HoverText>();
        text = "This tile collects " + Mathf.Round(data.GetDailyTax() * 100f) / 100f + "<sprite index=0> per<sprite index=12>\n\n";
        text += "This is due to:\n";
        text += Mathf.Round(0.01f * (1f + data.localTaxEfficiency.v) * (1f + civ.taxEfficiency.v) * 100f) / 100f + "<sprite index=0> per population<sprite index=4> every year\n";
        text += "Local Bonuses: " + data.localTaxEfficiency.ToString() + "\n";
        text += "Multiplied by Global Bonuses: " + civ.taxEfficiency.ToString() + "\n\n";
        if (!data.hasCore)
        {
            text += "Reduced by 75% due to not being a core tile";
        }
        hoverText.text = text;

        hoverText = control.transform.parent.GetComponent<HoverText>();
        text = "This tile is " + Mathf.Round(data.control * 100f) / 100f + "% under your control\n\n";
        text += "This changes by:\n";
        text += Mathf.Round((data.dailyControl.v + civ.dailyControl.v) * 100f)/100f + " per<sprite index=12>\n";
        text += "Local Bonuses: "+data.dailyControl.ToString() + "\n";
        text += "Multiplied by Global Bonuses: " + civ.dailyControl.ToString() + "\n\n";
        text += "Up to a maximum of:\n";
        text += data.maxControl + "%\n\n";
        if (!data.hasCore)
        {
            text += "Cannot be over 25% due to not being a core tile\n\n";
        }
        if (data.pos == civ.capitalPos)
        {
            text += "Cannot be less than 100% due to being the capital tile\n\n";
        }
        text += "This has the following effects:\n";
        text += "Max effective population<sprite index=4> = " + Mathf.Round(data.control * 100f) / 100f + "% Max population<sprite index=4>\n";
        hoverText.text = text;
    }
    void SetDevCostText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1 || data.civID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = devCostText.GetComponent<HoverText>();
        string text = "It will cost " + data.GetDevCost() + " Power<sprite index=1><sprite index=2><sprite index=3> to develop this tile\n\n";
        text += "This is due to the following: \n\n";
        text += "The Base Cost of developing " + (int)(50 * (1f + data.localDevCostMod.v + civ.devCostMod.v)) + "\n";
        text += "Local Modifiers:" + data.localDevCostMod.ToString() + "\n";
        text += "Global Modifiers:" + civ.devCostMod.ToString() + "\n\n";
        text += "Multiplied by:\n";
        text += "Local Modifiers:"+ data.localDevCost.ToString() + "\n";
        text += "Global Modifiers:" + civ.devCost.ToString() + "\n\n";
        text += "With a minimum of 10% of the base cost";
        hoverText.text = text;
    }
}
