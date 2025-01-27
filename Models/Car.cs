using System;
using System.Collections.Generic;

namespace ParkeringsApp.Models;

public partial class Car
{
    public int CarId { get; set; }

    public string PlateNumber { get; set; } = null!;

    public string? Model { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public virtual User User { get; set; } = null!;
}
