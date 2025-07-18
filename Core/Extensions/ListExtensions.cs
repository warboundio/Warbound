namespace Core.Extensions;

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list, int seed = 35)
    {
        Random rng = new(seed);
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
