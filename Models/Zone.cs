using System;
using System.Collections.Generic;

namespace ParkeringsApp.Models;

public partial class Zone
{
    public int ZoneId { get; set; }

    public decimal Fee { get; set; }

    public string? Adress { get; set; }

    public virtual ICollection<ActiveParking> ActiveParkings { get; set; } = new List<ActiveParking>();

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
