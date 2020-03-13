using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SINU.Models;
using System.Collections.Generic;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(SINU.Startup))]
namespace SINU
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRoles();
        }
        private void createRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            
            SINUEntities db = new SINUEntities();
            
            List<vSeguridad_Grupos> perfiles = db.vSeguridad_Grupos.Where(m => m.codAplicacion == "SINU").ToList();

            foreach (vSeguridad_Grupos item in perfiles)
            {
                //Crea el Rol
                if (!roleManager.RoleExists(item.codGrupo))
                {
                    var role = new IdentityRole();
                    role.Name = item.codGrupo;
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
               if (!UserManager.IsInRole(user.Id,item.codGrupo))
                {
                    //role = roleManager.FindByName(item.codGrupo);
                    var result1 = UserManager.AddToRole(user.Id, item.codGrupo);

                }
            }

        }
    }
    
}
                    