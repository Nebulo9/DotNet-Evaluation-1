namespace VotePreferentiel;

public class VoteCounter : IVoteCounter
{
    public HashSet<string> GetCandidates(VoteModel voteModel)
    {
        return voteModel.Votes
            .SelectMany(vote => vote.Choices)
            .ToHashSet();
    }

    public string GetWinner(VoteModel voteModel)
    {
        if (voteModel.Votes.Count == 0 || voteModel.Votes.Any(vote => vote.Choices.Count == 0))
        {
            throw new InvalidOperationException("The votes contain no ballot or an empty ballot.");
        }

        var remaining = GetCandidates(voteModel);

        while (remaining.Count > 1)
        {
            var activeVotes = new VoteModel
            {
                Votes = voteModel.Votes
                    .Select(vote => new Vote { Choices = vote.Choices.Where(remaining.Contains).ToList() })
                    .Where(vote => vote.Choices.Count > 0)
                    .ToList()
            };

            var counts = CountFirstChoices(GetAllFirstChoices(activeVotes));

            foreach (var candidate in remaining)
            {
                if (!counts.ContainsKey(candidate))
                {
                    counts[candidate] = 0;
                }
            }

            var toEliminate = GetCandidatesWithLeastVotes(counts);

            if (toEliminate.Length == remaining.Count)
            {
                throw new InvalidOperationException("All remaining candidates are tied: no winner can be determined.");
            }

            remaining.ExceptWith(toEliminate);
        }

        return remaining.First();
    }

    public List<string> GetAllFirstChoices(VoteModel voteModel)
    {
        return voteModel.Votes
            .Select(vote => vote.Choices.First())
            .ToList();
    }

    public IDictionary<string, int> CountFirstChoices(List<string> firstChoices)
    {
        return firstChoices
            .GroupBy(choice => choice)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    public string[] GetCandidatesWithLeastVotes(IDictionary<string, int> firstChoiceCounts)
    {
        if (firstChoiceCounts.Count == 0)
        {
            return [];
        }

        var minimum = firstChoiceCounts.Values.Min();

        return firstChoiceCounts
            .Where(pair => pair.Value == minimum)
            .Select(pair => pair.Key)
            .ToArray();
    }
}