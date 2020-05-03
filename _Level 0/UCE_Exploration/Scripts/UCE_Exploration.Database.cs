// =======================================================================================
// Created and maintained by iMMO
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
    // MODELS
    class character_exploration
    {
        public string exploredArea { get; set; }
    }
    
    // -----------------------------------------------------------------------------------
    // Connect_UCE_Exploration
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Exploration()
    {
        connection.Execute("CREATE TABLE IF NOT EXISTS character_exploration (character TEXT NOT NULL, exploredArea TEXT NOT NULL)");
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Exploration
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Exploration(Player player)
    {
        var table = connection.Query<character_exploration>("SELECT exploredArea FROM character_exploration WHERE `character` = ?", player.name);
        foreach (var row in table)
        {
            player.UCE_exploredAreas.Add(row.exploredArea);
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Exploration
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Exploration(Player player)
    {
        connection.Execute("DELETE FROM character_exploration WHERE `character` = ?", player.name);
        foreach (var t in player.UCE_exploredAreas)
        {
            connection.Execute("INSERT INTO character_exploration VALUES (?, ?)",
                player.name, t);
        }
    }

    // -----------------------------------------------------------------------------------
}