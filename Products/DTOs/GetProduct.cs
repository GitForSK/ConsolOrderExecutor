namespace ConsoleOrderExecutor.Products.DTOs
{
    public class GetProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ean { get; set; } = string.Empty;
    }
}
