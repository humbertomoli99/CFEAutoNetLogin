using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ManagedNativeWifi;
using System;
using System.Linq;

class ReauthCfeConnect
{
    static void Main(string[] args)
    {
        // Verifica el nombre (SSID) de la red WiFi actual
        string currentSSID = GetCurrentWifiSSID();
        Console.WriteLine("Nombre de la red WiFi actual: " + currentSSID);

        // Comprueba si el SSID coincide con el esperado antes de iniciar el inicio de sesión
        string expectedSSID = "CFE Internet"; // Agrega aquí el SSID esperado para la red WiFi
        if (currentSSID == expectedSSID)
        {
            // Detecta si hay un portal cautivo y obtiene la URL de redirección si es necesario
            string captivePortalRedirectUrl = DetectCaptivePortalAndRedirect();
            if (!string.IsNullOrEmpty(captivePortalRedirectUrl))
            {
                // Realizar acciones específicas para la redirección del portal cautivo si es necesario
                // Por ejemplo, navegar a la URL de redirección o realizar una acción especial
                Console.WriteLine("Portal cautivo detectado. Redirigiendo a: " + captivePortalRedirectUrl);

                // Ruta al ejecutable del ChromeDriver
                string chromeDriverPath = "C:\\chromedriver_win32\\chromedriver.exe";
                var chromeDriverService = ChromeDriverService.CreateDefaultService(chromeDriverPath);

                // Ocultando la terminal de chromedriver
                chromeDriverService.HideCommandPromptWindow = true;

                // Configura las opciones de Chrome
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--headless"); // Ejecuta el navegador en modo headless (sin interfaz gráfica)
                options.AddArgument("--window-size=1920x1080"); // Tamaño de ventana (opcional)

                // Configura el directorio donde se encuentra el ChromeDriver.exe
                ChromeDriver driver = new ChromeDriver(chromeDriverService, options);

                // Navega a la página de inicio de sesión para ir a esa página y hacer el proceso de reconexión
                driver.Navigate().GoToUrl(captivePortalRedirectUrl);

                // A continuación, puedes continuar con el proceso de reconexión o las acciones necesarias para completar el inicio de sesión en la red cautiva.

                // Espera un poco para asegurarse de que la conexión se haya establecido correctamente
                System.Threading.Thread.Sleep(5000); // 5000 milisegundos = 5 segundos

                // Cierra el navegador
                driver.Quit();
            }
            else
            {
                // Continuar con el proceso normal de inicio de sesión, ya que no se detectó un portal cautivo
                Console.WriteLine("No se detectó un portal cautivo.");
            }
        }
        else
        {
            Console.WriteLine("La red WiFi actual no coincide con el SSID esperado.");
        }
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
}
