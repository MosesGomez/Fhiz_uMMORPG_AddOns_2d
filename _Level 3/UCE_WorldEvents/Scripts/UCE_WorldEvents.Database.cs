// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

using UnityEngine;
using System;
using System.Collections;

// =======================================================================================
// DATABASE (SQLite / mySQL Hybrid)
// =======================================================================================
public partial class Database
{
    public class worldevents
    {
        public string name { get; set; }
        public int count { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Connect_UCE_WorldEvents
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_WorldEvents()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS uce_worldevents (`name` TEXT NOT NULL, `count` INTEGER NOT NULL)");
    }

    // -----------------------------------------------------------------------------------
    // UCE_Load_WorldEvents
    // -----------------------------------------------------------------------------------
    public void UCE_Load_WorldEvents()
    {
        var table = connection.Query<worldevents>("SELECT `name`, `count` FROM uce_worldevents");
        foreach (var row in table)
        {
            string name = row.name;
            int count = row.count;

            if (!string.IsNullOrWhiteSpace(name) && count != 0)
            {
                NetworkManagerMMO.UCE_SetWorldEventCount(name, count);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // UCE_Save_WorldEvents
    // -----------------------------------------------------------------------------------
    public void UCE_Save_WorldEvents()
    {
        connection.Execute("DELETE FROM uce_worldevents");
        foreach (UCE_WorldEvent ev in NetworkManagerMMO.UCE_WorldEvents)
        {
            connection.Execute("INSERT INTO uce_worldevents VALUES (?, ?)",
                ev.name,
                ev.count
            );
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_WorldEvents
    // refresh the world event list once a character is loaded to populate it with data
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_WorldEvents(Player player)
    {
        player.UCE_WorldEvents.Clear();
        foreach (UCE_WorldEvent ev in NetworkManagerMMO.UCE_WorldEvents)
        {
            UCE_WorldEvent e = new UCE_WorldEvent();
            e.name = ev.name;
            e.count = ev.count;
            player.UCE_WorldEvents.Add(e);
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_WorldEvents
    // refresh the world event list every time a character is saved to keep it in sync
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_WorldEvents(Player player)
    {
        foreach (UCE_WorldEvent ev in NetworkManagerMMO.UCE_WorldEvents)
        {
            int id = player.UCE_WorldEvents.FindIndex(x => x.template == ev.template);

            if (id != -1)
            {
                UCE_WorldEvent e = player.UCE_WorldEvents[id];
                e.count = ev.count;
                player.UCE_WorldEvents[id] = e;
            }
        }

        // -- we save the world events as well here, but only if they changed and only once (not for every player)
        NetworkManagerMMO.UCE_SaveWorldEvents();
    }

    // -----------------------------------------------------------------------------------
}