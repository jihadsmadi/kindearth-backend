namespace Application.DTOs.Tags
{
    /// <summary>
    /// Gender for the tag. 0=Unisex, 1=Men, 2=Women, 3=Boys, 4=Girls
    /// </summary>
    public record CreateTagRequest(string Name);
} 