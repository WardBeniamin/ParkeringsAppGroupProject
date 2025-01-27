using System;
using System.Collections.Generic;

namespace ParkeringsApp.Models;

public partial class Receipt
{
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public int ZoneId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal Amount { get; set; }

    public int CarId { get; set; }

    public int PaymentId { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual PaymentMethod Payment { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Zone Zone { get; set; } = null!;
}
