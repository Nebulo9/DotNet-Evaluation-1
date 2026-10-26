namespace VotePreferentiel;

public class VotePreferentiel
{
    async public static Task Main(string[] args)
    {
        var testProvider = new VoteProvider(new HttpClient());

        var testModel = await testProvider.GetVotesAsync();
        
        Console.WriteLine(testModel.Votes.Count);
    }
}