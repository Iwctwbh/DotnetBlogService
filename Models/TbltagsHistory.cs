using System;
using System.Collections.Generic;

namespace DotnetBlogService.Models;

public partial class TbltagsHistory
{
    public int Id { get; set; }

    public int TagId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public string Action { get; set; } = null!;

    public ulong IsActive { get; set; }
}
