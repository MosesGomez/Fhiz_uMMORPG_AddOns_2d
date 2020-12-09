// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

class character_pvpzones
{
    public string realm { get; set; }
    public string alliedrealm { get; set; }
}

// =======================================================================================
// DATABASE (SQLite / mySQL Hybrid)
// =======================================================================================
public partial class Database
{
    // -----------------------------------------------------------------------------------
    // Connect_UCE_PVPZone
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    public void Connect_UCE_PVPZone()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS character_pvpzones (
			character TEXT NOT NULL,
			realm TEXT NOT NULL,
			alliedrealm TEXT NOT NULL
		)");
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_PVPZone
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    public void CharacterLoad_UCE_PVPZone(Player player)
    {
        var table = connection.Query<character_pvpzones>("SELECT realm, alliedrealm FROM character_pvpzones WHERE `character` = ?", player.name);
        if (table.Count == 1)
        {
            string realm = table[0].realm;
            string ally = table[0].alliedrealm;
            player.UCE_setRealm(realm.GetStableHashCode(), ally.GetStableHashCode());
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_PVPZone
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    public void CharacterSave_UCE_PVPZone(Player player)
    {
        connection.Execute("DELETE FROM character_pvpzones WHERE `character` = ?", player.name);
        connection.Execute("INSERT INTO character_pvpzones VALUES (?, ?, ?)",player.name, (player.Realm != null) ? player.Realm.name : "", (player.Ally != null) ? player.Ally.name : "");
    }

    // -----------------------------------------------------------------------------------
}