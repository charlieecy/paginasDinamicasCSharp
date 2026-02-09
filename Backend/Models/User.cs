using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class User : IdentityUser<long>
{
    public string name { get; set; } = string.Empty;
}