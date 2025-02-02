using System;
using System.Collections.Generic;

namespace ParkeringsApp.Models;

public partial class PaymentMethod
{
    public int PaymentId { get; set; }

    public string PaymentType { get; set; } = null!;

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

}
