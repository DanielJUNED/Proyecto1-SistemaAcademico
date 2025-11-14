
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;  
using Microsoft.AspNetCore.Identity;

namespace SistemaAcademico.Data.Entities
{ 
    [Table("Usuarios")]
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Docente = new HashSet<Docente>();
        }
        public virtual ICollection<Docente> Docente { get; set; }
        /*public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenga en cuenta que authenticationType debe coincidir con el valor definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar reclamaciones de usuario personalizadas aquí
            return userIdentity;
        }*/
    }
}