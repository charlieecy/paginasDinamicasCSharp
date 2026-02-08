namespace Blazor.Models;

// ViewModel para la vista Index
// Encapsula todos los datos que necesita la vista de listado
public class FunkoIndexViewModel
{
    // Lista de Funkos a mostrar en la tabla
    public IEnumerable<Backend.DTO.FunkoResponseDTO> Funkos { get; set; } = new List<Backend.DTO.FunkoResponseDTO>();
    
    // Lista de los últimos 3 Funkos visitados (desde la sesión)
    public List<Backend.DTO.FunkoResponseDTO> VistosRecientemente { get; set; } = new List<Backend.DTO.FunkoResponseDTO>();
    
    // Filtro de búsqueda actual (para mantener el valor en el input)
    public string? Nombre { get; set; }
    
    // Número de página actual (para la paginación)
    public int PageNumber { get; set; } = 1;
    
    // Total de páginas disponibles (para pintar los botones de navegación)
    public int TotalPages { get; set; }
    
    // Propiedad auxiliar: verifica si hay páginas anteriores
    public bool HasPreviousPage => PageNumber > 1;
    
    // Propiedad auxiliar: verifica si hay páginas siguientes
    public bool HasNextPage => PageNumber < TotalPages;
}