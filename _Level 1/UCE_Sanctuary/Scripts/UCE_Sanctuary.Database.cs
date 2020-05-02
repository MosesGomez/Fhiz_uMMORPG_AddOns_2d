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

using System;

public partial class Database
{
    // -----------------------------------------------------------------------------------
    // Connect_UCE_Sanctuary
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Sanctuary()
    {
	    connection.Execute(@"CREATE TABLE IF NOT EXISTS character_lastonline (
				character TEXT NOT NULL,
				lastOnline TEXT NOT NULL)");
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Sanctuary
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Sanctuary(Player player)
    {
        var row = connection.ExecuteScalar<string>("SELECT lastOnline FROM character_lastonline WHERE `character` = ?", player.name);
        if (!string.IsNullOrWhiteSpace(row))
        {
            DateTime time = DateTime.Parse(row);
            player.UCE_SecondsPassed = (DateTime.UtcNow - time).TotalSeconds;
        }
        else
        {
            player.UCE_SecondsPassed = 0;
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Sanctuary
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Sanctuary(Player player)
    {
        connection.Execute("DELETE FROM character_lastonline WHERE `character` = ?", player.name);
        connection.Execute("INSERT INTO character_lastonline VALUES (?, ?)",
                DateTime.UtcNow.ToString("s"),
                player.name);
    }

    // -----------------------------------------------------------------------------------
}