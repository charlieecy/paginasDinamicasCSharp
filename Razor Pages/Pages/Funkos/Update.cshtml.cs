using Backend.DTO;
using Backend.Service;
using Backend.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor_Pages.Pages.Funkos;

[Authorize(Policy = "EsAdmin")]
public class UpdateModel(
    IFunkoService service,
    IFunkoStorage storage
) : PageModel
{
    //Declaramos el Funko
    [BindProperty] public FunkoPostPutRequestDTO Form { get; set; } = default!;

    //Declaramos la imagen
    [BindProperty] public IFormFile? ImageFile { get; set; }

    public async Task<IActionResult> OnGetAsync(long id)
    {
        //Comprobamos si existe el funko a actualizar
        var result = await service.GetByIdAsync(id);

        //Si no existe, devolvemos NotFound
        if (result.IsFailure)
        {
            return NotFound();
        }
        
        //Si existe, le asignamos a los campos del formulario (FunkoPostPutRequestDTO)
        //los valores encontrados, para que se carguen por defecto en la vista de actualizar
        Form = new FunkoPostPutRequestDTO
        {
            Nombre = result.Value.Nombre,
            Categoria = result.Value.Categoria,
            Precio = result.Value.Precio,
            Imagen = result.Value.Imagen 
        };
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long id)
    {
        //Si hay errores de validación, volvemos a Update.cshtml
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
        //En caso de que el usuario no suba un archivo, se quedaría tal cual vino
        //el valor al cargar la página de editar al hacer el get.
        
        //Intentamos actualizar el Funko
        var result = await service.UpdateAsync(id, Form);
        
        if (result.IsFailure)
        {
            //Si hay algún error al guardar, lo inyectamos al modelo y devolvemos la página Create.cshtml
            ModelState.AddModelError(string.Empty, result.Error.Message);
            return Page();
        }
        //Si sale bien, creamos el mensaje flash (sobrevive a una redirección) y redireccionamos al index
        TempData["Actualizado"] = $"{Form.Nombre} fue actualizado con éxito.";
        return RedirectToPage("../Index");

    }
}