using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReleaseSubjectUI : MonoBehaviour
{
    [SerializeField] GameObject subjectPrefab;
    [SerializeField] Transform subjectTransform;
    [SerializeField] Button closeButton;
    List<GameObject> subjectList = new List<GameObject>();
    List<Civilisation> possibleSubjects = new List<Civilisation>();
    public void SetupList()
    {
        if (Player.myPlayer.myCivID == -1 || DiplomacyUIPanel.main.diploCivID != Player.myPlayer.myCivID) { return; }
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        List<int> cores = new List<int>();
        foreach(var prov in civ.GetAllCivTileDatas())
        {
            List<int> provCores = prov.cores.FindAll(i=>!cores.Contains(i) && i != civ.CivID && !Game.main.civs[i].isActive());
            cores.AddRange(provCores);
        }
        possibleSubjects = cores.ConvertAll(i => Game.main.civs[i]);
        while (subjectList.Count != cores.Count)
        {
            if (subjectList.Count > cores.Count)
            {
                int lastIndex = subjectList.Count - 1;
                Destroy(subjectList[lastIndex]);
                subjectList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(subjectPrefab, subjectTransform);
                int index = subjectList.Count;
                item.GetComponentInChildren<Button>().onClick.AddListener(delegate { ReleaseSubject(index); });
                subjectList.Add(item);
            }
        }
        for (int i = 0; i < cores.Count; i++)
        {
            Civilisation subject = Game.main.civs[cores[i]];
            Image[] images = subjectList[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = subjectList[i].GetComponentsInChildren<TextMeshProUGUI>();
            images[1].color = civ.c;
            texts[0].text = subject.civName;
        }
    }
    void ReleaseSubject(int index)
    {
        if (Player.myPlayer.myCivID == -1 || DiplomacyUIPanel.main.diploCivID != Player.myPlayer.myCivID) { return; }
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        if (possibleSubjects[index].isActive()) { return; }
        Civilisation released = possibleSubjects[index];
        List<TileData> releaseTiles = new List<TileData>();
        foreach (var prov in civ.GetAllCivTileDatas())
        {
            if (prov.cores.Contains(released.CivID) && civ.capitalPos != prov.pos)
            {
                releaseTiles.Add(prov);
            }
        }
        foreach(var releaseTile in releaseTiles)
        {
            releaseTile.civID = released.CivID;
            releaseTile.control = 100;
            if (releaseTile.cores.Contains(civ.CivID))
            {
                releaseTile.cores.Remove(civ.CivID);
            }
            if (civ.claims.Contains(releaseTile.pos))
            {
                civ.claims.Remove(releaseTile.pos);
            }
        }
        civ.updateBorders = true;
        released.updateBorders = true;
        civ.Subjugate(released);
        gameObject.SetActive(false);
    }
}
