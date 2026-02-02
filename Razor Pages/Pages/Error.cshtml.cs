using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor_Pages.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    // Propiedades nuevas para controlar el mensaje
    public int StatusCode { get; set; }
    public string ErrorTitle { get; set; } = "Error";
    public string ErrorMessage { get; set; } = "Ha ocurrido un error inesperado.";
    public string Icon { get; set; } = "bi-exclamation-triangle";

    // El parámetro 'code' viene del Program.cs: "?code={0}"
    public void OnGet(int? code)
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        StatusCode = code ?? 500; // Si no hay código, asumimos error 500

        // Personalizamos el mensaje según el código
        switch (StatusCode)
        {
            case 404:
                ErrorTitle = "Página no encontrada";
                ErrorMessage = "Lo sentimos, no hemos podido encontrar el Funko o la página que buscas.";
                Icon = "bi-search"; // Icono de lupa
                break;
            
            case 500:
                ErrorTitle = "Error del Servidor";
                ErrorMessage = "Algo ha salido mal en nuestros servidores. Por favor, inténtalo más tarde.";
                Icon = "bi-server";
                break;

            default:
                ErrorTitle = $"Error {StatusCode}";
                ErrorMessage = "Ha ocurrido un problema al procesar tu solicitud.";
                break;
        }
    }
}