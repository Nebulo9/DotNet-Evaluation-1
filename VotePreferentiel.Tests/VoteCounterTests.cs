namespace VotePreferentiel.Tests;

public class VoteCounterTests
{
    private readonly VoteCounter _counter = new();

    // ---------- Helpers ----------

    private static VoteModel CreateVotes(IEnumerable<string[]> ballots) => new()
    {
        Votes = ballots.Select(choices => new Vote { Choices = choices.ToList() }).ToList()
    };

    private static IEnumerable<string[]> Ballots(int count, params string[] choices) =>
        Enumerable.Repeat(choices, count);

    // ---------- Tests ----------

    [Test]
    public async Task GetWinner_NominalCase_ReturnsChloe()
    {
        var voteModel = CreateVotes(
        [
            ["Alice", "Bilal", "Chloé"],
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Bilal", "Alice"],
            ["Chloé", "Alice", "Bilal"]
        ]);

        var winner = _counter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_SingleCandidate_ReturnsAlice()
    {
        var voteModel = CreateVotes([["Alice"]]);

        var winner = _counter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Alice");
    }

    [Test]
    public async Task GetWinner_PreferenceTransfers_ReturnsChloe()
    {
        var voteModel = CreateVotes(
        [
            .. Ballots(4, "Alice", "Bilal", "Chloé", "Diane"),
            .. Ballots(3, "Bilal", "Chloé", "Alice", "Diane"),
            .. Ballots(3, "Chloé", "Alice", "Bilal", "Diane"),
            .. Ballots(1, "Diane", "Chloé", "Bilal", "Alice")
        ]);

        var winner = _counter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_TieForLastPlace_EliminatesBothAndReturnsChloe()
    {
        var voteModel = CreateVotes(
        [
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Alice", "Bilal"],
            ["Chloé", "Bilal", "Alice"]
        ]);

        var winner = _counter.GetWinner(voteModel);

        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_EmptyBallot_ThrowsException()
    {
        var voteModel = CreateVotes([Array.Empty<string>()]);

        await Assert.That(() => _counter.GetWinner(voteModel)).Throws<InvalidOperationException>();
    }
}