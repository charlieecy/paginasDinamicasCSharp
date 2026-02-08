using System.Diagnostics;
using Backend.DTO;
using Backend.Service;
using Backend.Storage;
using Microsoft.AspNetCore.Mvc;
using Blazor.Models;
using Blazor.Session;

namespace Blazor.Controllers;

public class FunkosController (IFunkoService service, IFunkoStorage storage) : Controller
{
    
    // GET: /Funkos/Index
// Método principal que muestra el listado de Funkos con búsqueda y paginación
[HttpGet]
public async Task<IActionResult> Index(string? nombre, int pageNumber = 1)
{
    // PARÁMETROS:
    // - nombre: Viene de la URL como query string (?nombre=Batman)
    //           Se captura automáticamente por el model binding de ASP.NET
    // - pageNumber: Número de página actual, por defecto 1
    //               También viene de la URL (?pageNumber=2)
    
    // Creamos el objeto de filtrado usando los parámetros recibidos
    var filter = new FilterDTO(nombre, null, null, pageNumber, 10, "id", "asc");

    // Llamamos al servicio pasándole el filtro
    var result = await service.GetAllAsync(filter);

    // Creamos el ViewModel que pasaremos a la vista
    var viewModel = new FunkoIndexViewModel();

    // Comprobamos si el resultado del servicio fue exitoso
    if (result.IsSuccess)
    {
        // Poblamos el ViewModel con los datos obtenidos
        viewModel.Funkos = result.Value.Items; // Lista de Funkos a mostrar
        viewModel.Nombre = nombre; // Para mantener el valor en el input de búsqueda
        viewModel.PageNumber = pageNumber; // Página actual
        
        // Calculamos el total de páginas
        // Total de elementos (11) / Tamaño (10) = 1.1 -> Redondeamos hacia arriba = 2 páginas
        viewModel.TotalPages = (int)Math.Ceiling((double)result.Value.TotalCount / 10);
        
        // LEER DE LA SESIÓN: Recuperamos los vistos recientemente
        viewModel.VistosRecientemente = HttpContext.Session.GetJson<List<FunkoResponseDTO>>("VistosRecientemente") 
                                         ?? new List<FunkoResponseDTO>();
    }
    // Si hubo error, el ViewModel ya tiene listas vacías por defecto (inicializadas en la clase)

    // Pasamos el ViewModel a la vista
    return View(viewModel);
}
    
    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        var result = await service.GetByIdAsync(id);

        if (result.IsSuccess)
        {
            var funko = result.Value;
            AddFunkoToSession(funko);

            var viewModel = new FunkoDetailsViewModel{
                Id = funko.Id,
                Nombre = funko.Nombre,
                Categoria = funko.Categoria,
                Precio = funko.Precio,
                Imagen = funko.Imagen
            };
        
            // Pasamos el ViewModel a la vista
            return View(viewModel);
        }
    
        return NotFound();
    }
    
    // MÉTODO PRIVADO: Gestiona la lista de Funkos vistos recientemente en la sesión
// Esta lista se muestra en la página Index para mejorar la experiencia del usuario
    private void AddFunkoToSession(FunkoResponseDTO funko)
    {
        // Recuperamos la lista actual de la sesión
        // Si no existe (es null), creamos una nueva lista vacía
        var vistosRecientemente = HttpContext.Session.GetJson<List<FunkoResponseDTO>>("VistosRecientemente") 
                                  ?? new List<FunkoResponseDTO>();

        // Evitamos duplicados
        // Si el Funko ya estaba en la lista, lo eliminamos de su posición anterior
        vistosRecientemente.RemoveAll(f => f.Id == funko.Id);

        // Insertamos el Funko al principio de la lista
        vistosRecientemente.Insert(0, funko);

        // Limitamos a 3 elementos máximo
        // Si hay más de 3 Funkos, eliminamos el más antiguo (posición 3) 
        if (vistosRecientemente.Count > 3)
        {
            vistosRecientemente.RemoveAt(3); // Borramos el 4º elemento
        }

        // Guardamos la lista actualizada en la sesión
        HttpContext.Session.SetJson("VistosRecientemente", vistosRecientemente);
    }
    
    // GET: /Funkos/Create
    [HttpGet]
    // Busca la vista en Views/Funkos/Create.cshtml
    //vFunkos lo coge del nombre de la clase, Create del nombre del método
    public IActionResult Create()
    {
        return View(); 
    }

    // POST: /Funkos/Create
    [HttpPost]
    [ValidateAntiForgeryToken] // Seguridad extra para formularios
    public async Task<IActionResult> Create(FunkoPostPutRequestDTO funko, IFormFile? imagen)
    {
        // Validación del DTO
        if (!ModelState.IsValid)
        {
            return View(funko); // Si falla, devolvemos la misma vista con los errores y los datos
        }

        // Guardamos la imagen
        if (imagen != null && imagen.Length > 0)
        {
            var saveResult = await storage.SaveFileAsync(imagen, "funkos");
            if (saveResult.IsSuccess)
            {
                funko.Imagen = saveResult.Value;
            }
            else
            {
                ModelState.AddModelError("Imagen", "Error al subir la imagen");
                return View(funko);
            }
        }
        else
        {
            // Si no se subió imagen, le asignamos la imagen por defecto
            funko.Imagen = "/uploads/default.png";
        }

        // Creamos el Funko
        var result = await service.CreateAsync(funko);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Message);
            return View(funko);
        }

        //Si se guarda con éxito, redirigimos al index
        //Patrón POST - REDIRECT - GET
        // TempData: Diccionario que sobrevive a UNA redirección (perfecto para mensajes flash)
        // Se almacena en sesión temporalmente y se elimina después de ser leído
        TempData["Creado"] = $"{result.Value.Nombre} fue creado con éxito.";
        return RedirectToAction(nameof(Index));
    }
    
    // GET: /Funkos/Update/5
// Muestra el formulario de edición con los datos actuales del Funko
[HttpGet]
public async Task<IActionResult> Update(long id)
{
    // PARÁMETRO:
    // - id: Se captura de la URL /Funkos/Update/5
    //       Viene de asp-route-id="@item.Id" en las vistas Index y Details
    
    // Buscamos el Funko que queremos editar
    var result = await service.GetByIdAsync(id);

    // Si no existe, devolvemos un error 404 Not Found
    if (result.IsFailure)
    {
        return NotFound();
    }

    // Extraemos el Funko del resultado
    var funko = result.Value;
    
    // Creamos el ViewModel y lo poblamos con los datos actuales del Funko
    // Esto permite que el formulario se cargue con los valores existentes
    var viewModel = new FunkoUpdateViewModel
    {
        Id = funko.Id,
        
        // Poblamos el Form (DTO) con los valores actuales
        Form = new FunkoPostPutRequestDTO
        {
            Nombre = funko.Nombre,
            Categoria = funko.Categoria,
            Precio = funko.Precio,
            Imagen = funko.Imagen // Ruta de la imagen actual
        }
        // ImageFile se queda null (el usuario puede o no subir una nueva imagen)
    };

    // Pasamos el ViewModel a la vista Update.cshtml
    return View(viewModel);
}

// POST: /Funkos/Update/5
// Procesa el formulario de edición y actualiza el Funko
[HttpPost]
[ValidateAntiForgeryToken] // Protección CSRF
public async Task<IActionResult> Update(long id, FunkoUpdateViewModel viewModel)
{
    // PARÁMETROS:
    // - id: Viene de la URL (asp-route-id en el form)
    // - viewModel: Se puebla automáticamente con los datos del formulario
    //              ASP.NET mapea los campos del form a las propiedades del ViewModel
    
    // VALIDACIÓN: Comprobamos si el modelo cumple las reglas de validación
    // (anotaciones [Required], [Range], etc. del FunkoPostPutRequestDTO)
    if (!ModelState.IsValid)
    {
        // Si hay errores, volvemos a mostrar el formulario con los datos y errores
        // El usuario verá los mensajes de error junto a cada campo
        return View(viewModel);
    }

    // Si el usuario subió una nueva imagen, la guardamos
    if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
    {
        // Guardamos el archivo en la carpeta "funkos" del storage
        var saveResult = await storage.SaveFileAsync(viewModel.ImageFile, "funkos");

        if (saveResult.IsSuccess)
        {
            // Si se guardó correctamente, actualizamos la ruta en el DTO
            // Esto reemplaza la imagen anterior
            viewModel.Form.Imagen = saveResult.Value;
        }
        else
        {
            // Si hubo error al guardar, añadimos el error al ModelState
            // y devolvemos la vista con el mensaje de error
            ModelState.AddModelError(nameof(viewModel.ImageFile), 
                $"Error guardando imagen: {saveResult.Error.Message}");
            return View(viewModel);
        }
    }
    // Si NO se subió imagen (ImageFile es null), viewModel.Form.Imagen mantiene
    // el valor que traía del hidden field (la imagen actual del Funko)

    // ACTUALIZACIÓN: Intentamos actualizar el Funko en el servicio
    var result = await service.UpdateAsync(id, viewModel.Form);

    if (result.IsFailure)
    {
        // Si hubo error al actualizar (ej: el Funko ya no existe, error de BD, etc.)
        // añadimos el error al ModelState y volvemos a mostrar el formulario
        ModelState.AddModelError(string.Empty, result.Error.Message);
        return View(viewModel);
    }

    // ÉXITO: Si salió bien, creamos un mensaje flash y redirigimos
    // PATRÓN POST-REDIRECT-GET: Evita que el usuario reenvíe el formulario al refrescar
    TempData["Actualizado"] = $"{viewModel.Form.Nombre} fue actualizado con éxito.";
    
    // Redirigimos al Index para ver la lista actualizada
    return RedirectToAction(nameof(Index));
}

    // POST: /Funkos/Delete
    // Método para eliminar un Funko
    [HttpPost]
    [ValidateAntiForgeryToken] // Protección contra CSRF
    public async Task<IActionResult> Delete(long id)
    {
        // El parámetro 'id' se captura automáticamente desde el formulario
        // En la vista tenemos: <form asp-action="Delete" asp-route-id="@item.Id">
        
        // Llamamos al servicio para borrar
        var result = await service.DeleteAsync(id);

        // Comprobamos si fue bien
        if (result.IsSuccess)
        {
            // PATRÓN POST-REDIRECT-GET
            // TempData: Diccionario que sobrevive a UNA redirección (perfecto para mensajes flash)
            // Se almacena en sesión temporalmente y se elimina después de ser leído
            TempData["Eliminado"] = $"{result.Value.Nombre} fue eliminado con éxito.";
            
            // Redirigimos al Index para recargar la lista
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // Si no se encontró o hubo error, devolvemos 404
            return NotFound();
        }
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}