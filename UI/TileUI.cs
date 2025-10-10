using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class TileUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI terrainName, regionName, devA, devB, devC, devT, resourceValue, devCostText;
    [SerializeField] Image tileImage, resourceImage, coreFill,infFill,cavFill,siegeFill,buildFill;
    [SerializeField] Button addInf,addCav,addSiege, devAButton, devBButton, devCButton, startCore, toggleBuildingMenu;
    [SerializeField] GameObject buildingMenu;
    [SerializeField] Sprite unknown;
    [SerializeField] TextMeshProUGUI forceLimit, unrest;
    [SerializeField] TextMeshProUGUI population;
    [SerializeField] TextMeshProUGUI productionIncome, taxIncome, control;
    [SerializeField] Transform coreBack;
    [SerializeField] GameObject corePrefab;
    [SerializeField] List<GameObject> cores = new List<GameObject>();
    private void Start()
    {
        addInf.onClick.AddListener(delegate{ AddArmy(0); });
        addCav.onClick.AddListener(delegate { AddArmy(1); });
        addSiege.onClick.AddListener(delegate { AddArmy(2); });
        devAButton.onClick.AddListener(delegate { AddDev(0); });
        devBButton.onClick.AddListener(delegate { AddDev(1); });
        devCButton.onClick.AddListener(delegate { AddDev(2); });
        startCore.onClick.AddListener(StartCore);
        toggleBuildingMenu.onClick.AddListener(ToggleBuildingMenu);
    }
    void ToggleBuildingMenu()
    {
        buildingMenu.SetActive(!buildingMenu.activeSelf);
        toggleBuildingMenu.GetComponentsInChildren<Transform>()[1].rotation = Quaternion.Euler(new Vector3(0, 0, buildingMenu.activeSelf ? 90f : -90f));
    }
    private void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            Player.myPlayer.tileSelected = false;
            Player.myPlayer.selectedTile = null;
        }
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        devA.text = tile.developmentA + "";
        devB.text = tile.developmentB + "";
        devC.text = tile.developmentC + "";
        devT.text = tile.totalDev + "";
        SetDevelopText();
        SetCoreText();
        SetEconText();
        SetDevCostText();
        SetArmyRecruitText(0);
        SetArmyRecruitText(1);
        SetArmyRecruitText(2);
        SetPopulationText();
        SetForceLimitText();
        SetUnrestText();
        SetResourceText();
        if (tile.hasCore) { coreFill.fillAmount = 1f; }
        else if (tile.needsCoring()) { coreFill.fillAmount = 0f; }
        else { coreFill.fillAmount = 1f - (float)tile.coreTimer/(float)tile.GetCoreTime(); }
        addCav.enabled = civ.techUnlocks.Contains("Flanking Units");
        addSiege.enabled = civ.techUnlocks.Contains("Siege Units");

        if (tile.recruitQueue.Count == 0) 
        {
            infFill.fillAmount = 0f;
            cavFill.fillAmount = 0f;
            siegeFill.fillAmount = 0f;
        }
        else  
        {
            int type = tile.recruitQueue.First();
            infFill.fillAmount = type == 0? 1f - (float)tile.recruitTimer/(float)tile.GetRecruitTime() : 0f;
            cavFill.fillAmount = type == 1 ? 1f - (float)tile.recruitTimer / (float)tile.GetRecruitTime() : 0f;
            siegeFill.fillAmount = type == 2 ? 1f - (float)tile.recruitTimer / (float)tile.GetRecruitTime() : 0f;
        }

        if (tile.buildQueue.Count == 0) { buildFill.fillAmount = 0f; }
        else { buildFill.fillAmount = 1f - (float)tile.buildTimer / (float)Map.main.Buildings[tile.buildQueue.First()].GetTime(tile, tile.civ); }

        forceLimit.text = Mathf.Round(tile.GetForceLimit() * 100f) / 100f + "<sprite index=0>";
        unrest.text = Mathf.Round(tile.unrest * 100f) / 100f + "<sprite index=11>";
        productionIncome.text = Mathf.Round(tile.GetDailyProductionValue()*100f)/100f + " <sprite index=0>";
        taxIncome.text = Mathf.Round(tile.GetDailyTax() * 100f) / 100f + " <sprite index=0>";
        control.text = Mathf.Round(tile.control * 100f) / 100f + "%";
        Color c = Color.Lerp(Color.red, Color.green, (float)tile.avaliablePopulation / (float)(tile.avaliableMaxPopulation+1));    
        if(tile.avaliablePopulation >= tile.avaliableMaxPopulation)
        {
            c = Color.magenta;
        }
        population.text = "<#"+c.ToHexString()+">" +tile.avaliablePopulation + "<sprite index=0>";
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
                UIManager.main.UI.Remove(cores[lastIndex]);
                cores.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(corePrefab, coreBack);
                cores.Add(item);
                UIManager.main.UI.Add(item);
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
    }
    void StartCore()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        data.StartCore(); 
    }
    void AddDev(int index)
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }       
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        data.AddDevelopment(index, civ.CivID);
    }
    void AddArmy(int id)
    {
        TileData data = Player.myPlayer.selectedTile;
        data.StartRecruiting(id);
    }
    void SetPopulationText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
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
            text += data.populationGrowth + "<sprite index=4> ("+ (int)(data.populationGrowth * data.control/ 100f + data.population * (data.dailyControl.value + civ.dailyControl.value) / 100f) +") join this tile every day\n";
            text += "Local Bonuses: " + data.localPopulationGrowth.ToString() + "\n";
            text += "Global Bonuses: " + civ.populationGrowth.ToString() + "\n\n";
            hoverText.text = text;
        }

    }
    void SetResourceText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
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
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = unrest.transform.parent.GetComponent<HoverText>();
        string text = "This tile has " + Mathf.Round((data.population - data.avaliablePopulation) * 100f) / 100f + "<sprite index=4> population that do not support you\n\n";
        text += "Currently there is " + Mathf.Round(data.unrest*100f)/100f+ " unrest here\n";
        text += "Due To:\n";
        text += "Local Bonuses: " + data.localUnrest.ToString() + "\n";
        text += "Global Bonuses: " + civ.globalUnrest.ToString() + "\n";        
        hoverText.text = text;
    }
    void SetForceLimitText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
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
    void SetArmyRecruitText(int index)
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = index == 0 ? addInf.GetComponent<HoverText>() : index == 1? addCav.GetComponent<HoverText>() : addSiege.GetComponent<HoverText>();
        if (data.recruitQueue.Count == 0)
        {
            string text = "It will cost " + data.GetRecruitCost(index) + "<sprite index=0> to recruit from this tile\n\n";
            text += "Base: " + civ.units[index].baseCost +"<sprite index=0>\n";
            text += "Local Bonuses: " + data.localRecruitmentCost.ToString() + "\n";
            text += "Global Bonuses: " + civ.regimentCost.ToString() + "\n\n";
            text += "This will take " + Mathf.Round(data.GetRecruitTime() * 10f/6f)/10f + " hours\n\n";
            text += "Base: 12 hours\n";
            text += "Local Bonuses: " + data.localRecruitmentTime.ToString() + "\n";
            text += "Global Bonuses: " + civ.recruitmentTime.ToString() + "\n\n";
            hoverText.text = text;
        }
        else
        {
            string text = "Currently recruiting from this tile\n";
            if (data.recruitQueue.Count > 1)
            {
                text += "There are " + (data.recruitQueue.Count - 1) + " regiments in the queue\n\n";
            }
            text += "It will be complete in " + data.recruitTimer + " days\n\n";
            text = "It will cost " + data.GetRecruitCost(index) + "<sprite index=0> to recruit from this tile\n\n";
            text += "Base: "+ civ.units[index].baseCost+"<sprite index=0>\n";
            text += "Local Bonuses: " + data.localRecruitmentCost.ToString() + "\n";
            text += "Global Bonuses: " + civ.regimentCost.ToString() + "\n\n";
            text += "This will take " + Mathf.Round(data.GetRecruitTime() * 10f / 6f) / 10f + " hours\n\n";
            text += "Base: 12 hours\n";
            text += "Local Bonuses: " + data.localRecruitmentTime.ToString() + "\n";
            text += "Global Bonuses: " + civ.recruitmentTime.ToString() + "\n\n";
            hoverText.text = text;
        }

    }
    void SetCoreText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = startCore.GetComponent<HoverText>();
        if (data.needsCoring())
        {
            string text = "It will cost " + data.GetCoreCost() + " Admin Power<sprite index=1> to core this tile\n\n";
            text += "Base: " + data.totalDev * 5 + "<sprite index=1>\n";
            text += "Global Bonuses: " + civ.coreCost.ToString() + "\n\n";
            text += "This will take " + data.GetCoreTime() + " days";
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
    void SetDevelopText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = devAButton.GetComponent<HoverText>();
        string text = "It will cost " + data.GetDevCost() + " Admin Power<sprite index=1> to develop this tile\n\n";
        text += "This will have the following effects: \n";
        text += "Maximum population +200<sprite index=4>\n";
        text += "Population Growth +" + Mathf.Round(6 *(1f + data.localPopulationGrowth.value + civ.populationGrowth.value) * 100f) / 100f + "<sprite index=4>\n";
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
        text += "Force Limit +" + Mathf.Round(data.GetAnyDevForceLimitIncrease() * 100f) / 100f + "<sprite index=3>\n";
        text += "Max Control +1\n";
        text += "Control +1";
        hoverText.text = text;
    }
    void SetEconText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = productionIncome.transform.parent.GetComponent<HoverText>();
        string text = "This tile generates " + Mathf.Round(data.GetDailyProductionValue() * 100f) / 100f + "<sprite index=0> per day\n\n";
        text += "This is due to:\n";
        text += "Value of the good "+data.tileResource.Value + "<sprite index=0> * " +  Mathf.Round((1f + data.localProductionValue.value + civ.productionValue.value) * 100f) /100f+" = "+ data.tileResource.Value * Mathf.Round((1f + data.localProductionValue.value+ civ.productionValue.value) * 100f) / 100f + "<sprite index=0>\n";
        text += "Local Bonuses: " +  data.localProductionValue.ToString() + "\n";
        text += "Global Bonuses: " + civ.productionValue.ToString() + "\n\n";
        text += "This good is produced "+Mathf.Round(7.2f * data.developmentB * (1f + data.localProductionQuantity.value + civ.productionAmount.value) * 100f) / 100f + " times per year while population is above "+ (200f*data.developmentB) + "<sprite index=4>\n";
        text += "Local Bonuses: " + data.localProductionQuantity.ToString() + "\n";
        text += "Global Bonuses: " + civ.productionAmount.ToString() + "\n\n";
        if (!data.hasCore)
        {
            text += "Reduced by 75% due to not being a core tile";
        }
        hoverText.text = text;

        hoverText = taxIncome.transform.parent.GetComponent<HoverText>();
        text = "This tile collects " + Mathf.Round(data.GetDailyTax() * 100f) / 100f + "<sprite index=0> per day\n\n";
        text += "This is due to:\n";
        text += Mathf.Round(0.01f * (1f + data.localTaxEfficiency.value + civ.taxEfficiency.value) * 100f) / 100f + "<sprite index=0> per population<sprite index=4> every year\n";
        text += "Local Bonuses: " + data.localTaxEfficiency.ToString() + "\n";
        text += "Global Bonuses: " + civ.taxEfficiency.ToString() + "\n\n";
        if (!data.hasCore)
        {
            text += "Reduced by 75% due to not being a core tile";
        }
        hoverText.text = text;

        hoverText = control.transform.parent.GetComponent<HoverText>();
        text = "This tile is " + Mathf.Round(data.control * 100f) / 100f + "% under your control\n\n";
        text += "This changes by:\n";
        text += data.dailyControl.value + civ.dailyControl.value + " per day\n";
        text += "Local Bonuses: "+data.dailyControl.ToString() + "\n";
        text += "Global Bonuses: "+civ.dailyControl.ToString() + "\n\n";
        text += "Up to a maximum of:\n";
        text += "100 - distance to capital squared (" + Mathf.Pow(TileData.evenr_distance(data.pos, civ.capitalPos),2) + ") * control decay "+Mathf.Round((1f + civ.controlDecay.value)*100f)+"% + development (" +data.totalDev +") = "+data.maxControl + "%\n\n";
        if (!data.hasCore)
        {
            text += "Cannot be over 10% due to not being a core tile\n\n";
        }
        text += "This has the following effects:\n";
        text += "Max effective population<sprite index=4> = " + Mathf.Round(data.control * 100f) / 100f + "% Max population<sprite index=4>\n";
        hoverText.text = text;
    }
    void SetDevCostText()
    {
        TileData data = Player.myPlayer.selectedTile;
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.CivID != data.civID) { return; }
        HoverText hoverText = devCostText.GetComponent<HoverText>();
        string text = "It will cost " + data.GetDevCost() + " Power<sprite index=1><sprite index=2><sprite index=3> to develop this tile\n\n";
        text += "This is due to the following: \n\n";
        text += "The Base Cost of developing " + (int)(50 * (1f + data.localDevCostMod.value + civ.devCostMod.value)) + "\n";
        text += "Local Modifiers:" + data.localDevCostMod.ToString() + "\n";
        text += "Global Modifiers:" + civ.devCostMod.ToString() + "\n\n";
        text += "Multiplied by:\n";
        text += "Local Modifiers:"+ data.localDevCost.ToString() + "\n";
        text += "Global Modifiers:" + civ.devCost.ToString() + "\n\n";
        text += "With a minimum of 10% of the base cost";
        hoverText.text = text;
    }
}
