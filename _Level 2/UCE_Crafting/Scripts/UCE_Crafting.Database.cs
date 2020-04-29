// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

#if _iMMOCRAFTING

// =======================================================================================
// DATABASE (SQLite / mySQL Hybrid)
// =======================================================================================
public partial class Database
{
    class character_crafts
    {
        public string profession { get; set; }
        public int experience { get; set; }
    }

    class character_recipes
    {
        public string recipe { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Crafting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Crafting()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS character_crafts (
			character TEXT NOT NULL,
			profession TEXT NOT NULL,
			experience INTEGER
		)");

        connection.Execute(@"CREATE TABLE IF NOT EXISTS character_recipes (
			character TEXT NOT NULL,
			recipe TEXT NOT NULL
		)");
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Crafting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Crafting(Player player)
    {
        var table = connection.Query<character_crafts>(
            "SELECT profession, experience FROM character_crafts WHERE character = ?",
            player.name);

        foreach (var row in table)
        {
            UCE_CraftingProfession profession = new UCE_CraftingProfession(row.profession);
            profession.experience = row.experience;
            player.UCE_Crafts.Add(profession);
        }

        var table2 = connection.Query<character_recipes>("SELECT recipe FROM character_recipes WHERE `character` = ?",
            player.name);
        foreach (var row in table2)
        {
            player.UCE_recipes.Add(row.recipe);
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Crafting
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Crafting(Player player)
    {
        connection.Execute("DELETE FROM character_crafts WHERE character = ?", player.name);
        foreach (var profession in player.UCE_Crafts)
            connection.Execute("INSERT INTO character_crafts VALUES (?, ?, ?)",
                player.name,
                profession.templateName,
                profession.experience);

        connection.Execute("DELETE FROM character_recipes WHERE `character` = ?", player.name);
        for (int i = 0; i < player.UCE_recipes.Count; ++i)
        {
            connection.Execute("INSERT INTO character_recipes VALUES (?, ?)",
                player.name,
                player.UCE_recipes[i]);
        }
    }

    // -----------------------------------------------------------------------------------
}
#endif