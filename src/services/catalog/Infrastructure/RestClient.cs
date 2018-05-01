using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace CiK.Catalog
{
  public static class HttpRequestExtensions
  {
    public static IEnumerable<KeyValuePair<string, string>> GetOpenTracingInfo(this HttpRequest request)
    {
      return request.Headers.Where(x =>
        x.Key == "x-request-id" ||
        x.Key == "x-b3-traceid" ||
        x.Key == "x-b3-spanid" ||
        x.Key == "x-b3-parentspanid" ||
        x.Key == "x-b3-sampled" ||
        x.Key == "x-b3-flags" ||
        x.Key == "x-ot-span-context"
      ).Select(y =>
        new KeyValuePair<string, string>(
          y.Key,
          y.Value.FirstOrDefault()));
    }
  }

  public class RestClient
  {
    private readonly HttpClient _client;
    private IEnumerable<KeyValuePair<string, string>> _openTracingInfo;
    public RestClient(HttpClient client = null)
    {
      _client = client ?? new HttpClient();
    }

    public void SetOpenTracingInfo(IEnumerable<KeyValuePair<string, string>> info)
    {
      _openTracingInfo = info;
    }

    public async Task<TReturnMessage> GetAsync<TReturnMessage>(string serviceName, int port, string path)
            where TReturnMessage : class, new()
    {
      var enrichClient = EnrichHeaderInfo(_client, _openTracingInfo);

      var response = await enrichClient.GetAsync(GetUri(serviceName, port, path));
      response.EnsureSuccessStatusCode();

      if (!response.IsSuccessStatusCode) return await Task.FromResult(new TReturnMessage());
      var result = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<TReturnMessage>(result);
    }

    public async Task<TReturnMessage> PostAsync<TReturnMessage>(string serviceName, int port, string path, object dataObject = null)
            where TReturnMessage : class, new()
    {
      var enrichClient = EnrichHeaderInfo(_client, _openTracingInfo);
      var content = dataObject != null ? JsonConvert.SerializeObject(dataObject) : "{}";

      var response = await _client.PostAsync(GetUri(serviceName, port, path), GetStringContent(content));
      response.EnsureSuccessStatusCode();

      if (!response.IsSuccessStatusCode) return await Task.FromResult(new TReturnMessage());
      var result = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<TReturnMessage>(result);
    }

    public async Task<TReturnMessage> PutAsync<TReturnMessage>(string serviceName, int port, string path, object dataObject = null)
            where TReturnMessage : class, new()
    {
      var enrichClient = EnrichHeaderInfo(_client, _openTracingInfo);
      var content = dataObject != null ? JsonConvert.SerializeObject(dataObject) : "{}";

      var response = await _client.PutAsync(GetUri(serviceName, port, path), GetStringContent(content));
      response.EnsureSuccessStatusCode();

      if (!response.IsSuccessStatusCode) return await Task.FromResult(new TReturnMessage());
      var result = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<TReturnMessage>(result);
    }

    public async Task<bool> DeleteAsync(string serviceName, int port, string path)
    {
      var enrichClient = EnrichHeaderInfo(_client, _openTracingInfo);

      var response = await _client.DeleteAsync(GetUri(serviceName, port, path));
      response.EnsureSuccessStatusCode();

      if (!response.IsSuccessStatusCode) return await Task.FromResult(false);
      var result = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<bool>(result);
    }

    public static Uri GetUri(string serviceName, int port, string path)
    {
      return new Uri($"http://{serviceName}:{port}/{path}");
    }

    private static StringContent GetStringContent(string content)
    {
      return new StringContent(content, Encoding.UTF8, "application/json");
    }

    private static HttpClient EnrichHeaderInfo(
      HttpClient client,
      IEnumerable<KeyValuePair<string, string>> openTracingInfo)
    {
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

      foreach (var info in openTracingInfo)
      {
        client.DefaultRequestHeaders.Add(info.Key, info.Value);
      }

      return client;
    }
  }
}