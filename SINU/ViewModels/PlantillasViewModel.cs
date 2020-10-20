using System;

namespace SINU.ViewModels
{
   
    public class PlantillaMail {
        public string Apellido { get; set; }

    }
    public class PlantillaMailConfirmacion : PlantillaMail
    {
        public string CuerpoMail { get; set; }
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
        public string LinkConfirmacion { get; set; }
        public string MailCuerpo { get; set; }
        public bool Postulado { get; set; }
    }

    public class ValidoCorreoPostulante : PlantillaMail
    {
        public string Nombre_P { get; set; }
        public string Apellido_P { get; set; }
        public string Dni_P { get; set; }
        public int IdInscripcion_P { get; set; }
        public string Delegacion { get; set; }
        public string url { get; set; }
    }

    public class SolicitudEntreCorreoPostulante : PlantillaMail
    {
        public string Nombre_P { get; set; }
        public string Apellido_P { get; set; }
        public string Dni_P { get; set; }
        public int IdInscripcion_P { get; set; }
        public string Delegacion { get; set; }
        public string url { get; set; }
    }

    //public class xxx : PlantillaMail
    //{
    //    public string xxx { get; set; }
    //    public string xxx { get; set; }
    //}

    public class DatosResponsable
    {
        public string ResponsablePisoOfic { get; set; }
        public string ResponsableTelefonoEinterno { get; set; }
        public string ResponsableCelular { get; set; }
        public string ResponsableMail { get; set; }
        public string Apellido { get; set; }

        //ver agregar direccion
        //string ResponsableNombreEdificio { get; set; }
        //string ResponsableCalleYnro { get; set; }
    }
}