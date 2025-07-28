using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.BlizzardAPI;
using Data.Support;
using Microsoft.EntityFrameworkCore;

namespace Data.Export;

public class CollectionOverallDataBuilder
{
    public static void WriteCollectionsOverallFile()
    {
        using BlizzardAPIContext _context = new();

        List<int> petIds = [.. _context.Pets.AsNoTracking().Select(x => x.Id)];
        List<int> toyIds = [.. _context.Toys.AsNoTracking().Select(x => x.Id)];
        List<int> mountIds = [.. _context.Mounts.AsNoTracking().Select(x => x.Id)];
        List<int> appearanceIds = [.. _context.ItemAppearances.AsNoTracking().Select(x => x.Id)];
        List<int> recipeIds = [.. _context.Recipes.AsNoTracking().Select(x => x.Id)];

        List<string> pets = Base90.Encode(petIds);
        List<string> toys = Base90.Encode(toyIds);
        List<string> mounts = Base90.Encode(mountIds);
        List<string> appearances = Base90.Encode(appearanceIds);
        List<string> recipes = Base90.Encode(recipeIds);

        string petsLine = "P|" + string.Join(';', pets);
        string toysLine = "T|" + string.Join(';', toys);
        string mountsLine = "M|" + string.Join(';', mounts);
        string appearancesLine = "A|" + string.Join(';', appearances);
        string recipesLine = "R|" + string.Join(';', recipes);

        string output = $"{petsLine}\n{toysLine}\n{mountsLine}\n{appearancesLine}\n{recipesLine}";

        Directory.CreateDirectory(@"C:\Applications\Warbound\cached");
        File.WriteAllText(@"C:\Applications\Warbound\cached\CollectionsOverall.data", output);
    }
}
