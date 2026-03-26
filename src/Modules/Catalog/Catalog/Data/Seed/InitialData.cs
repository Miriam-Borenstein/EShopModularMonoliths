namespace Catalog.Data.Seed
{
    public static class InitialData
    {
        public static IEnumerable<Product> Products =>
            new List<Product>
            {
                Product.Create(new Guid(), "IPhone X",["Category 1"] , "Long description", "imagefile", 500),
                Product.Create(new Guid(), "Samsung 10", ["Category 1"], "Long description", "imagefile", 400 ),
                Product.Create(new Guid(), "Huawei Plus", ["Category 2"], "Long description", "imagefile", 650 ),
                Product.Create(new Guid(), "Xiaomi Mi", ["Category 2"], "Long description", "imagefile", 450 ),
                
            };
    }
}