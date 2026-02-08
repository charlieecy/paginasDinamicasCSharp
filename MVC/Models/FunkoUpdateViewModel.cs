using System.ComponentModel.DataAnnotations;
using Backend.DTO;

namespace Blazor.Models;

// ViewModel para la vista Update
// Encapsula el DTO del formulario y el ID del Funko a actualizar
public class FunkoUpdateViewModel
{
    // ID del Funko que estamos editando
    // Se pasa como parámetro de ruta y como hidden field en el formulario
    [Required]
    public long Id { get; set; }
    
    // DTO con los datos del formulario
    // Contiene: Nombre, Categoria, Precio, Imagen
    public FunkoPostPutRequestDTO Form { get; set; } = new();
    
    // Archivo de imagen opcional que el usuario puede subir
    // Si es null, mantenemos la imagen actual
    public IFormFile? ImageFile { get; set; }
}