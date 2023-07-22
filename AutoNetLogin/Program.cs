using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using ManagedNativeWifi;

class CFEAutoNetLogin
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
            // Configura el directorio donde se encuentra el ChromeDriver.exe
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized"); // Maximiza la ventana del navegador (opcional)
            ChromeDriver driver = new ChromeDriver("C:\\chromedriver_win32\\chromedriver.exe", options);

            // Navega a la página de inicio de sesión
            driver.Navigate().GoToUrl("https://acs.cfeteit.gob.mx:19008/portalpage/817e256e-2bf1-45a1-a504-9a6b8e17018f/20210905130555/pc/auth.html?apmac=30c50fd52c10&uaddress=10.1.1.68&umac=000db0049c8e&authType=2&lang=en_US&ssid=Q0ZFIEludGVybmV0&pushPageId=e2c9d848-3565-4065-b796-05fb3b0dfa6a");

            // Encuentra el elemento <label> con 'for="option-aceptar"'
            IWebElement labelAceptar = driver.FindElement(By.CssSelector("label[for='option-aceptar']"));

            // Hacer clic en el elemento <label>
            labelAceptar.Click();

            // Encuentra el elemento <input> con 'id="annoyBtn"'
            IWebElement btnEntrar = driver.FindElement(By.Id("annoyBtn"));

            // Hacer clic en el botón "Entrar"
            btnEntrar.Click();

            // Espera un poco para asegurarse de que la conexión se haya establecido correctamente
            System.Threading.Thread.Sleep(5000); // 5000 milisegundos = 5 segundos

            // Cierra el navegador
            driver.Quit();
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
}
