namespace SugboGo.Models;

public sealed class DestinationPost
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string DestinationName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string ImageFileName { get; set; } = string.Empty;
    public int Likes { get; set; }
    public int Comments { get; set; }
    public List<PostComment> CommentsList { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
