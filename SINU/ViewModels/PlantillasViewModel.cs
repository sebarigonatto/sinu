namespace SINU.ViewModels
{
    public class PLantillaMail
    { }
    public class PlantillaMailConfirmacion : PLantillaMail
    {
        public string Apellido { get; set; }

        public string LinkConfirmacion { get; set; }
    }
    public class PlantillaMailCuenta : PLantillaMail
    {
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string LinkConfirmacion { get; set; }
    }

}