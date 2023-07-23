using OpenQA.Selenium.Chrome;
using ManagedNativeWifi;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.RegularExpressions;

class ReauthCfeConnect
{
    static void Main(string[] args)
    {
        // Intervalo de tiempo entre verificaciones de conexión (en milisegundos)
        int checkInterval = 10000; // 10 segundos

        // Ruta al ejecutable del ChromeDriver
        string chromeDriverPath = "C:\\chromedriver_win32\\chromedriver.exe";
        var chromeDriverService = ChromeDriverService.CreateDefaultService(chromeDriverPath);

        // Ocultando la terminal de chromedriver
        chromeDriverService.HideCommandPromptWindow = true;

        // Configura las opciones de Chrome
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless"); // Ejecuta el navegador en modo headless (sin interfaz gráfica)
        options.AddArgument("--window-size=1920x1080"); // Tamaño de ventana (opcional)

        // Verifica el nombre (SSID) de la red WiFi actual
        string currentSSID = GetCurrentWifiSSID();
        Console.WriteLine("Nombre de la red WiFi actual: " + currentSSID);

        // Ejecuta la detección de conexión a internet en segundo plano
        var monitorThread = new Thread(async () =>
        {
            while (true)
            {
                // Comprueba si el SSID coincide con el esperado antes de iniciar el inicio de sesión
                string expectedSSID = "CFE Internet"; // Agrega aquí el SSID esperado para la red WiFi
                if (currentSSID == expectedSSID)
                {
                    // Verifica si hay conexión a Internet antes de proceder
                    bool isConnected = IsInternetConnected();
                    if (!isConnected)
                    {
                        Console.WriteLine("No hay conexión a Internet en la red WiFi actual.");

                        // Realizar el proceso de inicio de sesión o reconexión, ya que estamos en la red WiFi correcta pero no hay conexión a Internet.

                        // Detecta si hay un portal cautivo y obtiene la URL de redirección si es necesario
                        string captivePortalRedirectUrl = await GetFinalRedirectUrl("http://clients3.google.com/generate_204");
                        if (!string.IsNullOrEmpty(captivePortalRedirectUrl))
                        {
                            // Realizar acciones específicas para la redirección del portal cautivo si es necesario
                            // Por ejemplo, navegar a la URL de redirección o realizar una acción especial
                            Console.WriteLine("Portal cautivo detectado. Redirigiendo a: " + captivePortalRedirectUrl);

                            // Configura el directorio donde se encuentra el ChromeDriver.exe
                            ChromeDriver driver = new ChromeDriver(chromeDriverService, options);

                            // Navega a la página de inicio de sesión para ir a esa página y hacer el proceso de reconexión
                            driver.Navigate().GoToUrl(captivePortalRedirectUrl);

                            // A continuación, puedes continuar con el proceso de reconexión o las acciones necesarias para completar el inicio de sesión en la red cautiva.

                            // Espera un poco para asegurarse de que la conexión se haya establecido correctamente
                            Thread.Sleep(5000); // 5000 milisegundos = 5 segundos

                            // Cierra el navegador
                            driver.Quit();
                        }
                        else
                        {
                            // Continuar con el proceso normal de inicio de sesión, ya que no se detectó un portal cautivo
                            Console.WriteLine("No se detectó un portal cautivo.");
                        }
                    }
                    // Espera el intervalo de tiempo antes de la próxima verificación
                    Thread.Sleep(checkInterval);
                }
                else
                {
                    Console.WriteLine("La red WiFi actual no coincide con el SSID esperado.");
                }
            }
        });
        monitorThread.Start();
    }

    // Función para obtener el nombre (SSID) de la red WiFi actual
    static string GetCurrentWifiSSID()
    {
        try
        {
            return NativeWifi.EnumerateAvailableNetworkSsids()
                .Select(ssid => ssid.ToString())
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al obtener el nombre de la red WiFi: " + ex.Message);
            return string.Empty;
        }
    }

    // Función para detectar un portal cautivo y obtener la URL de redirección si es necesario
    static string DetectCaptivePortalAndRedirect()
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                // Establece el User-Agent para simular un navegador web estándar
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36");

                // Realiza la solicitud a una página conocida para detectar el portal cautivo
                HttpResponseMessage response = httpClient.GetAsync("http://clients3.google.com/generate_204").Result;

                if (!response.IsSuccessStatusCode)
                {
                    // Se ha detectado un portal cautivo
                    // Intentamos obtener la URL de redirección
                    Uri redirectUri = response.Headers.Location;
                    if (redirectUri != null)
                    {
                        return redirectUri.AbsoluteUri;
                    }
                }
            }
            catch
            {
                // Manejo de errores si la detección del portal cautivo falla
            }
        }
        return null;
    }


    // Función para verificar la conexión a internet mediante ping
    static bool IsInternetConnected()
    {
        using (var ping = new Ping())
        {
            try
            {
                // Puedes hacer ping a un servidor conocido como www.google.com
                // o a la dirección IP de un servidor externo
                PingReply reply = ping.Send("www.google.com");
                return (reply?.Status == IPStatus.Success);
            }
            catch
            {
                // Si ocurre alguna excepción, asumimos que no hay conexión a internet
                return false;
            }
        }
    }

    public static async Task<string> GetFinalRedirectUrl(string initialUrl)
    {
        if (string.IsNullOrEmpty(initialUrl))
        {
            return null;
        }

        initialUrl = await NormalizeUrl(initialUrl);

        // Check if the URL is valid according to the URL syntax
        var regex = new Regex(@"^(https?://)?([\w-]+\.)+[\w-]+(/[^\s]*)?$");

        if (!regex.IsMatch(initialUrl))
        {
            Console.WriteLine($"The URL is not valid according to the regular expression: {initialUrl}");
            return null;
        }

        var maxRedirections = 10;
        var currentRedirections = 0;

        while (currentRedirections < maxRedirections)
        {
            // Create an HTTP client with AllowAutoRedirect set to false
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                // Send an HTTP GET request to the URL
                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync(initialUrl);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"An exception occurred while sending the HTTP request to {initialUrl}: {ex.Message}");
                    return null;
                }

                if (response.StatusCode == HttpStatusCode.Redirect ||
                    response.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    var locationHeader = response.Headers.Location?.ToString();

                    if (!string.IsNullOrEmpty(locationHeader))
                    {
                        // Update the URL with the redirection location
                        initialUrl = locationHeader;
                        currentRedirections++;
                        continue;
                    }
                }

                // If there are no more redirections, return the current URL
                return initialUrl;
            }
        }

        // If the maximum number of redirections is reached, return the current URL
        return initialUrl;
    }
    public static async Task<string> NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        // Agregar el esquema https si no está especificado en la URL
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = "https://" + url;
        }

        // Validar la URL antes de crear el objeto Uri
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return null; // la URL no es válida, retornar null o lanzar una excepción si es necesario
        }

        // Convertir el host a minúsculas
        var host = uri.Host.ToLower();


        // Reconstruir la URL con el host en minúsculas y con el prefijo "www"
        var path = uri.AbsolutePath + uri.Query;
        var normalizedUrl = new UriBuilder(string.Format("{0}://{1}:{2}{3}", uri.Scheme, host, uri.Port, path)).ToString();

        // Eliminar el puerto 80 o 443 al final de la URL
        normalizedUrl = normalizedUrl.Replace(":80", "");
        normalizedUrl = normalizedUrl.Replace(":443", "");

        return normalizedUrl;
    }
}
