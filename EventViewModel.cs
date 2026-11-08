using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class EventViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public string Location { get; set; } = "";

    [Required]
    public double Distance { get; set; }

    [Required]
    public string EventType { get; set; } = "Run";
}