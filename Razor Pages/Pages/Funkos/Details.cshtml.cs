using Backend.DTO;
using Backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor_Pages.Pages.Funkos;

public class DetailsModel (IFunkoService service) : PageModel
{
    public FunkoResponseDTO Funko { get; private set; } = default!;

    public async Task<IActionResult> OnGet(long id)
    {
        //Buscamos el funko en el servicio
        var funko = await service.GetByIdAsync(id);

        //Si lo encontramos, devolvemos la página Details.cshtml
        if (funko.IsSuccess)
        {
            Funko = funko.Value;
            return  Page();
        }
        //Si no, devolvemos un NotFound, que directamente 
        return NotFound();
    }
    
}