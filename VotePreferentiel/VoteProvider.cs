using System.Text.Json;

namespace VotePreferentiel;

public class VoteProvider(HttpClient httpClient)
{
    private const string Url = "https://raw.githubusercontent.com/JLou/dotnet-course/refs/heads/main/eval-1/votes.example.json";

    public async Task<VoteModel> GetVotesAsync()
    {
        var response = await httpClient.GetAsync(Url);
        
        var voteModel = await  JsonSerializer.DeserializeAsync<VoteModel>(await response.Content.ReadAsStreamAsync());

        if (voteModel is null)
        {
            throw new NullReferenceException("Could not get votes");
        }
        
        return voteModel;
    }
}