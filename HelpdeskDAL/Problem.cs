/*
 * File: Problem.cs
 * @author: Thushar Joseph Joji, 1190586
 */

using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class Problem : HelpdeskEntity
{
    public string? Description { get; set; }

    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();
}
