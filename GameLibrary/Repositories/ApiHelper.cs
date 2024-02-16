using System.Text.Json;

namespace GameLibrary.Repositories
{
    public static class ApiHelper
    {
        public static async Task<Tobject> ReadContentAsync<Tobject>(this HttpResponseMessage response, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (response.IsSuccessStatusCode == false)
                throw new ApplicationException($"Something went wrong calling the API: " + response.ReasonPhrase);

            string? dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = JsonSerializer.Deserialize<Tobject>(
                dataAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (result is null)
                throw new ApplicationException($"Something went wrong calling the API");

            return result;
        }
    }
}
