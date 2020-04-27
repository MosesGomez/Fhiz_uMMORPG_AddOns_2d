// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

#if _iMMOHARVESTING

// =======================================================================================
// DATABASE (SQLite / mySQL Hybrid)
// =======================================================================================
public partial class Database
{
    class professions
    {
        public string profession { get; set; }
        public int experience { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Harvesting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Harvesting()
    {
        connection.Execute(
            @"CREATE TABLE IF NOT EXISTS character_professions ( character TEXT NOT NULL, profession TEXT NOT NULL, experience INTEGER)");
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Harvesting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    public void CharacterLoad_UCE_Harvesting(Player player)
    {
        var table = connection.Query<professions>(
            "SELECT profession, experience FROM character_professions WHERE character = ?", player.name);
        foreach (var row in table)
        {
            UCE_HarvestingProfession profession = new UCE_HarvestingProfession(row.profession);
            profession.experience = row.experience;
            player.UCE_Professions.Add(profession);
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Harvesting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    public void CharacterSave_UCE_Harvesting(Player player)
    {
        connection.Execute("DELETE FROM character_professions WHERE character = ?", player.name);
        foreach (var profession in player.UCE_Professions)
            connection.Execute("INSERT INTO character_professions VALUES (?, ?, ?)", player.name,
                profession.templateName, profession.experience);
    }

    // -----------------------------------------------------------------------------------
}

#endif