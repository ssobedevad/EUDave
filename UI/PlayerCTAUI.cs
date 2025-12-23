using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCTAUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Button accept, decline;
    public War war;
    public bool joinAsAttackerAlly;
    private void Awake()
    {
        accept.onClick.AddListener(Accept);
        decline.onClick.AddListener(Decline);
    }
    void Accept()
    {
        if(Player.myPlayer.myCivID == -1) { return; }
        if (Game.main.isMultiplayer)
        {
            if (war.networkWar != null)
            {
                war.networkWar.JoinWarRpc(Player.myPlayer.myCivID,joinAsAttackerAlly);
            }
        }
        else
        {
            war.JoinWar(Player.myPlayer.myCiv, joinAsAttackerAlly);
        }
        Destroy(gameObject);
    }
    void Decline()
    {

        if (Player.myPlayer.myCivID == -1) { return; }
        Player.myPlayer.myCiv.AddPrestige(-25);
        Player.myPlayer.myCiv.diploRep.AddModifier(new Modifier(-1, 1, "Declined CTA", 6 * 24 * 30 * 6));
        Player.myPlayer.myCiv.BreakAlliance((joinAsAttackerAlly ? war.attackerCiv.CivID : war.defenderCiv.CivID));
        Destroy(gameObject);
    }
    public void SetupDescription()
    {
        string descriptionText = "";
        descriptionText += "Your ally of " + (joinAsAttackerAlly? war.attackerCiv.civName : war.defenderCiv.civName) + " requests aid in the " + war.GetName() + "\n\n";
        descriptionText += (joinAsAttackerAlly ? "This is a war they have started" : "This is a defensive war") + "\n\n";
        descriptionText += "Declining would result in the alliance breaking and a hit of -25<sprite index=5> and -1 diplo rep<sprite index=2> for 6 months.";
        description.text = descriptionText;
    }
}
