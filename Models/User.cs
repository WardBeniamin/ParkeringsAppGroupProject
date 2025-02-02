﻿using System;
using System.Collections.Generic;

namespace ParkeringsApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Adress { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<ActiveParking> ActiveParkings { get; set; } = new List<ActiveParking>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public virtual ICollection<PaymentMethod> Payments { get; set; } = new List<PaymentMethod>();
}
