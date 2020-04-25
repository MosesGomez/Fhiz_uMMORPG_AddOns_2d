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
    // Models
    class character_currencies
    {
        public string character { get; set; }
        public string currency { get; set; }
        public int amount { get; set; }
        public int total { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Connect_UCE_HonorShop
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_HonorShop()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS character_currencies (
			character TEXT NOT NULL,
			currency TEXT NOT NULL,
			amount INTEGER NOT NULL,
			total INTEGER NOT NULL
		)");
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_HonorShop
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_HonorShop(Player player)
    {
        var table = connection.Query<character_currencies>(
            "SELECT currency, amount, total FROM character_currencies WHERE `character` = ?", player.name);
        foreach (var row in table)
        {
            string tmplName = row.currency;
            UCE_Tmpl_HonorCurrency tmplCurrency;

            if (UCE_Tmpl_HonorCurrency.dict.TryGetValue(tmplName.GetStableHashCode(), out tmplCurrency))
            {
                UCE_HonorShopCurrency hsc = new UCE_HonorShopCurrency
                {
                    honorCurrency = tmplCurrency, amount = row.amount, total = row.total
                };
                player.UCE_currencies.Add(hsc);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_HonorShop
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_HonorShop(Player player)
    {
        connection.Execute("DELETE FROM character_currencies WHERE `character` = ?", player.name);
        for (int i = 0; i < player.UCE_currencies.Count; ++i)
        {
            connection.Execute("INSERT INTO character_currencies VALUES (?, ?, ?, ?)",
                player.name,
                player.UCE_currencies[i].honorCurrency.name,
                player.UCE_currencies[i].amount,
                player.UCE_currencies[i].total
            );
        }
    }

    // -----------------------------------------------------------------------------------
}