using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class Problem
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public byte[] Timer { get; set; } = null!;
}
