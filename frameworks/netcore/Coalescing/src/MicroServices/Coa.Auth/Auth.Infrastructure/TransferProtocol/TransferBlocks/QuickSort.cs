using Auth.Domain.Entities.MongoEntities;
using static System.String;

namespace Auth.Infrastructure.TransferProtocol.TransferBlocks;

public static class QuickSort
{
    public static async Task<bool> CheckReuseToken(BaseUserEntitiy baseUserEntitiy, string token,
        CancellationToken cancellationToken = default)
    {
        var tokens = baseUserEntitiy.UserProperty.RefreshTokens.ToArray(); // 0, 1, 2

        Sort(tokens, 0, tokens.Length - 1); // Sort(tokens, 0, 2)
        var isTokenPresent = BinarySearch(tokens, token); // BinarySearch(tokens, token)

        return await Task.FromResult(isTokenPresent); // true
    }

    private static void Sort(IList<string> tokens, int low, int high)
    {
        while (true)
        {
            if (low >= high) return; // 0 >= 2
            var partitionIndex = Partition(tokens, low, high); // 2

            Sort(tokens, low, partitionIndex - 1); // Sort(tokens, 0, 1)
            low = partitionIndex + 1;
        }
    }

    private static int Partition(IList<string> tokens, int low, int high)
    {
        if (tokens.Count == 0) // 0
            return -1; // -1

        var pivot = tokens[high]; // 2
        var i = low - 1; // -1

        for (var j = low; j < high; j++) // 0 < 2
        {
            if (Compare(tokens[j], pivot, StringComparison.Ordinal) >= 0) continue; // tokens[0] >= tokens[2]
            i++; // 0
            Swap(tokens, i, j); // (tokens[0], tokens[1]) = (tokens[1], tokens[0])
        }

        Swap(tokens, i + 1, high); // (tokens[1], tokens[2]) = (tokens[2], tokens[1])
        return i + 1; // 2
    }

    private static void Swap(IList<string> tokens, int i, int j)
    {
        (tokens[i], tokens[j]) = (tokens[j], tokens[i]); // (tokens[0], tokens[1]) = (tokens[1], tokens[0])
    }

    private static bool BinarySearch(IReadOnlyList<string> tokens, string token)
    {
        // Check for empty array
        var low = 0; // 0
        var high = tokens.Count - 1; // 2

        while (low <= high) // 0 <= 2
        {
            // Get the middle index
            var mid = low + (high - low) / 2; // 1

            if (tokens[mid] == token) // tokens[1] == token
                return true; // true

            if (Compare(tokens[mid], token, StringComparison.Ordinal) < 0) // tokens[1] < token
                low = mid + 1; // 2
            else // tokens[1] > token
                high = mid - 1; // 0
        }

        return false; // false
    }
}