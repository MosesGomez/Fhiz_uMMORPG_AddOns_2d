// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================
// =======================================================================================
// DATABASE (SQLite / mySQL Hybrid)
// =======================================================================================

public partial class Database
{
    class character_factions
    {
        public string faction { get; set; }
        public int rating { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Factions
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Factions()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS character_factions (
        				character TEXT NOT NULL,
        				faction TEXT NOT NULL,
        				rating INTEGER
        				)");
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Factions
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    public void CharacterLoad_UCE_Factions(Player player)
    {
        var table = connection.Query<character_factions>(
            "SELECT faction, rating FROM character_factions WHERE character = ?", player.name);

        foreach (var row in table)
        {
            UCE_Faction faction = new UCE_Faction {name = row.faction, rating = row.rating};
            player.UCE_Factions.Add(faction);
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Factions
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    public void CharacterSave_UCE_Factions(Player player)
    {
        connection.Execute("DELETE FROM character_factions WHERE character = ?", player.name);
        foreach (UCE_Faction faction in player.UCE_Factions)
            connection.Execute("INSERT INTO character_factions VALUES (?, ?, ?)",
                player.name, faction.name, faction.rating);
    }
    // -----------------------------------------------------------------------------------
}