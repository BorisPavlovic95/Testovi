namespace C_Test.Services
{
    public class ApiService
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetEmployeeData(string apiKey)
        {
            string apiUrl = $"https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code={apiKey}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode(); 

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }
        
    }
}
