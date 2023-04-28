using System;
using System.Collections.Generic;

namespace DotnetBlogService.Models;

public partial class Tbltag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public ulong IsActive { get; set; }
}
