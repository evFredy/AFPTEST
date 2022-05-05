// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;

var Apis = new List<ApiData>();
Apis.Add(new ApiData("San Salvador"));
Apis.Add(new ApiData("Ahuachapan"));
Apis.Add(new ApiData("Cojutepeque"));

foreach (var item in Apis)
{
    var LatitudLongitud = await item.LatitudLongitud();
    foreach (var Process in LatitudLongitud)
    {
        var lat = Process.FirstOrDefault(p => p.Key == "lat");
        var lon = Process.FirstOrDefault(p => p.Key == "lon");
        var ResultWeather = await item.Weather(lat.Value.ToString(), lon.Value.ToString());
        var Result = ResultWeather.Where(p => p.Key == "wind" || p.Key == "main" || p.Key == "timezone");
        await File.AppendAllTextAsync("Log.txt", "\n" + DateTime.Now.ToString() + " " + item.CityName + "\n");
        await File.AppendAllTextAsync("Log.txt", JsonConvert.SerializeObject(Result, Formatting.Indented));
    }
}

public class ApiData
{
    public ApiData(string cityName, string apiKey = "b003b168006b7a5e8e9e4781c4d37336", string countryCode = "503", string limit = "3")
    {
        ApiKey=apiKey;
        CityName=cityName;
        CountryCode=countryCode;
        Limit=limit;
    }

    public string ApiKey { get; set; }

    public string CityName { get; set; }

    public string CountryCode { get; set; }

    public string Limit { get; set; }

    public async Task<IList<IDictionary<string, object>>> LatitudLongitud()
    {
        var webClient = new HttpClient();
        var Uri = $"http://api.openweathermap.org/geo/1.0/direct?q={CityName},{CountryCode}&limit={Limit}&appid={ApiKey}";
        var Response = await webClient.GetAsync(Uri).ConfigureAwait(false);
        var jsonString = await Response.Content.ReadAsStringAsync();
        var Result = JsonConvert.DeserializeObject<IList<IDictionary<string, object>>>(jsonString);
        return Result ?? new List<IDictionary<string, object>>();
    }

    public async Task<IDictionary<string, object>> Weather(string Latitud, string Longitud)
    {
        var webClient = new HttpClient();
        var Uri = $"https://api.openweathermap.org/data/2.5/weather?lat={Latitud}&lon={Longitud}&appid={ApiKey}";
        var Response = await webClient.GetAsync(Uri).ConfigureAwait(false);
        var jsonString = await Response.Content.ReadAsStringAsync();
        var Result = JsonConvert.DeserializeObject<IDictionary<string, object>>(jsonString);
        return Result ?? new Dictionary<string, object>();
    }
}