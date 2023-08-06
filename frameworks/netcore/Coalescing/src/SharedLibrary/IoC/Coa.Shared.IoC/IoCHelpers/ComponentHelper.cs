namespace Coa.Shared.IoC.IoCHelpers;

public static class ComponentHelper
{
    public static List<T> SortByDependencies<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
    {
        var sorted = new List<T>();
        var visited = new HashSet<T>();

        foreach (var item in source)
            Visit(item, visited, sorted, getDependencies);

        return sorted;
    }

    private static void Visit<T>(T item, ISet<T> visited, ICollection<T> sorted,
        Func<T, IEnumerable<T>>? getDependencies)
    {
        if (visited.Contains(item))
            return;

        visited.Add(item);

        var dependencies = getDependencies?.Invoke(item);
        if (dependencies is not null)
            foreach (var dependency in dependencies.Where(dep => dep != null))
                Visit(dependency, visited, sorted, getDependencies);

        sorted.Add(item);
    }
}