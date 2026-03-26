namespace Ordering.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId);

        builder.HasIndex(x => x.OrderName).IsUnique();

        builder.Property(x => x.OrderName).IsRequired().HasMaxLength(100);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId);


        builder.ComplexProperty(o => o.ShippingAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.FirstName).IsRequired().HasMaxLength(50);
            addressBuilder.Property(a => a.LastName).IsRequired().HasMaxLength(50);
            addressBuilder.Property(a => a.EmailAddress).HasMaxLength(50);
            addressBuilder.Property(a => a.AddressLine).IsRequired().HasMaxLength(180);
            addressBuilder.Property(a => a.Country).HasMaxLength(50);
            addressBuilder.Property(a => a.State).HasMaxLength(50);
            addressBuilder.Property(a => a.ZipCode).IsRequired().HasMaxLength(5);
        });

        builder.ComplexProperty(o => o.BillingAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.FirstName).IsRequired().HasMaxLength(50);
            addressBuilder.Property(a => a.LastName).IsRequired().HasMaxLength(50);
            addressBuilder.Property(a => a.EmailAddress).HasMaxLength(50);
            addressBuilder.Property(a => a.AddressLine).IsRequired().HasMaxLength(180);
            addressBuilder.Property(a => a.Country).HasMaxLength(50);
            addressBuilder.Property(a => a.State).HasMaxLength(50);
            addressBuilder.Property(a => a.ZipCode).IsRequired().HasMaxLength(5);
        });


        builder.ComplexProperty(o => o.Payment, paymentBuilder =>
        {
            paymentBuilder.Property(p => p.CardName).HasMaxLength(50);
            paymentBuilder.Property(p => p.CardNumber).HasMaxLength(24).IsRequired();
            paymentBuilder.Property(p => p.Expiration).HasMaxLength(10);
            paymentBuilder.Property(p => p.CVV).HasMaxLength(3);
            paymentBuilder.Property(p => p.PaymentMethod);
        });
    }
}
