//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Threading.Tasks;
//using Core.ETL;
//using Data.BlizzardAPI.Models;

//namespace Data.ETLs;

//[Table("item_expansion", Schema = "wow")]
//public sealed class ItemExpansion
//{
//    [Key]
//    [DatabaseGenerated(DatabaseGeneratedOption.None)]
//    public int ItemId { get; set; }
//    public int ExpansionId { get; set; }
//}


//public class ItemExpansionETL : RunnableBlizzardETL
//{
//    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ItemExpansionETL>(job);

//    protected override Task<List<object>> GetItemsToProcessAsync()
//    {
//        WarcraftData warcraftData = WarcraftData.Instance;

//        Dictionary<int, ItemExpansion>.KeyCollection existingMappings = warcraftData.ItemExpansions.Keys;
//        Dictionary<int, Item>.KeyCollection allItems = warcraftData.Items.Keys;
//        List<object> itemsToProcess = [.. allItems.Except(existingMappings).Cast<object>()];

//        return Task.FromResult(itemsToProcess);
//    }

//    protected override Task UpdateItemAsync(object item)
//    {
//        int itemId = (int)item;
//        int expansionId = DetermineExpansionForItem(itemId);

//        ItemExpansion itemExpansion = new()
//        {
//            ItemId = itemId,
//            ExpansionId = expansionId,
//        };

//        SaveBuffer.Add(itemExpansion);
//        return Task.CompletedTask;
//    }

//    private int DetermineExpansionForItem(int itemId)
//    {
//        WarcraftData warcraftData = WarcraftData.Instance;

//        List<JournalEncounter> encounters = [.. warcraftData.JournalEncounters.Values.Where(x => !string.IsNullOrEmpty(x.Items))];

//        List<JournalEncounter> matchingEncounters = [.. encounters.Where(encounter =>
//        {
//            IEnumerable<int> itemIds = encounter.Items.Split(';', StringSplitOptions.RemoveEmptyEntries)
//                .Select(x => int.TryParse(x.Trim(), out int id) ? id : -1)
//                .Where(x => x > 0);
//            return itemIds.Contains(itemId);
//        })];

//        if (!matchingEncounters.Any())
//        {
//            return -1;
//        }

//        foreach (JournalEncounter? encounter in matchingEncounters)
//        {
//            if (encounter.InstanceId <= 0)
//            {
//                continue;
//            }

//            List<JournalExpansion> expansions = [.. warcraftData.JournalExpansions.Values.Where(x => !string.IsNullOrEmpty(x.DungeonIds) || !string.IsNullOrEmpty(x.RaidIds))];

//            JournalExpansion? matchingExpansion = expansions.FirstOrDefault(expansion =>
//            {
//                IEnumerable<int> dungeonIds = expansion.DungeonIds.Split(';', StringSplitOptions.RemoveEmptyEntries)
//                    .Select(x => int.TryParse(x.Trim(), out int id) ? id : -1)
//                    .Where(x => x > 0);
//                IEnumerable<int> raidIds = expansion.RaidIds.Split(';', StringSplitOptions.RemoveEmptyEntries)
//                    .Select(x => int.TryParse(x.Trim(), out int id) ? id : -1)
//                    .Where(x => x > 0);

//                return dungeonIds.Contains(encounter.InstanceId) || raidIds.Contains(encounter.InstanceId);
//            });

//            if (matchingExpansion != null)
//            {
//                return matchingExpansion.Id;
//            }
//        }

//        return -1;
//    }
//}
