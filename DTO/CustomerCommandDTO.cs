using System.ComponentModel.DataAnnotations;

namespace Intuit_Entrevista.DTO
{
    public abstract class CustomerCommandDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "La razón social debe tener entre 2 y 150 caracteres")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El CUIT es obligatorio")]
        [RegularExpression(@"^\d{2}-\d{8}-\d{1}$", ErrorMessage = "El CUIT debe tener el formato XX-XXXXXXXX-X (ej: 20-12345678-9)")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El CUIT debe tener exactamente 13 caracteres incluyendo guiones")]
        public string CUIT { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El teléfono celular es obligatorio")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$|^\d{10,15}$", ErrorMessage = "El teléfono debe tener entre 10 y 15 dígitos")]
        [StringLength(30, MinimumLength = 10, ErrorMessage = "El teléfono debe tener entre 10 y 30 caracteres")]
        public string TelefonoCelular { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(150, ErrorMessage = "El email no puede exceder los 150 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; }
    }

    public class CustomerCreateDTO : CustomerCommandDTO
    {
    }

    public class CustomerUpdateDTO : CustomerCommandDTO
    {
    }
}
