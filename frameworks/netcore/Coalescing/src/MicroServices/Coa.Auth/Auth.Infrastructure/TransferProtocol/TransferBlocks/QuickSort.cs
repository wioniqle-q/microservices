using Auth.Domain.Entities.MongoEntities;
using static System.String;

namespace Auth.Infrastructure.TransferProtocol.TransferBlocks;

public static class HorsPool
{
    public static async Task<bool> HorspoolSearch(string[] texts, string key)
    {
        var keyLength = key.Length;
        var shifts = CalculateShiftTable(key);

        foreach (var text in texts)
        {
            var textLength = text.Length;
            var i = keyLength - 1;
            var j = keyLength - 1;

            while (i < textLength)
            {
                var k = i;
      
                while (k >= 0 && text[k] == key[j])
                {
                    k--;
                    j--;  
                }

                if (j == -1)
                {
                    return await Task.FromResult(true);
                }

                i += shifts[text[i]];
                j = keyLength - 1;
            }
        }

        return await Task.FromResult(false);
    }

    private static int[] CalculateShiftTable(string key)
    {
        var shifts = new int[256];
  
        for(var i = 0; i < 256; i++) {
            shifts[i] = key.Length; 
        }
  
        for(var i = 0; i < key.Length - 1; i++) {
            var character = key[i];
            shifts[character] = key.Length - i - 1;
        }

        return shifts;
    }
}

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