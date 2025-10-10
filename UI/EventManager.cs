using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Title, Description;
    [SerializeField] Transform target;
    [SerializeField] GameObject optionPrefab;

    public EventData eventData;

    private void Start()
    {
        UIManager.main.UI.Add(gameObject);
        for(int i = 0; i < eventData.options.Length;i++)
        {
            string option = eventData.options[i];
            int index = i;
            GameObject button = Instantiate(optionPrefab, target);
            button.GetComponentInChildren<TextMeshProUGUI>().text = option;
            button.GetComponent<Button>().onClick.AddListener(delegate { ButtonClick(index); });
            button.GetComponent<HoverText>().text = eventData.optionDescription(i);
        }
    }
    private void OnGUI()
    {
        Title.text = eventData.Name;
        Description.text = eventData.Desc;
    }
    void ButtonClick(int id)
    {
        EventOption selected = eventData.optionEffects[id];
        TakeOption(selected);
        Game.main.paused = false;
        UIManager.main.UI.Remove(gameObject);
        Destroy(gameObject);
    }
    void TakeOption(EventOption option)
    {
        Civilisation civilisation = Player.myPlayer.myCiv;
        if (option.provinceModifier)
        {
            for (int i = 0; i < option.Effect.Length; i++)
            {
                eventData.province.ApplyTileLocalModifier(option.Effect[i], option.EffectStrength[i], option.EffectType[i], eventData.Name, option.EffectDuration[i]);
            }
            if(option.devA != 0)
            {
                eventData.province.developmentA += option.devA;
            }
            if (option.devB != 0)
            {
                eventData.province.developmentB += option.devB;
            }
            if (option.devC != 0)
            {
                eventData.province.developmentC += option.devC;
            }
            if (option.population != 0)
            {
                eventData.province.population = (int)Mathf.Clamp(eventData.province.population + option.population, 0, eventData.province.maxPopulation);
                eventData.province.avaliablePopulation = (int)Mathf.Clamp(eventData.province.avaliablePopulation + (option.population * eventData.province.control/100f), 0, eventData.province.avaliableMaxPopulation);
            }
        }
        else
        {
            for(int i = 0; i < option.Effect.Length; i++) 
            {
                civilisation.ApplyCivModifier(option.Effect[i], option.EffectStrength[i], eventData.Name, option.EffectType[i], option.EffectDuration[i]);
            }
        }
        if (option.manaA != 0)
        {
            civilisation.adminPower += option.manaA;
        }
        if (option.manaB != 0)
        {
            civilisation.diploPower += option.manaB;
        }
        if (option.manaC != 0)
        {
            civilisation.milPower += option.manaC;
        }
        if (option.stability != 0)
        {
            civilisation.AddStability(option.stability);
        }
        if (option.prestige != 0)
        {
            civilisation.AddPrestige(option.prestige);
        }
        if (option.coins != 0)
        {
            civilisation.coins += option.coins;
        }
        if(option.coinsYIncomePercent != 0)
        {
            civilisation.coins += (civilisation.ProductionIncome() + civilisation.TaxIncome()) * 12f * option.coinsYIncomePercent;
        }
    }
}
