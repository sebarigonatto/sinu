using System;

namespace SINU.ViewModels
{
   
    public class PlantillaMail {
        public string Apellido { get; set; }

    }
    public class PlantillaMailConfirmacion : PlantillaMail
    {
        public string LinkConfirmacion { get; set; }
    }
    public class PlantillaMailCuenta : PlantillaMail
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string LinkConfirmacion { get; set; }
    }
    public class MailConfirmacionEntrevista : PlantillaMail
    {
        public Nullable<System.DateTime> FechaEntrevista { get; set; }
    }
    public class MailPostular : PlantillaMail
    {
        public string Estado { get; set; }
    }
    public class DatosResponsable
    {
        public string ResponsablePisoOfic { get; set; }
        public string ResponsableTelefonoEinterno { get; set; }
        public string ResponsableMail { get; set; }
        public string Apellido { get; set; }

        //ver agregar direccion
        //string ResponsableNombreEdificio { get; set; }
        //string ResponsableCalleYnro { get; set; }
    }
}