using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SINU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Models;

namespace SINU.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private SINUEntities db = new SINUEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.logueado = false;
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.logueado = true;
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaMvc.Attributes.CaptchaVerify("Captcha no valido")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.logueado = false;
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.logueado = true;
                return View();
            }
            
            ViewBag.mensaje = "";
            if (!ModelState.IsValid)
            {
                AddErrors(IdentityResult.Failed("Respuesta de Capcha incorrecto"));
                return View(model);
            }
            ////validadndo si el email fue validado por el usuarios

            ApplicationUser usuario = await UserManager.FindByNameAsync(model.Email);
            if (usuario != null)
            {
                if (!usuario.EmailConfirmed)

                {
                    ViewBag.mensaje = "Correo no validado, revise su bandeja de entrada para validar su cuenta.";
                    //revisar la logica de si el token expiro
                    return View();
                }
                else
                {
                    // VER :cuenta los errores de inicio de sesión para el bloqueo de la cuenta
                    // Para permitir que los errores de contraseña desencadenen el bloqueo de la cuenta, cambie a shouldLockout: true
                    // shouldLockout: sirve para que se haga una cantidad indefinida de accesos fallidos false o true - dependiendo. 

                    //SignInManager: logica del logueo del usuario
                    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            //VER  redigir al controlador segun el tipo de usuario o CodGrupo ej /Postulante/index /Delegacion/index.. /Administracion/index ../Consultor/index
                            //tener en cuenta que si no exite el usuario registrado en la base de datos seguridad causara una excepcion 

                            if (returnUrl != null && (returnUrl.Contains("/Home/Index") || returnUrl != "/"))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                string Grupo = db.vSeguridad_Grupos_Usuarios.FirstOrDefault(sg => sg.codUsuario == model.Email).codGrupo.TrimEnd();
                                return RedirectToAction("Index", Grupo);

                            }
                        //el codigo anterior reemplaza al comentado                       
                        // return RedirectToAction("Index","Postulante");
                        case SignInStatus.LockedOut:
                            //ver cambio de pantalla de error
                            DateTime fin = ((DateTime)UserManager.FindByEmail(model.Email).LockoutEndDateUtc).ToLocalTime();
                            var min =  fin - DateTime.Now;
                           
                            var x = new System.Web.Mvc.HandleErrorInfo(new Exception("Inténtelo de nuevo en " + min.Minutes + " minutos."), "Account", "Login");
                            ViewBag.Title = "Cuenta Bloqueada";

                            return View("Lockout", x);
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "E-mail o Contraseña incorrecto.");
                            return View(model);
                    }
                }
            }
            else
            {
                model.Password = "";
                return RedirectToAction("Register");
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Requerir que el usuario haya iniciado sesión con nombre de usuario y contraseña o inicio de sesión externo
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // El código siguiente protege de los ataques por fuerza bruta a los códigos de dos factores. 
            // Si un usuario introduce códigos incorrectos durante un intervalo especificado de tiempo, la cuenta del usuario 
            // se bloqueará durante un período de tiempo especificado. 
            // Puede configurar el bloqueo de la cuenta en IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Código no válido.");
                    return View(model);
            }
        }

        
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(int? idInstitucion)
        {
          
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            //si es null lo estableco 1="Necesito Orientacion"
            idInstitucion??=1;
            ViewBag.Inst = db.Institucion.Find(idInstitucion).Titulo.ToString() + "  <br/>" + db.Institucion.Find(idInstitucion).NombreInst.ToString();

            //verifico si la convocatoria a la que quiere inscribir esta abierta
            var hoy = DateTime.Now.Date;
            var periodo = db.vPeriodosInscrip.FirstOrDefault(m=>m.IdInstitucion==idInstitucion);
            //si no hay ninguna convocatoria abierta y el instituto es distinto a 1
            if(periodo == null && idInstitucion!=1)
            {
                return View("ConvocatoriaNoAbierta");
            }

            RegisterViewModel regi = new RegisterViewModel
            {   //creando la lista para la vista register lista de las oficinas de ingreso y delegaciones y de las institucions ccon periodos disponibles
                ListOficinaYDelegacion = new SelectList(db.OficinasYDelegaciones.ToList(), "IdOficinasYDelegaciones", "NOmbre"),
                ListIntitutos = new SelectList(db.vPeriodosInscrip.DistinctBy(mbox=>mbox.IdInstitucion).OrderBy(m => m.IdInstitucion).ToList(), "IdInstitucion", "NombreInst"),
                IdInstituto = (int)idInstitucion
            };
        
            var DatosDelegacion2 = new List<Array>();
            db.OficinasYDelegaciones.ToList().ForEach(m => DatosDelegacion2.Add(new object[] { m.IdOficinasYDelegaciones,
                                                                                               m.Provincia + ", " + m.Localidad + ", " + m.Direccion,
                                                                                               m.Telefono,
                                                                                               m.Celular}));
            regi.DatosDelegacion = JsonConvert.SerializeObject(DatosDelegacion2);

            
            return View(regi);
        }

        
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            //el objeto model ya tiene la preferencia del instituto = model.IdInstituto , model.idOficinaDelegacion la mas cercana a su domicilio            
            model.ListOficinaYDelegacion = new SelectList(db.OficinasYDelegaciones.ToList(), "IdOficinasYDelegaciones", "NOmbre");
            model.ListIntitutos = new SelectList(db.vPeriodosInscrip.DistinctBy(mbox => mbox.IdInstitucion).OrderBy(m => m.IdInstitucion).ToList(), "IdInstitucion", "NombreInst");
            var DatosDelegacion2 = new List<Array>();
            db.OficinasYDelegaciones.ToList().ForEach(m => DatosDelegacion2.Add(new object[] { m.IdOficinasYDelegaciones,
                                                                                               m.Provincia + ", " + m.Localidad + ", " + m.Direccion,
                                                                                               m.Telefono,
                                                                                               m.Celular}));

            model.DatosDelegacion = JsonConvert.SerializeObject(DatosDelegacion2);
            
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                    sp_InvestigaDNI_Result SituacionPersona = db.sp_InvestigaDNI(model.DNI).First();
                    
                    //Si el DNI corresponde a un postulante anucio lo mismo en la vista de registro
                    if ((bool)SituacionPersona.ES_Postulante)
                    {
                        //debo verifsicar si se trata de un postulante o simplemente una persona                        
                        AddErrors(IdentityResult.Failed("El Dni ingresado corresponde a una cuenta existente."));
                        return View(model);
                        
                    }

                    //db.AspNetUsers.Remove(db.AspNetUsers.First(m => m.Email == model.Email));
                    //db.SaveChanges();
                    //creo la cuenta con el mail proporcionado
                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                                              
                        //crea una persona, un postulante y una inscripcion
                        var r = db.spCreaPostulante(false,SituacionPersona.IdPersona,model.Apellido, model.Nombre, model.DNI, model.Email, model.IdInstituto, model.idOficinaYDelegacion);
                      
                        //comentado para evitar el inicio de session automatico
                        //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                        //se genera el codigo para lueg oconfirma el email.
                        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);

                        //ENVIO de COREO con plantilla razor (*.cshtml) https://github.com/Antaris/RazorEngine
                        var modelPlantilla = new ViewModels.PlantillaMailConfirmacion
                        {
                            Apellido = model.Apellido,
                            LinkConfirmacion = callbackUrl,
                            CuerpoMail= db.Configuracion.FirstOrDefault(m=>m.NombreDato== "CuerpoMailRegistro").ValorDato.ToString()
                        };

                        //la funcio para enviar devuelve un booleano, 
                        //necesita 4 paramtros:
                        //ViewModel, con la que se cargaran los datos en la Plantilla del mail
                        //Nombre de la Plantilla a Utilizar
                        //Id de la Persona para obtener el correo de destino
                        //Asusnto de Mail.
                        string MODALIDAD = db.vConvocatoriaDetalles.FirstOrDefault(m => m.IdInstitucion == model.IdInstituto).IdModalidad;
                        if (MODALIDAD == "CPESNM" || MODALIDAD == "CPESSA" || MODALIDAD == "CUIM")
                        {
                            MODALIDAD = "CPESNM-CPESSA";
                        }
                        var resultado = Func.EnvioDeMail(modelPlantilla, "PlantillaMailConfirmacion", user.Id, null, "MailAsunto" + MODALIDAD,null,null);


                        ////Segun si el correo fue enviado o no se lanza un mensaje
                        //if (resultado.Contains("Correo Enviado"))
                        //{
                            ViewBag.mensaje = "Registro completado exitosamente, se le enviara un correo para validar su cuenta. Recuerde hacerlo dentro de las 24HS.";
                            return View("Login");
                        //}
                        //else
                        //{
                        //    ViewBag.mensaje = "Registro completado exitosamente, se le enviara un correo para validar su cuenta. Recuerde hacerlo dentro de las 24HS.";
                        //    return View("Login");
                        //}

                        
                    }
                    AddErrors(IdentityResult.Failed(result.Errors.ToList()[1]));
                }
                catch (Exception ex)// esto es una prueba ..quiero provocar un error y que venga por aca si falla el mail
                {
                    //HttpContext.Session["funcion"] = ex.Message; //no se debe usar session hay que crear el System.Web.Mvc.HandleErrorInfo

                    db.AspNetUsers.Remove(db.AspNetUsers.First(m => m.Email == model.Email));
                    db.SaveChanges();
                    return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Account", "Register"));
                }

            }
            //db.AspNetUsers.Remove(db.AspNetUsers.First(m => m.Email == model.Email));
            //db.SaveChanges();

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                if (userId == null || code == null)
                {
                    //Revisar no usamos return View("Error"); y usamos una pantalla de error generalizada
                    var x = new System.Web.Mvc.HandleErrorInfo(new Exception("Usuario inexistente o tiempo de confirmación expirado. Intente la registrarse nuevamente."), "Account", "Confirmacion de mail");
                    return View("Lockout", x);
                }
                //cuando se quiere confiramr mail de un usuario eliminado
                if (UserManager.FindById(userId) == null)
                {
                    var x = new System.Web.Mvc.HandleErrorInfo(new Exception("Tiempo de confirmación expirado. Inscribase nuevamente."), "Account", "Confirmacion de mail");
                    ViewBag.Title = "Confirmación de Cuenta";
                    return View("Lockout", x);
                }
                //verifico que el usuario ya alla confirmado

                var user = UserManager.FindById(userId);
                var Email = user.UserName;
                var persona = db.Persona.FirstOrDefault(m => m.Email == Email);

                ViewBag.YAconfirmo = user.EmailConfirmed;
                if (ViewBag.YAconfirmo)
                {
                    return View();
                }

                var result = await UserManager.ConfirmEmailAsync(userId, code);
                if (result.Succeeded)
                {
                   
                    //Ver aqui de colocar a esta persona si el result succeeded en la seguridad como POSTULANTE y se pone en etapa 5(REGISTRO Validado siguiente  6).
                    var r = db.spIngresaASeguridad(Email, "Postulante", "", "", "", "", "");
                    

                    Task<IdentityResult> resultado = UserManager.AddToRoleAsync(userId, "Postulante");

                    //siendo exitoso el ingreso a seguridad del postulante hago que avance a la secuencia siguiente : "DATOS BASICOS-Inicio De Carga"
                    db.spProximaSecuenciaEtapaEstado(persona.IdPersona, 0, false, 0, "DATOS BASICOS", "Inicio De Carga");

                    //envio mail a todos los usuarios de la delegaacion coorespondiete al postulante ye valido el correo
                    int ID_Delegacion = (int)db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == persona.IdPersona).IdDelegacionOficinaIngresoInscribio;
                    
                    ViewModels.ValidoCorreoPostulante datosMail = new ViewModels.ValidoCorreoPostulante
                    {
                        Apellido="",
                        Apellido_P = persona.Apellido,
                        Dni_P = persona.DNI,
                        IdInscripcion_P = persona.Postulante.Inscripcion.First().IdInscripcion,
                        Nombre_P = persona.Nombres,
                        url = Url.Action("Index", "Postulante", new { ID_Postulante= persona.IdPersona }, protocol: Request.Url.Scheme),
                        Delegacion = db.OficinasYDelegaciones.Find(ID_Delegacion).Nombre
                    };

                    Func.EnvioDeMail(datosMail, "PlantillaConfirmoCorreoPostulante", null, null, "MailAsunto6", ID_Delegacion, null);
                }
                else //Revisar (result.Succeeded == false) el booleano nunca se compara!!
                {
                    //Revisar En vez de usar Session["funcion"] = "Expiro token para la confirmacio de correo!!"; se debe generar un objeto que es usado por la view como modelo.
                    var x = new System.Web.Mvc.HandleErrorInfo(new Exception("Expiro el Token para la confirmación de correo. Intente registrarse nuevamente."), "Account", "Confirmacion de mail");
                    ViewBag.Title = "Token Vencido";
                    //ejecutar el script de eliminacion de cuenta con el token vencido
                    db.spAspNetUserYPostulanteEliminar(Email);

                    return View("Lockout", x);
                };

                return View(); //(result.Succeeded ? "ConfirmEmail" : "Error");
            }
            catch (Exception ex)
            {

                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Account", "Confirmacion de mail"));
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewBag.Title = "Cambio de Contraseña";
            ViewBag.Parrafo = "Se envio un mail a su correo, por favor  verifique el mismo para continuar con el proceso de cambio de contraseña.";
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ForgotPasswordConfirmation");
                }

                // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                //Revisar GUARDO LA FECHA Y HORA DE GENERACION DE TOKEN PARA EL USUARIO EN LA TABLA ASPUSER
                user.FechaToken = DateTime.Now;
                UserManager.Update(user);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);

                //busco los datos del usuario en las tabla que corresponda
                var postu = db.Persona.FirstOrDefault(mbox => mbox.Email == user.Email);
                //verifico si el usuario es un postulante o usuario administativo
                var apellido = (postu!=null) ? postu.Apellido:db.vUsuariosAdministrativos.FirstOrDefault(mbox => mbox.Email == user.Email).Apellido;

                ViewModels.PlantillaMailConfirmacion datosMail = new ViewModels.PlantillaMailConfirmacion
                {
                    Apellido = apellido,
                    CuerpoMail = "Se inicio el Proceso para el reestablecimiento de la contraseña, desde la opciones de Inicio de Sesion.",
                    LinkConfirmacion = callbackUrl
                };

                Func.EnvioDeMail(datosMail, "PlantillaMailForgotPassword", user.Id, null, "MailAsunto6", null,null);

              
                return View("ForgotPasswordConfirmation");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string userId)
        {
            if (code == null) return View("Error");

            double horaexpiToken = double.Parse(db.Configuracion.FirstOrDefault(b => b.NombreDato == "TokenCambioContrVida").ValorDato);
            //levanto la fecha/hora de creacion del token de reestablecimiento de contraseña
            var user = UserManager.FindById(userId);
            DateTime FechaGeneToken = DateTime.Parse(user.FechaToken.ToString());
            var horatrascurridas = (DateTime.Now - FechaGeneToken).TotalHours;
          
            if (horatrascurridas <= horaexpiToken && UserManager.VerifyUserToken(user.Id,"ResetPassword",code))
            {
                return View();
            }
            var x = new System.Web.Mvc.HandleErrorInfo(new Exception("Expiro el Token para el cambio de contraseña. Intentelo nuevamente."), "Account", "Confirmacion de mail");
            //revisar logica para cuando expiro el token de reestablesimiento de contraseña
            return View("Lockout", x);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                ViewBag.Title = "Cambio de Contraseña";
                ViewBag.Parrafo = "Se realizo el cambio de Contraseña exitosamente.";
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Solicitar redireccionamiento al proveedor de inicio de sesión externo
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generar el token y enviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Si el usuario ya tiene un inicio de sesión, iniciar sesión del usuario con este proveedor de inicio de sesión externo
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Si el usuario no tiene ninguna cuenta, solicitar que cree una
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtener datos del usuario del proveedor de inicio de sesión externo
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult RecuperarCuenta()
        {
                                                                                                                                                        
            return View();
        }


        //accion para cambia el correo de una cuenta
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarCuenta(RecuperarCuenta recu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = UserManager.FindByEmail(recu.Email);
                    ViewBag.Title = "Recuperacion De Cuenta";
                    ViewBag.Parrafo = "Se envio un mail a su correo, por favor  verifique el mismo para continuar con la recuperacion de la cuenta.";
                    if (user == null || !( UserManager.IsEmailConfirmed(user.Id)))
                    {
                        // No revelar que el usuario no existe o que no está confirmado
                        return View("ForgotPasswordConfirmation");

                    }

                    //genero el token 
                    string code = UserManager.GeneratePasswordResetToken(user.Id);
                    //Revisar GUARDO LA FECHA Y HORA DE GENERACION DE TOKEN PARA EL USUARIO EN LA TABLA ASPUSER

                    user.FechaToken = DateTime.Now;
                    UserManager.Update(user);

                    var callbackUrl = Url.Action("RecuperacionCuenta", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);

                    //busco los datos del usuario en las tabla que corresponda
                    var postu = db.Persona.FirstOrDefault(mbox => mbox.Email == user.Email);
                    //verifico si el usuario es un postulante o usuario administativo
                    var apellido = (postu != null) ? postu.Apellido : db.vUsuariosAdministrativos.FirstOrDefault(mbox => mbox.Email == user.Email).Apellido;

                    ViewModels.PlantillaMailConfirmacion datosMail = new ViewModels.PlantillaMailConfirmacion
                    {
                        Apellido = apellido,
                        CuerpoMail = "Se inicio el Proceso para la Recuperacion de la Cuenta, desde la opciones de Inicio de Sesion.",
                        LinkConfirmacion = callbackUrl
                    };

                    Func.EnvioDeMail(datosMail, "PlantillaMailForgotPassword", user.Id, null, "MailAsunto6", null,null);

                    return View("ForgotPasswordConfirmation");

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return View(recu);
        }

        [AllowAnonymous]
        public ActionResult RecuperacionCuenta(string code, string userId)
        {
            if (code == null) return View("Error");

            double horaexpiToken = double.Parse(db.Configuracion.FirstOrDefault(b => b.NombreDato == "TokenCambioContrVida").ValorDato);
            //levanto la fecha/hora de creacion del token de reestablecimiento de contraseña
            var user = UserManager.FindById(userId);
            DateTime FechaGeneToken = DateTime.Parse(user.FechaToken.ToString());
            var horatrascurridas = (DateTime.Now - FechaGeneToken).TotalHours;

            if (horatrascurridas <= horaexpiToken && UserManager.VerifyUserToken(user.Id, "ResetPassword", code))
            {
                return View();
            }

            var x = new System.Web.Mvc.HandleErrorInfo(new Exception("Expiro el Token para realizar la recuperacion de la cuenta. Intentelo nuevamente."), "Account", "Confirmacion de mail");
            //revisar logica para cuando expiro el token de reestablesimiento de contraseña
            return View("Lockout", x);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaMvc.Attributes.CaptchaVerify("Captcha is not valid")]
        public ActionResult RecuperacionCuenta(RecuperacionCuenta model)
        {
            if (!ModelState.IsValid)
            {
                AddErrors(IdentityResult.Failed("Respuesta de Capcha incorrecto"));
                return View(model);
            }
            //ver su buscar nuevamente el user con el nuevo mail 
            var user =  UserManager.FindByEmail(model.EmailOriginal);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var passcuenta = UserManager.CheckPassword(user, model.PasswordOriginal);
            if (!passcuenta)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            //ejecuto algunsp que cambie los mail de una cuenta

            //var result =  UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            //if (result.Succeeded)
            //{
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}

            ViewBag.Title = "Cambio de Email de la Cuenta";
            ViewBag.Parrafo = "Se realizo el cambio de E-mail de la Cuenta exitosamente.";

            return RedirectToAction("ResetPasswordConfirmation", "Account");


        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}