using System;
using R2API;

namespace BloodMageMod.Modules
{
    internal static class Tokens
    {
        internal static string prefix = BloodMagePlugin.developerPrefix + "_BLOODMAGE_";
        internal static void AddTokens()
        {
            string desc = "The Blood Mage is a ranged \"glass cannon\" who specializes in sacrificing HP to deal massive damage.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Your passive gives you increased damage the lower your health is. Make sure to balance your passive's bonus with your survivability!" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Petrified Blood is a quick, easy way to get a massive boost from your passive and gain some defense, but it's not a panic button." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Doom Desire stores damage from all sources, including from <style=cIsUtility>Sanguine</style> skills." + Environment.NewLine + Environment.NewLine;

            LanguageAPI.Add(prefix + "NAME", "Blood Mage");
            LanguageAPI.Add(prefix + "SUB", "Cultist of Sacrifice");

            LanguageAPI.Add(prefix + "DESC", desc);

            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Pain Attunement");
            LanguageAPI.Add(prefix + "PASSIVE_DESC", "You are stronger the lower your health is, dealing up to <style=cIsDamage>50% more damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_BLOODBOLT_NAME", "Blood Bolt");
            LanguageAPI.Add(prefix + "PRIMARY_BLOODBOLT_DESC", Helpers.sanguinePrefix + "<style=cIsHealth>15 HP/s.</style> <style=cIsUtility>Channel</style> this ability,"
                + " draining health over time. Then, fire a blood bolt for <style=cIsDamage>200% damage</style>.");
            
            LanguageAPI.Add(prefix + "SECONDARY_ESSENCESAP_NAME", "Essence Sap");
            LanguageAPI.Add(prefix + "SECONDARY_ESSENCESAP_DESC", Helpers.agilePrefix + "<style=cIsUtility>Hold</style> to drain a target's life force, <style=cIsDamage>dealing damage equal to 10% of your HP per second</style> and <style=cIsHealing>healing you for that amount</style>.");

            LanguageAPI.Add(prefix + "UTILITY_PETRIBLOOD_NAME", "Petrified Blood");
            LanguageAPI.Add(prefix + "UTILITY_PETRIBLOOD_DESC", "<style=cIsHealth>Sets your HP to 50%, and prevents you from recovering above 50% HP. </style>"
                + "<style=cIsDamage>40% of damage from hits</style> is taken instantly, the other <style=cIsDamage>60%</style> is taken over <style=cIsUtility>4 seconds</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_DOOMDESIRE_NAME", "Doom Desire");
            LanguageAPI.Add(prefix + "SPECIAL_DOOMDESIRE_DESC", "Place a debuff on both you and the target. The debuff on you causes you to take "
                + "<style=cIsDamage>10% increased damage</style>. The debuff on the target stores damage you take. "
                + "When the debuff expires, it deals <style=cIsDamage>3000% of the stored damage in an explosion</style>." + Environment.NewLine
                + "<style=cIsUtility>Recast</style> to <style=cIsUtility>manually detonate the debuff</style>.");

            LanguageAPI.Add("KEYWORD_SANGUINE", "<style=cKeywordName>Sanguine</style><style=cSub>Health spent by this skill is added to its base damage.</style>");
        }
    }
}
