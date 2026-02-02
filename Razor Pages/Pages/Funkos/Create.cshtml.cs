using Backend.DTO;
using Backend.Service;
using Backend.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor_Pages.Pages.Funkos;

public class CreateModel (
    IFunkoService service, 
    IFunkoStorage storage
    ) : PageModel
{
    //Declaramos el Funko
    [BindProperty] public FunkoPostPutRequestDTO Form { get; set; } = new();
    
    //Declaramos la imagen
    [BindProperty]
    public IFormFile? ImageFile { get; set; }
    public async Task<IActionResult> OnPostAsync()
    {
        //Si hay errores de validación, volvemos a Create.cshtml
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        // Si el usuario ha subido un archivo, lo guardamos
        if (ImageFile != null && ImageFile.Length > 0)
        {
            // Guardamos en la carpeta "funkos"
            var saveResult = await storage.SaveFileAsync(ImageFile, "funkos");

            if (saveResult.IsSuccess)
            {
                // Si se guarda bien, actualizamos la ruta en el DTO
                Form.Imagen = saveResult.Value; 
            }
            else
            {
                // Si hay error al guardar imagen, añadimos error al modelo y volvemos
                ModelState.AddModelError("ImageFile", $"Error guardando imagen: {saveResult.Error.Message}");
                return Page();
            }
        }
        else
        {
            //En caso de que el usuario no incluya imagen, el valor sería default.png 
            Form.Imagen = "/uploads/default.png";

        }
        
        //Intentamos crear el Funko
        var result = await service.CreateAsync(Form);

        if (result.IsFailure)
        {
            //Si hay algún error al guardar, lo inyectamos al modelo y devolvemos la página Create.cshtml
            ModelState.AddModelError(string.Empty, result.Error.Message);
            return Page();
        }
        //Si sale bien, redireccionamos al index
        return RedirectToPage("../Index");
    }
}