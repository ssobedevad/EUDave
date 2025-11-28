using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultUI : MonoBehaviour
{
    [SerializeField] Button goTo, oK;
    [SerializeField] TextMeshProUGUI battleName;
    [SerializeField] TextMeshProUGUI attackerName, attackerSize, attackerLosses, attackerRemaining;
    [SerializeField] TextMeshProUGUI defenderName, defenderSize, defenderLosses, defenderRemaining;
    Vector3Int battlePos;

    public void SetUpText(string provName,string attackername, string defendername, int attackersize, int defendersize, int attackerlosses, int defenderlosses, Vector3Int battlepos)
    {
        battleName.text = provName;
        attackerName.text = attackername;
        defenderName.text = defendername;
        attackerSize.text = ""+attackersize;
        defenderSize.text = "" + defendersize;
        attackerLosses.text="-" + attackerlosses;
        defenderLosses.text = "-" + defenderlosses;
        attackerRemaining.text = "" + (attackersize - attackerlosses);
        defenderRemaining.text = "" + (defendersize - defenderlosses);
        battlePos = battlepos;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }
    private void Start()
    {
        oK.onClick.AddListener(Close);
        goTo.onClick.AddListener(GoTo);
    }
    void GoTo()
    {
        CameraController.main.rb.position = Map.main.tileMapManager.tilemap.CellToWorld(battlePos);
        Map.main.tileMapManager.SelectTile(battlePos);
        Destroy(gameObject);
    }
    void Close()
    {
        UIManager.main.UI.Remove(gameObject);
        Destroy(gameObject);
    }
}
