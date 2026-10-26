namespace VotePreferentiel.Worker;

public class Worker(VoteProvider voteProvider, ILogger<Worker> logger, IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var voteModel = await voteProvider.GetVotesAsync();

            logger.LogInformation("Tours identifiés: {Count}", voteModel.Votes.Count);

            var premierTour = voteModel.Votes.FirstOrDefault();
            logger.LogInformation("Premier tour: {Choices}",
                premierTour.Choices is null ? "none" : string.Join(", ", premierTour.Choices));
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
