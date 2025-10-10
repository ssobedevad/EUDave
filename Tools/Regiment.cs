using System;
using UnityEngine;

[Serializable]
public class Regiment
{
    public int maxSize;
    public int type;
    public int size;
    public float morale;
    public float maxMorale;
    public int civID;
    public int flankingRange;
    public float meleeDamage;
    public float flankingDamage;
    public float rangedDamage;
    public bool inBatle;
    public Regiment(int CivID,int MaxSize = 1000,int Size = 1000, int Type = 0)
    {
        maxSize = MaxSize;
        size = Size;
        type = Type;
        morale = 2.5f;
        maxMorale = 2.5f;
        meleeDamage = 0.25f;
        flankingDamage = 0.2f;
        rangedDamage = 0.1f;
        flankingRange = 1;
        civID = CivID;
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            maxMorale = civ.moraleMax.value;
            morale = maxMorale;           
        }
        inBatle = false;
        Game.main.dayTick.AddListener(DayTick);
    }
    public Regiment(Regiment clone)
    {
        if (clone == null)
        {
            civID = -1;
            maxSize = 0;
            size = 0;
            type = -1;
            morale = 0f;
            maxMorale = 0f;
            meleeDamage = 0f;
            flankingRange = 0;
            inBatle = false;
        }
        else
        {
            civID = clone.civID;
            maxSize = clone.maxSize;
            size = clone.size;
            type = clone.type;
            morale = clone.morale;
            flankingRange = clone.flankingRange;
            maxMorale = clone.maxMorale;
            meleeDamage = clone.meleeDamage;
            inBatle = clone.inBatle;
        }
    }

    public void DayTick()
    {
        if (!inBatle && type > -1 && civID > -1)
        {
            RecoverMorale();
            RefillRegiment();
        }
    }
    void RecoverMorale()
    {

        Civilisation civ = Game.main.civs[civID];
        float recovery = 0.15f + civ.moraleRecovery.value;
        Army army = civ.armies.Find(i=>i.regiments.Contains(this));
        if(army != null) { if (army.tile.civID == civID) { recovery += 0.05f; } }
        morale = Mathf.Min(maxMorale, morale + maxMorale * recovery);
    }
    void RefillRegiment()
    {
        Civilisation civ = Game.main.civs[civID];
        int reinforce = (int)(100 * (1f + civ.reinforceSpeed.value));
        int targetAmount = Mathf.Min(reinforce, maxSize - size);
        int realAmount = civ.RemovePopulation(targetAmount);
        //Debug.Log(targetAmount + " / " + realAmount);
        size += realAmount;
    }
    public void TakeCasualties(int casualties,float averageEnemyMaxMorale)
    {      
        size = Mathf.Max(0,size - casualties);
        morale = Mathf.Max(0, morale - casualties * maxMorale/540f);
    }
    public void TakeReserveMoraleDamage(float averageEnemyMaxMorale)
    {
        morale = Mathf.Max(0, morale - averageEnemyMaxMorale * 0.02f);
    }
    public void TakeFrontlineMoraleDamage(float averageEnemyMaxMorale)
    {
        morale = Mathf.Max(0, morale - averageEnemyMaxMorale * 0.01f);
    }
}
