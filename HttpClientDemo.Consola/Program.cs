using Comun;
using System.Text;
using System.Text.Json;

var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
using (var httpclient = new HttpClient())
{
    httpclient.BaseAddress = new Uri("https://localhost:7192");
    var respuesta = await httpclient.GetAsync("/api/Personas");    
    if (respuesta.IsSuccessStatusCode)
    {
        var resp = await respuesta.Content.ReadAsStringAsync();
        var personas = JsonSerializer.Deserialize<List<Persona>>(resp, jsonSerializerOptions);
    }
    else    
        Console.WriteLine("Ocurrió un error");    
    
}

//POST

using(var client = new HttpClient())
{
    //client.BaseAddress = new Uri("https://localhost:7192");
    //var p = new Persona() { Nombre = "Tito Agus" };
    //var resp = await client.PostAsJsonAsync("/api/Personas/CreatePerson", p);
    //if (resp.IsSuccessStatusCode)
    //{
    //    Console.WriteLine(resp.Headers.Location);
    //    Console.WriteLine(await resp.Content.ReadAsStringAsync());
    //}
    //else
    //    Console.WriteLine("Error en la petición");

    var p = new Persona() { Nombre = "Tito Agus" };
    var content = new StringContent(JsonSerializer.Serialize(p), Encoding.UTF8, "application/json");
    var respuesta = await client.PostAsync("/api/Personas/CreatePerson", content);
    if (respuesta.IsSuccessStatusCode)
    {

    }
    else
    {

    }
}