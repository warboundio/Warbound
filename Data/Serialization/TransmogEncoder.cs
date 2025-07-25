using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.BlizzardAPI.Endpoints;
using Data.Support;

namespace Data.Serialization;

public class TransmogEncoder
{
    public readonly StringBuilder SB = new();

    private readonly Dictionary<int, List<Quest>> _questsByRewardItemId = [];

    public TransmogEncoder()
    {
        BuildQuestLookup();
    }

    public void AddAppearance(ItemAppearance appearance)
    {
        List<int> itemIds = [.. appearance.ItemIds.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)];
        List<Item> itemsGrantingAppearance = [.. WarcraftData.Instance.Items.Values.Where(item => itemIds.Contains(item.Id)).OrderBy(o => o.Id)];

        if (itemsGrantingAppearance.Count == 0) { return; }

        WriteAppearanceData(appearance, itemsGrantingAppearance[0].Name);

        foreach (Item item in itemsGrantingAppearance)
        {
            WriteItemData(item);
            WriteQuestSourceData(item);

            // probably wanna do sources here for each item
            // collection types are 'Q' quest, 'V' vendor, 'N' npc, 'A' achievement, 'R' recipe, 'W' world drop

        }
    }

    private void WriteAppearanceData(ItemAppearance appearance, string name)
    {
        SB.Append($"A|{appearance}");
        SB.Append($"|{name}|");
        SB.Append(Base90.Encode((int)appearance.SlotType, 1)); // SlotType (1)
        SB.Append(Base90.Encode(SwapValueIfValue((int)appearance.ClassType, 50), 1)); // ClassType (1)
        SB.Append(Base90.Encode(SwapValueIfValue((int)appearance.SubclassType, 8000), 2)); // SubclassType (2)
        SB.AppendLine();
    }

    private void WriteItemData(Item item)
    {
        SB.Append($"I|{item.Id}");
        SB.Append($"|{item.Name}|");
        SB.Append(Base90.Encode(SwapValueIfValue((int)item.QualityType, 80), 1)); // QualityType (1)
        SB.Append(Base90.Encode(SwapValueIfValue((int)item.InventoryType, 80), 1)); // InventoryType (1)
        SB.AppendLine();
    }

    private void WriteQuestSourceData(Item item)
    {
        if (!_questsByRewardItemId.TryGetValue(item.Id, out List<Quest>? quests) || quests.Count == 0) { return; }

        //TODO: write each quest source
    }

    private int SwapValueIfValue(int actualValue, int swapValue) => actualValue == swapValue ? 0 : actualValue;

    private void BuildQuestLookup()
    {
        foreach (Quest quest in WarcraftData.Instance.Quests.Values.Where(o => o.RewardItems.Length > 0))
        {
            IEnumerable<int> rewards = quest.RewardItems.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            foreach (int itemId in rewards)
            {
                if (!_questsByRewardItemId.TryGetValue(itemId, out List<Quest>? value)) { value = []; _questsByRewardItemId[itemId] = value; }
                value.Add(quest);
            }
        }
    }
}
