public partial class Database
{
    class loot_log
    {
        public int id_lootcrate { get; set; }
        public string account { get; set; }
    }
    
    // -----------------------------------------------------------------------------------
    // Connect_UCE_Lootcrate
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_Lootcrate()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS 'loot_log' (
						'account' TEXT NOT NULL PRIMARY KEY,
						'id_lootcrate' INTEGER NOT NULL)");
    }
    
    // -----------------------------------------------------------------------------------
    // RegisterLoot
    // -----------------------------------------------------------------------------------
    public void RegisterLoot(string account, int id_lootcrate)
    {
        if (string.IsNullOrWhiteSpace(account) || id_lootcrate <= 0) return;
        connection.Execute("INSERT INTO loot_log VALUES (?, ?)", account, id_lootcrate);
    }
    
    // -----------------------------------------------------------------------------------
    // CheckLoot
    // -----------------------------------------------------------------------------------
    public bool CheckLoot(string account, int id_lootcrate)
    {
        if (string.IsNullOrWhiteSpace(account) || id_lootcrate <= 0) return false;
        var result = connection.Query<loot_log>("SELECT account FROM loot_log WHERE account = ? AND id_lootcrate = ?", account, id_lootcrate);
        if (result.Count == 1)
        {
            // The loot has been opened by the user.
            return true;
        }

        return false;
    }

}