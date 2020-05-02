# Fhiz_uMMORPG_AddOns_2d
Repository for updated ummorpg 2d plugins using the fhiz code bases.
All the addons that you will find in this repository have already been tested using SQLite, as I do not have MySql currently in my game I cannot assure that they will work perfectly in that database engine, if there is any problem do not forget to create an issue .

Note: When adding some addons you will have problems with the fact that there are variables that are already declared with the same name, that happens because some addons completely modify the basic mechanics of ummorpg2d, such as the attributes, what I recommend is simply commenting on the line where it appears the duplication problem but in the original class for example:

If there is a conflict in the gold variable of the Player.cs class because it is also being declared from the UCE_Attributes.player.cs what you should do would be to comment on the line in Player.cs and leave the UCE_Attributes.player.cs intact.
