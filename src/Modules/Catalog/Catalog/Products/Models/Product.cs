using System.Threading.Tasks;

namespace Catalog.Products.Models
{
    public class Product : Aggregate<Guid>
    {
        public string Name { get; private set; } = default!;
        public List<string> Category { get; private set; } = new();
        public string Description { get; private set; } = default!;
        public string ImageName { get; private set; } = default!;
        public decimal Price { get; private set; }

        public static Product Create(Guid id, string name, List<string> category, string description, string imageName, decimal price)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            var product = new Product
            {
                Id = id,
                Name = name,
                Category = category,
                Description = description,
                ImageName = imageName,
                Price = price
            };

            product.AddDomainEvent(new ProductCreatedEvent(product));

            return product;
        }

        public void Update(string name, List<string> category, string description, string imageName, decimal price)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            Name = name;
            Category = category;
            Description = description;
            ImageName = imageName;


            if (Price != price)
            {
                Price = price;
                AddDomainEvent(new ProductPriceChangedEvent(this));
            }
        }
    }
}
