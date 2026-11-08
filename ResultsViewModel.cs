using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class ResultViewModel
{
    [Required]
    public int EnrolmentId { get; set; }

    [Required]
    public int Position { get; set; }

    [Required]
    public string FinishTime { get; set; } = "";
}