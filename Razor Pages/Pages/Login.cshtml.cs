using System.ComponentModel.DataAnnotations;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razor_Pages.Pages;

public class Login(SignInManager<User> signInManager, ILogger<Login> logger) : PageModel
{


    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }

    public void OnGet()
    {
        // Limpiar errores previos
        ErrorMessage = null;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var result = await signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                logger.LogInformation("Usuario {Email} ha iniciado sesión correctamente.", Input.Email);
                return RedirectToPage("Index");
            }

            if (result.IsLockedOut)
            {
                logger.LogWarning("La cuenta del usuario {Email} está bloqueada.", Input.Email);
                ErrorMessage = "Tu cuenta ha sido bloqueada. Contacta al administrador.";
                return Page();
            }

            ErrorMessage = "Correo electrónico o contraseña incorrectos.";
            logger.LogWarning("Intento de inicio de sesión fallido para el usuario {Email}.", Input.Email);
            return Page();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al intentar iniciar sesión para el usuario {Email}.", Input.Email);
            ErrorMessage = "Ocurrió un error al intentar iniciar sesión. Por favor, inténtalo de nuevo.";
            return Page();
        }
    }
}