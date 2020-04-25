// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

using System;

// =======================================================================================
// DATABASE (SQLite / mySQL Hybrid)
// =======================================================================================
public partial class Database
{
    // -----------------------------------------------------------------------------------
    // Connect_UCE_GuildUpgrades
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_GuildUpgrades()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS 'UCE_guild_upgrades' (
							'guild' TEXT NOT NULL PRIMARY KEY,
							'level' INTEGER NOT NULL DEFAULT 0)");
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_GuildUpgrades
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_GuildUpgrades(Player player)
    {
        UCE_SaveGuildUpgrades(player);
    }

    // -----------------------------------------------------------------------------------
    // UCE_LoadGuildUpgrades
    // -----------------------------------------------------------------------------------
    public void UCE_LoadGuildUpgrades(Player player)
    {
        if (!player.InGuild()) return;
        var guildLevel = connection.ExecuteScalar<int>("SELECT level FROM UCE_guild_upgrades WHERE guild = ?", player.guild.name);

        // -- exists already? load to player

        player.guildLevel = Convert.ToInt32((long)guildLevel);
        connection.Execute("UPDATE UCE_guild_upgrades SET busy=1 WHERE guild = ?", player.guild.name);
        player.guildWarehouseActionDone = false;
    }

    // -----------------------------------------------------------------------------------
    // UCE_SaveGuildUpgrades
    // -----------------------------------------------------------------------------------
    public void UCE_SaveGuildUpgrades(Player player)
    {
        if (!player.InGuild()) return;
        connection.RunInTransaction(() =>
        {
            connection.Execute("DELETE FROM UCE_guild_upgrades WHERE guild = ?", player.guild.name);
            connection.Execute("INSERT INTO UCE_guild_upgrades(guild, level) VALUES(?, ?)",
                player.guild.name, player.guildLevel);
        });
    }

    // -----------------------------------------------------------------------------------
}