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

using UnityEngine;

public partial class Database
{
    // MODELS
    class character_bindpoint
    {
        public string name { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public string sceneName { get; set; }
    }
    // -----------------------------------------------------------------------------------
    // Connect_UCE_Bindpoint
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Bindpoint()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS character_bindpoint (
					character TEXT NOT NULL,
					name TEXT NOT NULL,
					x REAL NOT NULL,
            		y REAL NOT NULL,
            		z REAL NOT NULL,
            		sceneName TEXT NOT NULL
                )");
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Bindpoint
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Bindpoint(Player player)
    {
        if (!player.UCE_myBindpoint.Valid) return;

        connection.Execute("DELETE FROM character_bindpoint WHERE `character` = ?", player.name);
        connection.Execute("INSERT INTO character_bindpoint VALUES (?, ?, ?, ?, ?, ?)",
            player.name,
            player.UCE_myBindpoint.name,
            player.UCE_myBindpoint.position.x,
            player.UCE_myBindpoint.position.y,
            player.UCE_myBindpoint.position.z,
            player.UCE_myBindpoint.SceneName);
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Bindpoint
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Bindpoint(Player player)
    {
        player.UCE_myBindpoint = new UCE_BindPoint();
        
        var table = connection.Query<character_bindpoint>("SELECT name, x, y, z, sceneName FROM character_bindpoint WHERE character = ?",
            player.name);
        if (table.Count == 1)
        {
            var row = table[0];

            Vector3 p = new Vector3(row.x, row.y, row.z);
            string sceneName = row.sceneName;

            if (p != Vector3.zero && !string.IsNullOrEmpty(sceneName))
            {
                player.UCE_myBindpoint.name = row.name;
                player.UCE_myBindpoint.position = p;
                player.UCE_myBindpoint.SceneName = sceneName;
            }
        }
    }

    // -----------------------------------------------------------------------------------
}