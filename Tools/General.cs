using MessagePack;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[Serializable]
public class General
{
    public string name;
    public int combatSkill;
    public int siegeSkill;
    public int maneuverSkill;
    public Effect[] traits;
    public bool active;
    public bool equipped;
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
        count += (combatSkill + siegeSkill + maneuverSkill) / 3;
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
        if (Game.main.isMultiplayer && !NetworkManager.Singleton.IsServer) { return; }
        if (age.m > 0 || age.y > 0)
        {
            if (UnityEngine.Random.Range(0f, 50f) < age.m + age.y * 12)
            {               
                 Kill();                
            }
        }
    }
}
