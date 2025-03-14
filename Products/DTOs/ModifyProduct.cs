namespace ConsoleOrderExecutor.Products.DTOs
{
    public class ModifyProduct
    {
        public required int Id { get; set; }
        public string? Ean { get; set; }
        public string? Name { get; set; }
    }
}
