using Intuit_Clientes.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Intuit_Clientes.Data.Configurations
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("clientes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Apellido)
                .HasColumnName("apellido")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.RazonSocial)
                .HasColumnName("razon_social")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.CUIT)
                .HasColumnName("cuit")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.FechaNacimiento)
                .HasColumnName("fecha_nacimiento")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(c => c.TelefonoCelular)
                .HasColumnName("telefono_celular")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(c => c.Email)
                .HasColumnName("email")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.FechaModificacion)
                .HasColumnName("fecha_modificacion")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
