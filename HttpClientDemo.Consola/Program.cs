using Comun;
using HttpClientDemo.API;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;




//Instalamos paquete Microsoft.extension.Http
//Configurar el sistema de Inyección de independencia


var serviceCollection = new ServiceCollection();
Configure(serviceCollection);
var httpclientFactory = serviceCollection
                        .BuildServiceProvider()
                        .GetService<IHttpClientFactory>();


void Configure(ServiceCollection service)
{
    service.AddHttpClient("personas", opciones =>
    {
        opciones.BaseAddress = new Uri("https://localhost:7192");
    });
    service.AddHttpClient("weatherForeCast", opciones =>
    {
        opciones.BaseAddress = new Uri("https://localhost:7192/WeatherForecast");
        opciones.DefaultRequestHeaders.Add("cantidadElementos", "25");
    });
}

var hHttpClientPersona = httpclientFactory.CreateClient("persona");
var responseMessage3 = await hHttpClientPersona.GetAsync("");
responseMessage3.EnsureSuccessStatusCode();


//var httpClient = new ServiceCollection()
//                .AddHttpClient("personas", opciones =>
//                {
//                    opciones.BaseAddress = new Uri("https://localhost:7192");
//                })
//                .BuildServiceProvider()
//                .GetService<IHttpClientFactory>()
//                .CreateClient();

var responseMessage2 = await httpClient.GetAsync(laurl);
responseMessage2.EnsureSuccessStatusCode();








string urlCuentas = "https://localhost:5001/api/cuentas";
UserInfo credenciales = new UserInfo() { Email = "felipe@hotmail.com", Password = "aA123456!" };
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



//ENVIAMOS INFORMACION EN LA CABECERA (PARA GET Y POST)

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

    //Mandar JWT
    await CrearUsuario();
    var httpRespuestaToken = await client.PostAsJsonAsync($"{urlCuentas}/login", credenciales);
    var respuestaToken = JsonSerializer.Deserialize<UserToken>(await
        httpRespuestaToken.Content.ReadAsStringAsync(), jsonSerializerOptions);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                    respuestaToken.Token);
    var respuestaJwt = await client.PostAsJsonAsync(url, new Persona() { Nombre = "Felipe" });
    respuestaJwt.EnsureSuccessStatusCode();
    Console.WriteLine("Persona creada de manera exitosa");

}




async Task CrearUsuario()
{
    try
    {
        using (var httpClient = new HttpClient())
        {
            var respuesta = await httpClient.PostAsJsonAsync($"{urlCuentas}/crear", credenciales);
            if (respuesta.StatusCode == HttpStatusCode.InternalServerError)
            {
                respuesta.EnsureSuccessStatusCode();
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}


//PUT (Modifica una entidad completa, es decir, necesitamos enviar toda la entidad para hacer un put puesto que los valore sno enviados serán modificados a null
//DELETE
using (var client = new HttpClient())
{
    url = "https://localhost:7192";
    var persona = new Persona { Nombre = "Nombre de persona" };
    var responseMessage = await client.PostAsJsonAsync(url, persona);
    responseMessage.EnsureSuccessStatusCode();
    var content = await responseMessage.Content.ReadAsStringAsync();
    var idPersona = int.Parse(content);

    // Ejemplo 1: Hacer un HTTP PUT
    persona.Id = idPersona;
    persona.Nombre = "Modificamos elnombre de persona";
    await client.PutAsJsonAsync($"{url}/{idPersona}", persona);

    var personas = await client.GetFromJsonAsync<List<Persona>>(url);

    foreach (var p in personas)
    {
        Console.WriteLine($"Id: {p.Id} - Nombre: {p.Nombre}");
    }

    // Ejemplo 2: Hacer un HTTP DELETE

    await client.DeleteAsync($"{url}/{idPersona}");
    var personas2 = await client.GetFromJsonAsync<List<Persona>>(url);

    if (personas2.Count == 0)
    {
        Console.WriteLine("No hay registros en la tabla de personas");
    }
}

Console.Read();


