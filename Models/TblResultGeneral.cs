using System;
using System.Collections.Generic;

namespace DotnetBlogService.Models;

public partial class TblResultGeneral
{
    public int ErrorCode { get; set; }

    public string? ErrorDesc { get; set; }
}
