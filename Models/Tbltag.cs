namespace DotnetBlogService.Models;

public partial class TblTag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public ulong IsActive { get; set; }
}
