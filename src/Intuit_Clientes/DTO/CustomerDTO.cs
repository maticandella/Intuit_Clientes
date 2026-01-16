namespace Intuit_Clientes.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? RazonSocial { get; set; }
        public string? CUIT { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? TelefonoCelular { get; set; }
        public string? Email { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
