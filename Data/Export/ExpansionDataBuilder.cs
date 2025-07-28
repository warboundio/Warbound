using Data.BlizzardAPI;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Data.Export;

public class ExpansionDataBuilder
{
    public static void WriteExpansionDataFile()
    {
        using BlizzardAPIContext context = new();

        List<JournalExpansion> expansions = [.. context.JournalExpansions.AsNoTracking().OrderBy(e => e.Id)];

        List<string> lines = ["-1|UNKNOWN"];
        lines.AddRange(expansions.Select(e => $"{e.Id}|{e.Name}"));

        Directory.CreateDirectory(@"C:\Applications\Warbound\cached");
        File.WriteAllLines(@"C:\Applications\Warbound\cached\ExpansionData.data", lines);
    }
}
