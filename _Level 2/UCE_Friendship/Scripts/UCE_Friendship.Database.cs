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
    class character_friends
    {
        public string friendName { get; set; }
        public string lastGifted { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Connect_UCE_Friendlist
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Friendlist()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS 'character_friends' (
                        character TEXT NOT NULL,
                        friendName TEXT NOT NULL,
                        lastGifted TEXT
        )");
    }

    // -----------------------------------------------------------------------------------
    // ´CharacterLoad_UCE_Friendlist
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_Friendlist(Player player)
    {
        var table = connection.Query<character_friends>(
            "SELECT friendName, lastGifted FROM character_friends WHERE character = ?", player.name);
        if (table.Count > 0)
        {
            foreach (var row in table)
            {
                UCE_Friend frnd = new UCE_Friend(row.friendName, row.lastGifted);
                player.UCE_Friends.Add(frnd);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_Friendlist
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_Friendlist(Player player)
    {
        connection.Execute("DELETE FROM character_friends WHERE character = ?", player.name);

        foreach (var frnd in player.UCE_Friends)
        {
            connection.Execute("INSERT INTO character_friends VALUES (?, ?, ?)", player.name, frnd.name, frnd.lastGifted);
        }
    }

    // -----------------------------------------------------------------------------------
}