using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAcademico.Data.Constantes
{
    // Enumeraciones para estandarizar acciones y módulos
    public static class AccionesBitacora
    {
        public const string Crear = "Crear";
        public const string Editar = "Editar";
        public const string Eliminar = "Eliminar";
        public const string Login = "Login";
        public const string Logout = "Logout";
        public const string LoginFallido = "LoginFallido";
        public const string Consultar = "Consultar";
    }

    public static class ModulosBitacora
    {
        public const string CursoCuatrimestre = "CursoCuatrimestre";
        public const string CursoCuatrimestreDocente = "CursoCuatrimestreDocente";
        public const string Curso = "Curso";
        public const string Cuatrimestre = "Cuatrimestre";
        public const string Docente = "Docente";
        public const string Estudiante = "Estudiante";
        public const string Usuario = "Usuario";
        public const string Evaluacion = "Evaluacion";
        public const string Autenticacion = "Autenticacion";
    }
}
