namespace e_commerce_web_api.DTOs
{
    public class CategoryReadDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}