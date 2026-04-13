using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlashcardApplicationServer.Models;

public class Flashcard
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Error: Term is required")]
    [StringLength(500, MinimumLength = 1)]
    public string? Term { get; set; }

    [Required(ErrorMessage = "Error: Definition is required")]
    [StringLength(500, MinimumLength = 1)]
    public string? Definition { get; set; }

    public int SetId { get; set; }

    [JsonIgnore]
    public FlashcardSet? FlashcardSet { get; set; }
}
