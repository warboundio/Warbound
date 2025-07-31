namespace Data.Serialization;

public class TransmogEncoder
{
    //public readonly StringBuilder SB = new();

    //private readonly Dictionary<int, List<Quest>> _questsByRewardItemId = [];
    //private readonly Dictionary<int, List<Vendor>> _vendorsByItemId = [];
    //private readonly Dictionary<int, List<Achievement>> _achievementsByRewardItemId = [];
    //private readonly Dictionary<int, List<JournalEncounter>> _encountersById = [];
    //private readonly Dictionary<int, List<Recipe>> _recipesByCraftedItemId = [];

    //private readonly EncodingStringBuilder _transmogBuilder = new('T', typeof(ItemAppearance));
    //private readonly EncodingStringBuilder _itemBuilder = new('I', typeof(Item));
    //private readonly EncodingStringBuilder _questBuilder = new('Q', typeof(Quest));
    //private readonly EncodingStringBuilder _vendorBuilder = new('V', typeof(Vendor));
    //private readonly EncodingStringBuilder _vendorItemBuilder = new('W', typeof(VendorItem));
    //private readonly EncodingStringBuilder _achievementBuilder = new('A', typeof(Achievement));
    //private readonly EncodingStringBuilder _encounterBuilder = new('E', typeof(JournalEncounter));
    //private readonly EncodingStringBuilder _recipeBuilder = new('R', typeof(Recipe));

    //public TransmogEncoder()
    //{
    //    BuildQuestLookup();
    //    BuildVendorLookup();
    //    BuildAchievementLookup();
    //    BuildEncounterLookup();
    //    BuildRecipeLookup();
    //}

    //public void BuildAndSave()
    //{
    //    string pathToSerialize = @"C:\Applications\Warbound\serialized\appearances.data";
    //    using StreamWriter writer = new(pathToSerialize, false, Encoding.UTF8);

    //    foreach (ItemAppearance ia in WarcraftData.Instance.ItemAppearances.Values.OrderBy(o => o.Id))
    //    {
    //        AddAppearance(ia, writer);
    //    }
    //}

    //private void AddAppearance(ItemAppearance appearance, StreamWriter writer)
    //{
    //    List<int> itemIds = [.. appearance.ItemIds.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)];
    //    List<Item> itemsGrantingAppearance = [.. WarcraftData.Instance.Items.Values.Where(item => itemIds.Contains(item.Id)).OrderBy(o => o.Id)];

    //    if (itemsGrantingAppearance.Count == 0) { return; }

    //    writer.WriteLine(_transmogBuilder.GetEncodedString(appearance));

    //    foreach (Item item in itemsGrantingAppearance)
    //    {
    //        writer.WriteLine(_itemBuilder.GetEncodedString(item));

    //        List<Quest> quests = _questsByRewardItemId.TryGetValue(item.Id, out List<Quest>? value) ? value : [];
    //        foreach (Quest quest in quests) { writer.WriteLine(_questBuilder.GetEncodedString(quest)); }

    //        List<Vendor> vendors = _vendorsByItemId.TryGetValue(item.Id, out List<Vendor>? v) ? v : [];
    //        foreach (Vendor vendor in vendors) { writer.WriteLine(_vendorBuilder.GetEncodedString(vendor)); }

    //        foreach (Vendor vendor in vendors)
    //        {
    //            VendorItem vItem = WarcraftData.Instance.G_VendorItems.Values.First(v => v.VendorId == vendor.NpcId && v.ItemId == item.Id);
    //            writer.WriteLine(_vendorItemBuilder.GetEncodedString(vItem));
    //        }

    //        List<Achievement> achievements = _achievementsByRewardItemId.TryGetValue(item.Id, out List<Achievement>? a) ? a : [];
    //        foreach (Achievement achievement in achievements) { writer.WriteLine(_achievementBuilder.GetEncodedString(achievement)); }

    //        List<AuctionRecord> auctions = WarcraftData.Instance.G_Auctions.TryGetValue(item.Id, out List<AuctionRecord>? auctionList) ? auctionList : [];
    //        if (auctions.Count > 0)
    //        {
    //            int last14Points = (int)auctions.OrderByDescending(a => a.CreatedOn).Take(14).Average(a => a.Price / 100);
    //            writer.WriteLine($"A|{Base90.Encode(last14Points, 5)}");
    //        }

    //        //ItemMedia? itemMedia = WarcraftData.Instance.ItemMedias.TryGetValue(item.Id, out ItemMedia? media) ? media : null;
    //        //if (itemMedia != null) { writer.WriteLine($"M|{itemMedia.URL}"); }

    //        List<JournalEncounter> encounters = _encountersById.TryGetValue(item.Id, out List<JournalEncounter>? encounterList) ? encounterList : [];
    //        foreach (JournalEncounter encounter in encounters) { writer.WriteLine($"E|{_encounterBuilder.GetEncodedString(encounter)}"); }

    //        List<Recipe> recipes = _recipesByCraftedItemId.TryGetValue(item.Id, out List<Recipe>? recipeList) ? recipeList : [];
    //        foreach (Recipe recipe in recipes) { writer.WriteLine(_recipeBuilder.GetEncodedString(recipe)); }
    //    }
    //}

    //private void BuildQuestLookup()
    //{
    //    foreach (Quest quest in WarcraftData.Instance.Quests.Values.Where(o => o.RewardItems.Length > 0))
    //    {
    //        IEnumerable<int> rewards = quest.RewardItems.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
    //        foreach (int itemId in rewards)
    //        {
    //            if (!_questsByRewardItemId.TryGetValue(itemId, out List<Quest>? value)) { value = []; _questsByRewardItemId[itemId] = value; }
    //            value.Add(quest);
    //        }
    //    }
    //}

    //private void BuildVendorLookup()
    //{
    //    foreach (Vendor vendor in WarcraftData.Instance.G_Vendors.Values)
    //    {
    //        IEnumerable<VendorItem> vendorItems = WarcraftData.Instance.G_VendorItems.Values.Where(v => v.VendorId == vendor.NpcId);

    //        foreach (VendorItem vendorItem in vendorItems)
    //        {
    //            int itemId = vendorItem.ItemId;
    //            if (!_vendorsByItemId.TryGetValue(itemId, out List<Vendor>? value))
    //            {
    //                value = [];
    //                _vendorsByItemId[itemId] = value;
    //            }

    //            value.Add(vendor);
    //        }
    //    }
    //}

    //private void BuildAchievementLookup()
    //{
    //    foreach (Achievement achievement in WarcraftData.Instance.Achievements.Values)
    //    {
    //        if (achievement.RewardItemId > 0)
    //        {
    //            if (!_achievementsByRewardItemId.TryGetValue((int)achievement.RewardItemId, out List<Achievement>? value))
    //            {
    //                value = [];
    //                _achievementsByRewardItemId[(int)achievement.RewardItemId] = value;
    //            }

    //            value.Add(achievement);
    //        }
    //    }
    //}

    //private void BuildEncounterLookup()
    //{
    //    foreach (JournalEncounter encounter in WarcraftData.Instance.JournalEncounters.Values)
    //    {
    //        if (!_encountersById.TryGetValue(encounter.Id, out List<JournalEncounter>? value))
    //        {
    //            value = [];
    //            _encountersById[encounter.Id] = value;
    //        }
    //        value.Add(encounter);
    //    }
    //}

    //private void BuildRecipeLookup()
    //{
    //    foreach (Recipe recipe in WarcraftData.Instance.Recipes.Values)
    //    {
    //        if (recipe.CraftedItemId > 0)
    //        {
    //            if (!_recipesByCraftedItemId.TryGetValue(recipe.CraftedItemId, out List<Recipe>? value))
    //            {
    //                value = [];
    //                _recipesByCraftedItemId[recipe.CraftedItemId] = value;
    //            }
    //            value.Add(recipe);
    //        }
    //    }
    //}
}
