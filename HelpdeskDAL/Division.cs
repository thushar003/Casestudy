using System;
using System.Collections.Generic;

namespace ExercisesDAL;

public partial class Division
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public byte[] Timer { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
