// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

// =======================================================================================
// DATABASE
// =======================================================================================

using System;
using System.Collections.Generic;

public partial class Database
{
    // MODELS
    public class mail
    {
        public int id { get; set; }
        public string messageFrom { get; set; }
        public string messageTo { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public int sent { get; set; }
        public int expires { get; set; }
        public int read { get; set; }
        public int deleted { get; set; }
        public string item { get; set; }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    public void Connect_Mail()
    {
        connection.Execute(@"CREATE TABLE IF NOT EXISTS mail (
							id INTEGER PRIMARY KEY,
							messageFrom TEXT NOT NULL,
							messageTo TEXT NOT NULL,
							subject TEXT NOT NULL,
							body TEXT NOT NULL,
							sent INTEGER NOT NULL,
							expires INTEGER NOT NULL,
							read INTEGER NOT NULL,
							deleted INTEGER NOT NULL,
							item TEXT NOT NULL
							)");
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public MailMessage Mail_BuildMessageFromDBRow(mail row)
    {
        MailMessage message = new MailMessage
        {
            id = row.id,
            @from = row.messageFrom,
            to = row.messageTo,
            subject = row.subject,
            body = row.body,
            sent = row.sent,
            expires = row.expires,
            read = row.read,
            deleted = row.deleted
        };

        string name = row.item;
        if (ScriptableItem.dict.TryGetValue(name.GetStableHashCode(), out ScriptableItem itemData))
            message.item = itemData;

        return message;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    public void CharacterLoad_Mail(Player player)
    {
        var table = connection.Query<mail>(
            "SELECT * FROM mail WHERE messageTo=? AND deleted=0 AND expires > ? ORDER BY sent",
            player.name, Epoch.Current());
        foreach (var row in table)
        {
            MailMessage message = Mail_BuildMessageFromDBRow(row);
            player.mailMessages.Add(message);
        }
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public List<MailSearch> Mail_SearchForCharacter(string name, string selfPlayer)
    {
        List<MailSearch> result = new List<MailSearch>();

        var table = connection.Query<characters>($@"SELECT `name`, level FROM characters
										LEFT JOIN character_guild
											ON character=name
									WHERE name LIKE '%' || '{name}' || '%'
										AND name <> '{selfPlayer} '
									ORDER BY
										CASE
											WHEN name='{name}' THEN 0
											ELSE INSTR(LOWER(name), LOWER('{name}'))
										END, name
									LIMIT 30");

        foreach (var row in table)
        {
            MailSearch res = new MailSearch();
            res.name = row.name;
            res.level = row.level;
            res.guild = "";

            result.Add(res);
        }

        return result;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void Mail_CreateMessage(string from, string to, string subject, string body, string itemName,
        long expiration = 0)
    {
        long sent = Epoch.Current();
        long expires = 0;
        if (expiration > 0)
        {
            expires = sent + expiration;
        }

        if (itemName == null) itemName = "";
        connection.Execute(
            "INSERT INTO mail (messageFrom, messageTo, subject, body, sent, expires, read, deleted, item) VALUES (?, ?, ?, ?, ?, ?, 0, 0, ?)",
            from, to, subject, body, sent, expires, itemName
        );
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public void Mail_UpdateMessage(MailMessage message)
    {
        string itemName = "";
        if (message.item != null)
            itemName = message.item.name;

        connection.Execute("UPDATE mail SET read=?, deleted=?, item=? WHERE id=?",
            message.read, message.deleted, itemName, message.id);
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public MailMessage Mail_MessageById(long id)
    {
        MailMessage message = new MailMessage();

        var table = connection.Query<mail>("SELECT * FROM mail WHERE id=?", id);
        if (table.Count == 1)
        {
            message = Mail_BuildMessageFromDBRow(table[0]);
        }

        return message;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
    public List<MailMessage> Mail_CheckForNewMessages(long maxID)
    {
        List<MailMessage> result = new List<MailMessage>();
        var table = connection.Query<mail>(
            "SELECT * FROM mail WHERE id > ? AND deleted=0 AND expires > ? ORDER BY sent", maxID, Epoch.Current());
        foreach (var row in table)
        {
            MailMessage message = Mail_BuildMessageFromDBRow(row);
            result.Add(message);
        }

        return result;
    }

    public long Mail_MaxId()
    {
        return connection.ExecuteScalar<long>("SELECT IFNULL(id, 0) FROM (SELECT MAX(id) AS id FROM mail)");
    }

    // -----------------------------------------------------------------------------------
}