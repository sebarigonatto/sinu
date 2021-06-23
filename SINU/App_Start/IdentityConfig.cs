using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SINU.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SINU
{
    //clase para que se pueda enviar un correo a un listado de correos
    public class messageMAil
    {
        public List<string> Correos { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Destinatario { get; set; }
        public string Modo { get; set; }
    }

    //public interface IIdentityMessageService
    //{
    //    public Task SendMessage(messageMAil message);
    //// define methods for other message types that you want to send
    //}
    public class EmailService
    {
        private static SINUEntities db = new SINUEntities();

        public static async Task<string> SendEmail(string iD_AspNetUser, string asunto, string html, List<string> mails,string modo)
        {
            messageMAil message = new messageMAil
            {
                Correos = mails,
                Subject = asunto,
                Body = html,
                Destinatario = iD_AspNetUser,
                Modo=modo
            };
            var result = await Task.Factory.StartNew(() => SendMail(message));
            return await Task.FromResult(result.Result);
        }

        private static Task<string> SendMail(messageMAil message)
        {
            //LEVANTO LOS DATOS DE LA TABLA CONFIGURACION

            try
            {
                #region formatter
                //string text = string.Format(message.Body);
                //string html = db.Configuracion.Find(4).ValorDato.ToString() + message.Body;
                //string html = db.Configuracion.FirstOrDefault(b=>b.NombreDato== "MailCuerpo1").ValorDato + message.Body;

                //html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);

                //ARMADO DEL MENSAJE
                MailMessage mensage = new MailMessage();
                string MailAplicacion = db.Configuracion.FirstOrDefault(b => b.NombreDato == "MailAplicacion").ValorDato;
                string MailAplicacionDisplay = db.Configuracion.FirstOrDefault(b => b.NombreDato == "MailAplicacionDisp").ValorDato;

                mensage.From = new MailAddress(MailAplicacion, MailAplicacionDisplay);
                message.Destinatario = message.Destinatario != "" ? db.AspNetUsers.FirstOrDefault(m => m.Id == message.Destinatario).Email : "";
                if (message.Destinatario != "")
                {
                    mensage.To.Add(new MailAddress(message.Destinatario));
                }
                else
                {
                    foreach (var item in message.Correos)
                    {
                        if (message.Modo == "Bcc")
                        {
                            mensage.Bcc.Add(new MailAddress(item));
                        }
                        else
                        {
                            mensage.To.Add(new MailAddress(item));
                        }
                       
                        //mensage.Bcc
                    }
                }

                BuscarAdjuntos(message, mensage);

                mensage.Subject = message.Subject;
                #endregion

                //alternativa de vista segun lo prefiera el cliente del destinaario gmail,outlook....
                //mensage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                //la ultima tiene prioridad de las dos alternativas
                mensage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.Body, null, MediaTypeNames.Text.Html));

                SmtpClient smtpClient = new SmtpClient(db.Configuracion.FirstOrDefault(b => b.NombreDato == "MailHost").ValorDato);
                //SmtpClient smtpClient = new SmtpClient("webmail.armada.mil.ar", Convert.ToInt32(25));
                NetworkCredential credentials = new NetworkCredential(db.Configuracion.FirstOrDefault(b => b.NombreDato == "MailCredential").ValorDato,
                                                                      db.Configuracion.FirstOrDefault(b => b.NombreDato == "MailPasswCredential").ValorDato,
                                                                      db.Configuracion.FirstOrDefault(b => b.NombreDato == "MailDominioCredential").ValorDato);

                //Red: (predeterminado)https://www.codeproject.com/Articles/66257/Sending-Mails-in-NET-Framework
                //El mensaje se envía a través de la red al servidor SMTP.
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false; //ver nuevo agregado por el jefe
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = false;
                //smtpClient.UseDefaultCredentials = false;
                //Ver BRINDA CERTIFICADOS DE SEGURIDAD - quitado por el jefe
                //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //SE ENVIA EL MAIL ATRAVEs DEL CLIENTE
                smtpClient.Send(mensage);
                smtpClient.Dispose();

                string result = "Correo Enviado";
                //si todo esta bien se devuelve un mensaje
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
                //if (Ex.HResult==-2146233088) //error de correo inaccesible tal vez xq esta mal escrito o la cuenta esta bloqueada
            }

        }

        /// <summary>PROCESO que Busca el nro de paso que se esta haciendo, ejemplos: (1) registracion (2) datos personales,
        /// que se encuentra en el subject del mensaje que fue pasado (deberia ser de otra forma pero aun no lo pensamos)
        /// Si existe un nro de paso entre parentesis se accede a buscar en la tabla de configuracion los registros
        /// con "MailAdjuntoy el nro de paso" como nombre del dato. Luego se recorre la lista y se va armando
        /// la direccion donde está el archivo y  se van agregando
        /// los nombres de los archivos como adjunto al objeto MailMensaje
        /// Por último si es que habia un nro de paso quita ese nro que esta entre parentesis.
        /// </summary>
        /// <param name="Mensaje"></param>
        /// <param name="MensajeCorreo"></param>
        private static void BuscarAdjuntos(messageMAil Mensaje, MailMessage MensajeCorreo)
        {
            #region Busco el nro de paso que se esta haciendo, ejemplos: (1) registracion (2) datos personales
            String paso;
            int pos = Mensaje.Subject.IndexOf("(");
            int pos2 = (pos == -1) ? -1 : Mensaje.Subject.IndexOf(")", pos);
            paso = (pos2 == -1) ? "" : (Mensaje.Subject.Substring(pos + 1, pos2 - pos - 1));

            #endregion

            if (paso != "")
            {
                //busco los adjuntos que le corresponden al paso en cuestion MailAdjunto?
                List<Configuracion> MailAdjunto = db.Configuracion.Where(b => b.NombreDato == ("MailAdjunto" + paso)).ToList();
                String ubiadjunto = "";

                //recorro la lista de adjuntos obtenida en la configuracion y lo voy atachando al mail
                foreach (var item in MailAdjunto)
                {
                    ubiadjunto = Path.Combine(HttpRuntime.AppDomainAppPath, "Documentacion\\" + item.ValorDato);
                    Attachment adjunto = new Attachment(ubiadjunto);
                    MensajeCorreo.Attachments.Add(adjunto);
                }

                //quito el codigo que indica el paso en el asunto para que el usuario no lo vea
                Mensaje.Subject = Mensaje.Subject.Remove(pos, pos2 - pos + 1);

            }
            return;
        }


    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Conecte el servicio SMS aquí para enviar un mensaje de texto.
            return Task.FromResult(0);
        }
    }

    // Configure el administrador de usuarios de aplicación que se usa en esta aplicación. UserManager se define en ASP.NET Identity y se usa en la aplicación.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            SINUEntities db = new SINUEntities();
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            //Ver Configure la lógica de validación de nombres de usuario
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            //Ver Configure la lógica de validación de contraseñas
            manager.PasswordValidator = new PasswordValidator
            {
                //Alvarez Ismael liberada la restriccion por fuera de la vista.
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            //Ver Configurar valores predeterminados para bloqueo de usuario
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(double.Parse(db.Configuracion.FirstOrDefault(b => b.NombreDato == "DefaultAccountLockoutTimeSpan").ValorDato));
            //Ver numero maximo de intento de inicio de sesion
            manager.MaxFailedAccessAttemptsBeforeLockout = int.Parse(db.Configuracion.FirstOrDefault(b => b.NombreDato == "MaxFailedAccessAttemptsBeforeLockout").ValorDato);

            //Ver Registre proveedores de autenticación en dos fases. Esta aplicación usa los pasos Teléfono y Correo electrónico para recibir un código para comprobar el usuario
            // Puede escribir su propio proveedor y conectarlo aquí.
            //manager.RegisterTwoFactorProvider("Código telefónico", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Su código de seguridad es {0}"
            //});
            //manager.RegisterTwoFactorProvider("Código de correo electrónico", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "Código de seguridad",
            //    BodyFormat = "Su código de seguridad es {0}"
            //});
            //manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            var passTokenProvider = options.DataProtectionProvider;

            //ver Hora de duracion del token para la  confirmacion de mail
            double horavidaToken =double.Parse(db.Configuracion.FirstOrDefault(b => b.NombreDato == "TokenConfMailVida").ValorDato);

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        //ver define tiempo de duracon del token.
                        TokenLifespan = TimeSpan.FromHours(horavidaToken)
                    };
            }

            return manager;
        }
    }

    // Configure el administrador de inicios de sesión que se usa en esta aplicación.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
