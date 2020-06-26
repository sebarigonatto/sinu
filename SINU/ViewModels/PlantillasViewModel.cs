using System;

namespace SINU.ViewModels
{
    public class PlantillaMail
    { }
    public class PlantillaMailConfirmacion : PlantillaMail
    {
        public string Apellido { get; set; }

        public string LinkConfirmacion { get; set; }
    }
    public class PlantillaMailCuenta : PlantillaMail
    {
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string LinkConfirmacion { get; set; }
    }
    public class MailConfirmacionEntrevista : PlantillaMail
    {
        public string Apellido { get; set; }
        public Nullable<System.DateTime> FechaEntrevista { get; set; }

    }
    public class MailPostular : PlantillaMail
    {
        public string Estado { get; set; }
        public string Apellido { get; set; }
    }
}