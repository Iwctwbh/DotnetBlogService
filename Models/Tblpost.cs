using System;
using System.Collections.Generic;

namespace DotnetBlogService.Models;

public partial class TblPost
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public ulong IsActive { get; set; }
}
