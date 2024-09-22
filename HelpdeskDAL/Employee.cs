﻿using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class Employee
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNo { get; set; }

    public string? Email { get; set; }

    public int DepartmentId { get; set; }

    public bool? IsTech { get; set; }

    public byte[]? StaffPicture { get; set; }

    public byte[] Timer { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;
}
