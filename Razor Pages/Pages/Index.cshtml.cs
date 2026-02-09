using Backend.DTO;
using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Pages.Session;

namespace Razor_Pages.Pages;

public class IndexModel(IFunkoService service) : PageModel
{
    
    // Declaramos la propiedad que almacenará los tres últimos Funkos visitados
    public List<FunkoResponseDTO> VistosRecientemente { get; private set; } = [];
    
    // Declaramos la propiedad que almacenará la lista de Funkos para mostrarla en la vista
    public IEnumerable<FunkoResponseDTO> Funkos { get; private set; } = [];

    // Vinculamos esta propiedad con el input del buscador de la vista
    // Usamos SupportsGet = true para que capture el valor desde la URL (ej: ?Nombre=Batman)
    [BindProperty(SupportsGet = true)]
    public string? Nombre { get; set; }
    
    // Capturamos el número de página de la URL (por defecto 1)
    // Usamos SupportsGet = true para que capture el valor desde la URL (ej: ?Page=2)
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    // Propiedad para saber cuántas páginas hay en total y pintar los botones
    public int TotalPages { get; set; }

    // Este método se ejecuta automáticamente cuando cargamos la página
    public async Task OnGetAsync()
    {
        // Creamos el objeto de filtrado usando el valor que hemos recibido en la propiedad Nombre
        // Establecemos valores por defecto para paginación (página 1, tamaño 10) y ordenación
        var filter = new FilterDTO(Nombre, null, null, PageNumber, 10, "id", "asc");

        // Llamamos al servicio pasándole el filtro por nombre que acabamos de crear
        var result = await service.GetAllAsync(filter);

        // Comprobamos si el resultado del servicio fue exitoso
        if (result.IsSuccess)
        {
            // Si fue bien, extraemos la lista de items del objeto PageResponse y la guardamos
            Funkos = result.Value.Items;
            
            // Total de elementos (11) / Tamaño (10) = 1.1 -> Redondeamos hacia arriba (Ceiling) = 2 páginas
            TotalPages = (int)Math.Ceiling((double)result.Value.TotalCount / 10);
            
            // LEER DE LA SESIÓN: Recuperamos los vistos recientemente para pasarlos a la vista
            VistosRecientemente = HttpContext.Session.GetJson<List<FunkoResponseDTO>>("VistosRecientemente") ?? new();
        }
        else
        {
            // Si hubo algún error o no hay datos, inicializamos la lista como vacía para evitar nulos
            Funkos = [];
        }
    }
    
    public async Task<IActionResult> OnPostDeleteAsync(long id)
    {
        
        // Si el usuario no está autenticado -> Forzar Login (Challenge)
        if (!User.Identity.IsAuthenticated)
        {
            return Challenge(); 
        }

        // Si está logueado pero NO es Admin -> Prohibir (Forbid)
        if (!User.IsInRole("Admin"))
        {
            return Forbid();
        }
        //Llamamos al servicio para borrar
        var result = await service.DeleteAsync(id);

        // Comprobamos si fue bien
        if (result.IsSuccess)
        {
            // PATRÓN POST-REDIRECT-GET
            // Si se borró, recargamos la página actual (esto vuelve a ejecutar OnGetAsync)
            // Creamos el mensaje flash (sobrevive a una redirección) y redireccionamos al index
            TempData["Eliminado"] = $"{result.Value.Nombre} fue eliminado con éxito.";
            return RedirectToPage();
        }
        else
        {
            // Si no se encontró o hubo error, devolvemos 404
            return NotFound();
        }
    }
}