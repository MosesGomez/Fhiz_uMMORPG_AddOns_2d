// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

using System;
using System.Collections.Generic;

// =======================================================================================
// DATABASE (SQLite / mySQL Hybrid)
// =======================================================================================
public partial class Database
{
    // -----------------------------------------------------------------------------------
    // Connect_UCE_Quest
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Quest()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS character_UCE_quests (
                            character TEXT NOT NULL,
                            name TEXT NOT NULL,
                            pvped TEXT NOT NULL,
                            killed TEXT NOT NULL,
                            gathered TEXT NOT NULL,
                            harvested TEXT NOT NULL,
                            visited TEXT NOT NULL,
                            crafted TEXT NOT NULL,
                            looted TEXT NOT NULL,
                            completed INTEGER NOT NULL,
                            completedAgain INTEGER NOT NULL,
                            lastCompleted TEXT NOT NULL,
                            counter INTEGER NOT NULL,
                            PRIMARY KEY(character, name))");
    }

    public class quest
    {
        public string name { get; set; }
        public string pvped { get; set; }
        public string killed { get; set; }
        public string gathered { get; set; }
        public string harvested { get; set; }
        public string visited { get; set; }
        public string crafted { get; set; }
        public string looted { get; set; }
        public string lastCompleted { get; set; }
        public int completed { get; set; }
        public int completedAgain { get; set; }
        public int counter { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_Quest
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Quest(Player player)
    {
        var table = connection.Query<quest>(
            "SELECT name, pvped, killed, gathered, harvested, visited, crafted, looted, completed, completedAgain, lastCompleted, counter FROM character_UCE_quests WHERE character = ?",
            player.name);
        foreach (var row in table)
        {
            string questName = row.name;
            UCE_ScriptableQuest questData;
            if (UCE_ScriptableQuest.dict.TryGetValue(questName.GetStableHashCode(), out questData))
            {
                UCE_Quest quest = new UCE_Quest(questData);

                quest.pvpedTarget = UCE_Tools.IntStringToArray(row.pvped);
                quest.killedTarget = UCE_Tools.IntStringToArray(row.killed);
                quest.gatheredTarget = UCE_Tools.IntStringToArray(row.gathered);
                quest.harvestedTarget = UCE_Tools.IntStringToArray(row.harvested);
                quest.visitedTarget = UCE_Tools.IntStringToArray(row.visited);
                quest.craftedTarget = UCE_Tools.IntStringToArray(row.crafted);
                quest.lootedTarget = UCE_Tools.IntStringToArray(row.looted);

                foreach (int i in quest.visitedTarget)
                    if (i != 0)
                        quest.visitedCount++;

                quest.completed = row.completed != 0; // sqlite has no bool
                quest.completedAgain = row.completedAgain != 0; // sqlite has no bool
                quest.lastCompleted = row.lastCompleted;
                quest.counter = row.counter;
                player.UCE_quests.Add(quest);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Quest
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Quest(Player player)
    {
        // quests: remove old entries first, then add all new ones
        connection.Execute("DELETE FROM character_UCE_quests WHERE character = ?", player.name);
        foreach (UCE_Quest quest in player.UCE_quests)
            connection.Execute("INSERT INTO character_UCE_quests VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                player.name,
                quest.name,
                UCE_Tools.IntArrayToString(quest.killedTarget),
                UCE_Tools.IntArrayToString(quest.killedTarget),
                UCE_Tools.IntArrayToString(quest.gatheredTarget),
                UCE_Tools.IntArrayToString(quest.harvestedTarget),
                UCE_Tools.IntArrayToString(quest.visitedTarget),
                UCE_Tools.IntArrayToString(quest.craftedTarget),
                UCE_Tools.IntArrayToString(quest.lootedTarget),
                quest.completed,
                quest.completedAgain,
                quest.lastCompleted,
                quest.counter);
    }

    // -----------------------------------------------------------------------------------
}