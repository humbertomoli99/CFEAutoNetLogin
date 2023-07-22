# ReauthCfeConnect
## Descripción
CFEAutoNetLogin es una aplicación de consola desarrollada en C# que permite automatizar el proceso de inicio de sesión en la red WiFi proporcionada por CFE (Comisión Federal de Electricidad) para acceder a Internet de forma gratuita en puntos de atención prioritaria y/o sitios públicos.

La aplicación utiliza Selenium WebDriver para abrir un navegador web, navegar a la página de inicio de sesión y completar automáticamente el proceso de aceptar los términos y condiciones para establecer la conexión a Internet.

## Requisitos
Windows OS (probado en Windows 10)
.NET 5.0 o superior
Google Chrome (la aplicación utiliza ChromeDriver para interactuar con el navegador)
## Instalación
Clona el repositorio o descarga el código fuente.

Asegúrate de tener instalado .NET 5.0 o superior en tu sistema.

Instala Google Chrome si aún no lo tienes instalado en tu máquina.

Descarga la versión correspondiente de ChromeDriver que coincida con la versión de Chrome instalada. Coloca el archivo ejecutable (chromedriver.exe) en una ubicación accesible para la aplicación.

## Uso
Abre el archivo "Program.cs" y asegúrate de configurar la ruta correcta de ChromeDriver en la línea donde se crea la instancia de ChromeDriver.

Ejecuta la aplicación desde la línea de comandos o desde Visual Studio.

La aplicación abrirá automáticamente el navegador, navegará a la página de inicio de sesión y realizará el proceso de aceptar los términos y condiciones.

Una vez que la conexión a Internet esté establecida, la aplicación se cerrará automáticamente.

## Notas
La aplicación se ha probado en un entorno específico y puede requerir ajustes si se utiliza en diferentes redes o versiones de software.

Asegúrate de cumplir con los términos y condiciones proporcionados por CFE para el uso de su red WiFi gratuita.

La aplicación no almacena ni transmite ninguna información de usuario y solo se utiliza para automatizar el proceso de inicio de sesión.

## Contribución
Si encuentras algún problema o deseas mejorar la aplicación, siéntete libre de hacer un pull request o abrir un issue en el repositorio.

## Licencia
Este proyecto se distribuye bajo la licencia MIT. Consulta el archivo LICENSE para más detalles.

¡Gracias por usar CFEAutoNetLogin! Espero que esta aplicación te sea útil. Si tienes alguna duda, sugerencia o mejora, no dudes en compartirlo. ¡Disfruta de tu conexión a Internet gratuita con CFE!
