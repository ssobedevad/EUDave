using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class EventData
{
    public string Name;
    public string Desc;
    public string[] options;
    public bool affectsRandomProvince;
    public bool affectsCapital;
    public TileData province;
    public EventOption[] optionEffects;
    public Condition[] conditions;

    public bool CanFire(Civilisation civ)
    {
        if (conditions.Length > 0)
        {
            foreach (var condition in conditions)
            {
                bool met = condition.isMet(civ);
                if (!met) { return false; }
            }
        }
        return true;
    }
    public string optionDescription(int option)
    {
        Civilisation civilisation = Player.myPlayer.myCiv;
        string desc = "";
        EventOption opt = optionEffects[option];
        if (opt.provinceModifier)
        {
            desc += "The Following Effects on " + province.Name + ":\n\n";
            if (opt.devA != 0)
            {
                desc += (opt.devA > 0 ? "+" : "") + opt.devA + " Admin Development<sprite index=1>\n";
            }
            if (opt.devB != 0)
            {
                desc += (opt.devB > 0 ? "+" : "") + opt.devB + " Diplo Development<sprite index=2>\n";
            }
            if (opt.devC != 0)
            {
                desc += (opt.devC > 0 ? "+" : "") + opt.devC + " Mil Development<sprite index=3>\n";
            }
            if (opt.population != 0)
            {
                desc += (opt.population > 0 ? "+" : "") + opt.population + " population<sprite index=4>\n";
            }
            for (int i = 0; i < opt.effects.Length; i++)
            {
                desc += opt.effects[i].name + Modifier.ToString(opt.effects[i].amount, province.GetStat(opt.effects[i].name)) + " for " + Mathf.Round(opt.effects[i].duration * 10f / 60f) / 10f + " days\n";
            }
        }
        else
        {
            desc += "The Following Effects on the nation:\n\n";
            for (int i = 0; i < opt.effects.Length; i++)
            {
                desc += opt.effects[i].name + Modifier.ToString(opt.effects[i].amount, civilisation.GetStat(opt.effects[i].name)) +" for " + Mathf.Round(opt.effects[i].duration * 10f/60f) / 10f + " days\n";
            }
        }

        if (opt.coins != 0)
        {
            desc += (opt.coins > 0 ? "+" : "") + opt.coins + " coins<sprite index=0>\n";
        }
        if (opt.manaA != 0)
        {
            desc += (opt.manaA > 0 ?"+": "")+ opt.manaA + " Admin Power<sprite index=1>\n";
        }
        if (opt.manaB != 0)
        {
            desc += (opt.manaB > 0 ? "+" : "") + opt.manaB + " Diplo Power<sprite index=2>\n";
        }
        if (opt.manaC != 0)
        {
            desc += (opt.manaC > 0 ? "+" : "") + opt.manaC + " Mil Power<sprite index=3>\n";
        }
        if (opt.prestige != 0)
        {
            desc += (opt.prestige > 0 ? "+" : "") + opt.prestige + " Prestige<sprite index=5>\n";
        }
        if (opt.govReformProgress != 0)
        {
            desc += (opt.govReformProgress > 0 ? "+" : "") + opt.govReformProgress + " Government Reform Progress<sprite index=4>\n";
        }
        if (opt.stability != 0)
        {
            desc += (opt.stability > 0 ? "+" : "") + opt.stability + " Stability<sprite index=6>\n";
        }
        if (opt.coinsYIncomePercent != 0)
        {
            desc += (opt.coinsYIncomePercent > 0 ? "+" : "") + (civilisation.ProductionIncome() + civilisation.TaxIncome()) * 12f * opt.coinsYIncomePercent + " coins<sprite index=0>\n";
        }
        return desc;
    }
   
}
