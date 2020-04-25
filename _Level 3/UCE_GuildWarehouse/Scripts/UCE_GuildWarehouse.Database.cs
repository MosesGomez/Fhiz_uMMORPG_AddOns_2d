// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

using System;
// =======================================================================================
// DATABASE (SQLite)
// =======================================================================================
public partial class Database
{
    //Models
    class guild_warehouse
    {
        public string guild { get; set; }
        public int gold { get; set; }
        public int level { get; set; }
        public int locked { get; set; }
        public int busy { get; set; }
    }

    class guild_warehouse_items
    {
        public string guild { get; set; }
        public int slot { get; set; }
        public string name { get; set; }
        public int amount { get; set; }
        public int summonedHealth { get; set; }
        public int summonedLevel { get; set; }
        public int summonedExperience { get; set; }
    }
    // -----------------------------------------------------------------------------------
    // Connect_UCE_GuildWareHouse
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_GuildWareHouse()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS 'UCE_guild_warehouse' (
							'guild' TEXT NOT NULL PRIMARY KEY,
							'gold' INTEGER NOT NULL DEFAULT 0,
							'level' INTEGER NOT NULL DEFAULT 0,
							'locked' INTEGER NOT NULL DEFAULT 0,
							'busy' INTEGER NOT NULL DEFAULT 0)");

        connection.Execute(@"CREATE TABLE IF NOT EXISTS 'UCE_guild_warehouse_items' (
                           'guild' TEXT NOT NULL,
                           'slot' INTEGER NOT NULL,
                           'name' TEXT NOT NULL,
                           'amount' INTEGER NOT NULL,
                           'summonedHealth' INTEGER NOT NULL,
                           'summonedLevel' INTEGER NOT NULL,
                           'summonedExperience' INTEGER NOT NULL)");
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_GuildWarehouse
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_GuildWarehouse(Player player)
    {
        UCE_SaveGuildWarehouse(player);
    }

    // -----------------------------------------------------------------------------------
    // UCE_LoadGuildWarehouse
    // -----------------------------------------------------------------------------------
    public void UCE_LoadGuildWarehouse(Player player)
    {
        player.resetGuildWarehouse();
        if (!player.InGuild()) return;

        var UCE_warehouseData = connection.Query<guild_warehouse>("SELECT gold, level, locked FROM UCE_guild_warehouse WHERE guild = ?", player.guild.name);

        // -- exists already? load to player and set busy

        if (UCE_warehouseData.Count == 1)
        {
            player.guildWarehouseGold = UCE_warehouseData[0].gold;
            player.guildWarehouseLevel = UCE_warehouseData[0].level;
            player.guildWarehouseLock = (UCE_warehouseData[0].locked == 1);
            connection.Execute("UPDATE UCE_guild_warehouse SET busy=1 WHERE guild = ?", player.guild.name);
        }
        else
        {
            // -- does not exist? create new and set busy

            connection.Execute("INSERT INTO UCE_guild_warehouse (guild, gold, level, locked, busy) VALUES(?, 0, 0, 0, 1)", player.guild.name);
            player.guildWarehouseGold = 0;
            player.guildWarehouseLevel = 0;
            player.guildWarehouseLock = false;
        }

        for (int i = 0; i < player.guildWarehouseStorageItems; ++i)
        {
            player.UCE_guildWarehouse.Add(new ItemSlot());
        }

        var table = connection.Query<guild_warehouse_items>("SELECT name, slot, amount, summonedHealth, summonedLevel, summonedExperience FROM UCE_guild_warehouse_items WHERE guild = ?", player.guild.name);

        if (table.Count > 0)
        {
            foreach (var row in table)
            {
                string itemName = row.name;
                int slot = row.slot;
                ScriptableItem template;
                if (slot < player.guildWarehouseStorageItems && ScriptableItem.dict.TryGetValue(itemName.GetStableHashCode(), out template))
                {
                    Item item = new Item(template);
                    int amount = row.amount;
                    item.summonedHealth = row.summonedHealth;
                    item.summonedLevel = row.summonedLevel;
                    item.summonedExperience = row.summonedExperience;
                    player.UCE_guildWarehouse[slot] = new ItemSlot(item, amount);
                }
            }
        }

        player.guildWarehouseActionDone = false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SaveGuildWarehouse
    // -----------------------------------------------------------------------------------
    public void UCE_SaveGuildWarehouse(Player player)
    {
        if (!player.InGuild()) player.resetGuildWarehouse();
        if (!player.InGuild() || !player.guildWarehouseActionDone) return;

        long EntryExistsOrBusy = connection.ExecuteScalar<long>("SELECT busy FROM UCE_guild_warehouse WHERE guild = ?", player.guild.name);

        // -- check if exists, only delete entries when it does and is not busy

        if (EntryExistsOrBusy != 1)
            connection.Execute("DELETE FROM UCE_guild_warehouse_items WHERE guild = ?", player.guild.name);

        for (int i = 0; i < player.UCE_guildWarehouse.Count; ++i)
        {
            ItemSlot slot = player.UCE_guildWarehouse[i];

            if (slot.amount > 0)
            {
                connection.Execute("INSERT INTO UCE_guild_warehouse_items VALUES (?, ?, ?, ?, ?, ?, ?)",
                                 player.guild.name,
                                i,
                                slot.item.name,
                                slot.amount,
                                slot.item.summonedHealth,
                                slot.item.summonedLevel,
                                slot.item.summonedExperience);
            }
        }

        connection.Execute("UPDATE UCE_guild_warehouse SET gold = ?, level = ?, locked = ?, busy = 0 WHERE guild = ?",
             player.guildWarehouseGold, player.guildWarehouseLevel, player.guildWarehouseLock ? 1 : 0, player.guild.name);
    }

    // -----------------------------------------------------------------------------------
    // UCE_SetGuildWarehouseBusy
    // -----------------------------------------------------------------------------------
    public void UCE_SetGuildWarehouseBusy(Player player, int isbusy = 1)
    {
        connection.Execute("UPDATE UCE_guild_warehouse SET busy = ? WHERE guild = ?",player.guild.name, isbusy);
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetGuildWarehouseAccess
    // -----------------------------------------------------------------------------------
    public bool UCE_GetGuildWarehouseAccess(Player player)
    {
        return connection.ExecuteScalar<int>("SELECT busy FROM UCE_guild_warehouse WHERE guild = ?", player.guild.name) != 1;
    }

    // -----------------------------------------------------------------------------------
}