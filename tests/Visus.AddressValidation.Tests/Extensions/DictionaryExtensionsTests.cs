namespace Visus.AddressValidation.Tests.Extensions;

using AddressValidation.Extensions;
using AwesomeAssertions;

internal sealed class DictionaryExtensionsTests
{
    [Test]
    public void Merge_FromDictionary_AddsNonExistingKeys()
    {
        Dictionary<string, int> source = new(StringComparer.Ordinal)
        {
            { "a", 1 },
        };
        Dictionary<string, int> other = new(StringComparer.Ordinal)
        {
            { "b", 2 },
        };

        source.Merge((IDictionary<string, int>)other);

        source.Should().HaveCount(2);
        source["b"].Should().Be(2);
    }

    [Test]
    public void Merge_FromDictionary_DoesNotOverwriteExistingKeys()
    {
        Dictionary<string, int> source = new(StringComparer.Ordinal)
        {
            { "a", 1 },
        };
        Dictionary<string, int> other = new(StringComparer.Ordinal)
        {
            { "a", 99 },
        };

        source.Merge((IDictionary<string, int>)other);

        source["a"].Should().Be(1);
    }

    [Test]
    public void Merge_FromReadOnlyDictionary_AddsNonExistingKeys()
    {
        Dictionary<string, int> source = new(StringComparer.Ordinal)
        {
            { "a", 1 },
        };
        IReadOnlyDictionary<string, int> other = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            { "b", 2 },
            { "c", 3 },
        };

        source.Merge(other);

        source.Should().HaveCount(3);
        source["b"].Should().Be(2);
        source["c"].Should().Be(3);
    }

    [Test]
    public void Merge_FromReadOnlyDictionary_DoesNotOverwriteExistingKeys()
    {
        Dictionary<string, int> source = new(StringComparer.Ordinal)
        {
            { "a", 1 },
        };
        IReadOnlyDictionary<string, int> other = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            { "a", 99 },
        };

        source.Merge(other);

        source["a"].Should().Be(1);
    }

    [Test]
    public void Merge_NullDictionary_ThrowsArgumentNullException()
    {
        Dictionary<string, int> source = [];
        IDictionary<string, int>? other = null;

        Action act = () => source.Merge(other!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Merge_NullReadOnlyDictionary_ThrowsArgumentNullException()
    {
        Dictionary<string, int> source = [];
        IReadOnlyDictionary<string, int>? other = null;

        Action act = () => source.Merge(other!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Merge_NullSource_ThrowsArgumentNullException()
    {
        IDictionary<string, int>? source = null;
        IReadOnlyDictionary<string, int> other = new Dictionary<string, int>(StringComparer.Ordinal);

        Action act = () => source!.Merge(other);

        act.Should().Throw<ArgumentNullException>();
    }
}
