namespace NotebookLua.NativeLibs;

// Note: this was mostly made with AI tools

using System;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public static class Net
{
    public static bool PerformGetRequest(string url, out string content)
    {
        var httpClient = new HttpClient();
        var task = httpClient.GetAsync(url);
        task.Wait();
        var response = task.Result;

        if (response.IsSuccessStatusCode)
        {
            var readTask = response.Content.ReadAsStringAsync();
            readTask.Wait();
            content = readTask.Result;
            return true;
        }

        content = "Error: " + response.StatusCode;
        return false;
    }

    public static string PerformPostRequest(string url, string jsonData)
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                // Prepare the content data
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send a POST request to the specified URL with the JSON content
                var response = httpClient.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    // If successful, read and return the response content
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    // If the response is unsuccessful, return a formatted error message
                    return "Error: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                // In case of an exception, return the exception message
                return "Exception: " + ex.Message;
            }
        }
    }
    public static string AI(string prompt, string? system_message=null)
    {
        using var httpClient = new HttpClient();
        try
        {
            var messages = new[]
            {
                new { role = "user", content = prompt },
            };
            if (system_message is not null)
            {
                messages = messages.Append(
                    new { role = "system", content = system_message }
                ).ToArray();
            }
            // Prepare the content data
            // var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                messages = messages,
                model = "gpt-4",
                temperature = 0.7,
            }), System.Text.Encoding.UTF8, "application/json");
            Console.WriteLine("Content: " + JsonSerializer.Serialize(messages));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Config.openAiApiKey}");
            // Send a POST request to the specified URL with the JSON content
            var response = httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content).Result;

            if (response.IsSuccessStatusCode)
            {
                // If successful, read and return the response content
                var responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic? responseObject = JsonSerializer.Deserialize<dynamic>(responseContent);
                if (responseObject is null) {
                    return "Error: couldn't deserialize response";
                }
                return responseObject.GetProperty("choices")[0].GetProperty("message").GetProperty("content").ToString();
            }
            else
            {
                // If the response is unsuccessful, return a formatted error message
                return "Error: " + response.StatusCode;
            }
        }
        catch (Exception ex)
        {
            // In case of an exception, return the exception message
            return "Exception: " + ex.Message;
        }
    }
}
