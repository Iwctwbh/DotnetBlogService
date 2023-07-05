namespace DotnetBlogService.Models;

public partial class TblPost
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public ulong IsActive { get; set; }
}
