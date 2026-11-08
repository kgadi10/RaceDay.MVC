using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class CategoryViewModel
{
    [Required]
    public string Name { get; set; } = "";

    public int EventId { get; set; }
}