using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlashcardApplicationServer.Models;

public class FlashcardSet
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Error: Title is required")]
    [StringLength(100, MinimumLength = 1)]
    public string? Title { get; set; }
    
    public string? UserId { get; set; }

    [JsonIgnore]
    public IdentityUser? User { get; set; }

    public ICollection<Flashcard> Flashcards { get; set; } = [];
}
