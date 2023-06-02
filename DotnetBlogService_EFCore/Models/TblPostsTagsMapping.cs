using System;
using System.Collections.Generic;

namespace DotnetBlogService_EFCore.Models;

public partial class TblPostsTagsMapping
{
    public int PostId { get; set; }

    public int TagId { get; set; }

    public ulong IsActive { get; set; }

    public virtual TblPost Post { get; set; } = null!;

    public virtual TblTag Tag { get; set; } = null!;
}
