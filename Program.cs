using System.Text.Json;
using System.Text.Json.Serialization;

var client = new HttpClient();
var ApiUrl= "http://localhost:5004/api/machine";
while (true)
{
    var payload = new
    {
        machineId = "Machine1",
        temperature = new Random().Next(60, 90),
        timestamp = DateTime.UtcNow
    };
    var json= JsonSerializer.Serialize(payload);
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

    try
    {
        var response = await client.PostAsync(ApiUrl, content);

        Console.WriteLine($"Sent → Temp: {payload.temperature}°C | Status: {response.StatusCode}");

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Data sent successfully at {DateTime.UtcNow}");
        }
        else
        {
            Console.WriteLine($"Failed to send data: {response.StatusCode}");
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending data: {ex.Message}");
    }

    await Task.Delay(2000); // Wait for 2 seconds before sending the next reading
}