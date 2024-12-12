/*
 * File: Department.cs
 * @author: Thushar Joseph Joji, 1190586
 */

using System;
using System.Collections.Generic;

namespace HelpdeskDAL;

public partial class Department : HelpdeskEntity
{
    public string? DepartmentName { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
