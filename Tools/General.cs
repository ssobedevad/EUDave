using System;
using System.Collections.Generic;

using UnityEngine;

public class General
{
    public string name;
    public int meleeSkill;
    public int flankingSkill;
    public int rangedSkill;
    public int siegeSkill;
    public int maneuverSkill;
    public Effect[] traits;
    public bool active;
    public Army army;
    public Age age;

    public General(Age age)
    {
        this.age = age;
        Game.main.monthTick.AddListener(CheckDeath);
        active = true;
    }

    public int Stars()
    {
        int count = 1;
        count += (meleeSkill + flankingSkill + rangedSkill + siegeSkill + maneuverSkill) / 6;
        return count;
    }
    public void Kill()
    {
        age.DeActivate();
        active = false;
        Game.main.dayTick.RemoveListener(CheckDeath);
    }
    void CheckDeath()
    {
        if (age.months > 0 || age.years > 0)
        {
            if (UnityEngine.Random.Range(0f, 50f) < age.months + age.years * 12)
            {
                Kill();
            }
        }
    }
}
