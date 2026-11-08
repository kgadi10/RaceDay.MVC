using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class EnrolmentViewModel
{
    [Required]
    public int EventId { get; set; }

    [Required]
    public int CategoryId { get; set; }
}