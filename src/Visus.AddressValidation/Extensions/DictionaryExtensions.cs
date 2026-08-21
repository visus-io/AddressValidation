namespace Visus.AddressValidation.Extensions;

/// <summary>
///     Contains extension methods for <see cref="IDictionary{TKey,TValue}" /> and
///     <see cref="IReadOnlyDictionary{TKey,TValue}" />
///     instances.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    ///     Merges the elements from <paramref name="dictionary" /> with the <paramref name="source" />.
    /// </summary>
    /// <remarks>If the key already exists in <paramref name="source" />, the merge skips that element.</remarks>
    /// <param name="source">The dictionary to merge with.</param>
    /// <param name="dictionary">The <see cref="IReadOnlyDictionary{TKey,TValue}" /> to merge from.</param>
    /// <typeparam name="TKey">The element type of the key.</typeparam>
    /// <typeparam name="TValue">The element type of the value.</typeparam>
    public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> source, IReadOnlyDictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(dictionary);

        foreach ( KeyValuePair<TKey, TValue> kvp in dictionary )
        {
            if ( !source.ContainsKey(kvp.Key) )
            {
                source.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
