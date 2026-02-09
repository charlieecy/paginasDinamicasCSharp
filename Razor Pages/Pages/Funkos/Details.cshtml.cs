using Backend.DTO;
using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Pages.Session;

namespace Razor_Pages.Pages.Funkos;
[Authorize (Roles = "User, Admin")]
public class DetailsModel (IFunkoService service) : PageModel
{
    public FunkoResponseDTO Funko { get; private set; } = default!;

    public async Task<IActionResult> OnGet(long id)
    {
        //Buscamos el funko en el servicio
        var funko = await service.GetByIdAsync(id);

        //Si lo encontramos
        if (funko.IsSuccess)
        {
            Funko = funko.Value;
            //Lo añadimos a la lista de los 3 últimos vistos
            AddFunkoToSession(Funko);
            //Y redireccionamos a Details.cshtml
            return  Page();
        }
        //Si no, devolvemos un NotFound, que directamente 
        return NotFound();
    }

    private void AddFunkoToSession(FunkoResponseDTO funko)
    {
     
        //Recuperamos la lista actual de la sesión (o creamos una vacía si es null)
        var vistosRecientemente = HttpContext.Session.GetJson<List<FunkoResponseDTO>>("VistosRecientemente") ?? new();

        //Evitamos duplicados: Si el funko ya estaba en la lista, lo quitamos de su posición anterior
        vistosRecientemente.RemoveAll(f => f.Id == Funko.Id);

        //Insertamos al principio de la lista (el más reciente)
        vistosRecientemente.Insert(0, Funko);

        // 4. Limitamos a 3 elementos (Cola FIFO)
        if (vistosRecientemente.Count > 3)
        {
            vistosRecientemente.RemoveAt(3); // Borramos el 4º elemento (el más antiguo)
        }

        // 5. Guardamos la lista actualizada en la sesión
        HttpContext.Session.SetJson("VistosRecientemente", vistosRecientemente);
    }
    
}