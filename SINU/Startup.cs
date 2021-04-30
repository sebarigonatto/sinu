using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SINU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(SINU.Startup))]
namespace SINU
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Task.Factory.StartNew(() =>
            {
                CreateRoles();
            });
           
        }

     
        private void CreateRoles()
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            
                SINUEntities db = new SINUEntities();

                List<string> perfiles = db.vSeguridad_Grupos.Where(m => m.codAplicacion == "SINU").Select(m=>m.codGrupo.TrimEnd()).ToList();

                foreach(string item in perfiles)
                 {
                    //Crea el Rol
                    if (!roleManager.RoleExists(item))
                    {
                        var role = new IdentityRole();
                        role.Name = item;
                        roleManager.Create(role);

                    }
                }


                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                List<vSeguridad_Grupos_Usuarios> UsuarioYPerfiles = db.vSeguridad_Grupos_Usuarios.Where(m => m.codAplicacion == "SINU").ToList();
                ApplicationUser user;
                //IdentityRole role;

                foreach (var item in UsuarioYPerfiles)
                {
                    user = UserManager.FindByName(item.codUsuario);
                    if (!UserManager.IsInRole(user.Id, item.codGrupo.Trim()))
                    {
                        //role = roleManager.FindByName(item.codGrupo);
                        var result1 = UserManager.AddToRole(user.Id, item.codGrupo.Trim());

                    }
                }
            }
            catch (System.Exception ex)
            {

                Func.ConstruyeError(ex.InnerException.Message,"","");
            }
            

        }


    }
    
}
                    