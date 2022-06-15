using Comun;
using HttpClientDemo.API;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

//PETICIONES GET

using (var httpclient = new HttpClient())
{
    httpclient.BaseAddress = new Uri("https://localhost:7192");

    //Para recibir Headers y content y despues leer el contenido como string
    //ó las cabeceras si no sinteresan
    var respuesta = await httpclient.GetAsync("/api/Personas");
    if (respuesta.IsSuccessStatusCode)
    {
        var resp = await respuesta.Content.ReadAsStringAsync();
        var personas = JsonSerializer.Deserialize<List<Persona>>(resp, jsonSerializerOptions);
    }
    else
        Console.WriteLine("Ocurrió un error");

    //Para recibir directamente el contenido y luego no es necesario leer el contenido con ReadasString
    string json = await httpclient.GetStringAsync("/api/Personas");
    var personas2 = JsonSerializer.Deserialize<List<Persona>>(json);
    Console.WriteLine("\n\n\n");


}


//PETICIONES POST

using (var client = new HttpClient())
{
    client.BaseAddress = new Uri("https://localhost:7192");
    var p = new Persona() { Nombre = "Tito Agus" };

    //Para enviar un json directamente en la llamada
    var resp = await client.PostAsJsonAsync("/api/Personas/CreatePerson", p);
    if (resp.IsSuccessStatusCode)
    {
        Console.WriteLine(resp.Headers.Location);
        Console.WriteLine(await resp.Content.ReadAsStringAsync());
    }
    else
        Console.WriteLine("Error en la petición");


    p = new Persona() { Nombre = "Tito Agus 2" };
    var content = new StringContent(JsonSerializer.Serialize(p), Encoding.UTF8, "application/json");

    //Para enviar un StringContent y así poder enviar otras cosas diferentes a un json
    var respuesta = await client.PostAsync("/api/Personas/CreatePerson", content);
    if (respuesta.IsSuccessStatusCode)
    {
        Console.WriteLine(resp.Headers.Location);
        Console.WriteLine(await resp.Content.ReadAsStringAsync());
    }
    else
    {
        Console.WriteLine("Error en la petición");
    }
    Console.WriteLine("\n\n\n");
}



//ENVIAMOS INFORMACION EN LA CABECERA

string url = "https://localhost:7192/WeatherForecast";
using (var client = new HttpClient())
{    
    //Mandar un valor una vez si no haremos más peticiones
    using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
    {
        requestMessage.Headers.Add("cantidadelementos", "10");
        var respuesta = await client.SendAsync(requestMessage);
        var contenido = await respuesta.Content.ReadAsStringAsync();
        var listado = JsonSerializer.Deserialize<List<WeatherForecast>>(contenido, jsonSerializerOptions);        
        //Formateamos el json para verlo en la salida correctamente con NewtonSoft
        Console.WriteLine(JToken.Parse(contenido).ToString()); 
        Console.WriteLine("\n\n\n");
        Console.WriteLine($"Número de climas : {listado.Count}");
    }

    //Mandar un valor siempre en la cabecera en todas las peticiones sucesivas
    client.DefaultRequestHeaders.Add("cantidadelementos", "30");
    var aux = await client.GetFromJsonAsync<List<WeatherForecast>>(url, jsonSerializerOptions);
    Console.WriteLine($"Cantidad de climas : {aux.Count}");

}