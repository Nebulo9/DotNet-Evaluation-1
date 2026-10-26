namespace VotePreferentiel.Worker;

public class Worker(VoteProvider voteProvider, IVoteCounter voteCounter, ILogger<Worker> logger, IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var voteModel = await voteProvider.GetVotesAsync();

            logger.LogInformation("Tours identifiés: {Count}", voteModel.Votes.Count);

            var winner = voteCounter.GetWinner(voteModel);      // CHANGED: replaces the "first ballot" log
            Console.WriteLine($"Winner: {winner}"); 
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not retrieve votes");
        }
        finally
        {
            lifetime.StopApplication();
        }
    }
}
