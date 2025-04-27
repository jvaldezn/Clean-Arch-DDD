using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Infrastructure.Http;

public class HttpRepository<T> : IHttpRepository<T>
{
    private readonly HttpClient _httpClient;

    public HttpRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<T>> GetAllAsync(string? url = null)
    {
        var response = await _httpClient.GetAsync(url ?? "");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<T>>() ?? Enumerable.Empty<T>();
    }

    public async Task<T?> GetByIdAsync(string id, string? url = null)
    {
        var response = await _httpClient.GetAsync(url ?? id);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> CreateAsync(T entity, string? url = null)
    {
        var response = await _httpClient.PostAsJsonAsync(url ?? "", entity);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<bool> UpdateAsync(string id, T entity, string? url = null)
    {
        var response = await _httpClient.PutAsJsonAsync(url ?? id, entity);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string id, string? url = null)
    {
        var response = await _httpClient.DeleteAsync(url ?? id);
        return response.IsSuccessStatusCode;
    }

    /*
     builder.Services.AddHttpClient<IHttpRepository<Example>, HttpRepository<Example>>(client =>
    {
        client.BaseAddress = new Uri("https://api.midominio.com/api/example/");
    });

    var data = await repository.GetAllAsync(); // Llama a https://api.midominio.com/api/example/

    var data = await repository.GetByIdAsync("1");

    o

    var repository = new HttpRepository<Example>(httpClient);

    var url = https://api.midominio.com/api/example/

    // Obtener todos
    var data = await repository.GetAllAsync(url);

    // Obtener por ID
    var data = await repository.GetByIdAsync(url + "1");
     
     */
}