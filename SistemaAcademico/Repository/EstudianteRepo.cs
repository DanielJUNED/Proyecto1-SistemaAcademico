using SistemaAcademico.Models;
using SistemaAcademico.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks; 

namespace SistemaAcademico.Repository
{
    public class EstudianteRepo
    {
        private readonly ApplicationDbContext _db;
        public EstudianteRepo(ApplicationDbContext db)
        {
            _db = db;
        }
        // =============================================
        // REGISTRAR ESTUDIANTE
        // =============================================
        public ResultadoRegistro RegistrarEstudiante(EstudianteViewModel modelo)
        {
            var resultado = new ResultadoRegistro();

            // Iniciar transacción
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Verificar duplicados
                    if (ExisteIdentificacion(modelo.Identificacion))
                    {
                        resultado.Exitoso = false;
                        resultado.Mensaje = "Ya existe un estudiante con esta identificación";
                        return resultado;
                    }

                    if (ExisteEmail(modelo.Email))
                    {
                        resultado.Exitoso = false;
                        resultado.Mensaje = "Ya existe un estudiante con este correo electrónico";
                        return resultado;
                    }

                    // 2. Crear entidad Estudiante
                    var estudiante = new Estudiante
                    {
                        Identificacion = modelo.Identificacion.Trim(),
                        Nombre         = modelo.Nombre.Trim(),
                        Apellidos      = modelo.Apellidos.Trim(),
                        Fec_Nacimiento = modelo.FechaNacimiento,
                        Email          = modelo.Email.Trim().ToLower(),
                        DistritoId     = modelo.DistritoID,
                        Fec_Registro   = DateTime.Now,
                        Ind_Estado     = "A" 
                    };

                    // 3. Agregar estudiante al contexto
                    _db.Estudiante.Add(estudiante);
                    _db.SaveChanges();

                    // 4. Obtener IDs de CursoCuatrimestre y crear matrículas
                    foreach (var cursoId in modelo.CursosSeleccionados)
                    {
                        // Buscar el CursoCuatrimestre correspondiente
                        var cursoCuatrimestre = _db.CursoCuatrimestre
                            .FirstOrDefault(cc =>
                                cc.CursoId == cursoId &&
                                cc.CuatrimestreId == modelo.CuatrimestreID &&
                                cc.Ind_Estado == "A");

                        if (cursoCuatrimestre == null)
                        {
                            throw new Exception($"No se encontró el curso {cursoId} en el cuatrimestre {modelo.CuatrimestreID}");
                        }

                        // Crear matrícula
                        var matricula = new EstudianteCurso
                        {
                            EstudianteId = estudiante.EstudianteId,
                            CursoCuatrimestreId = cursoCuatrimestre.CursoCuatrimestreId,
                            Fec_Matricula = DateTime.Now,
                            Fec_Registro = DateTime.Now,
                            Ind_Estado = "A"
                        };

                        _db.EstudianteCurso.Add(matricula);
                    }

                    // 5. Guardar matrículas
                    _db.SaveChanges();

                    // 6. Confirmar transacción
                    transaction.Commit();

                    resultado.Exitoso = true;
                    resultado.Mensaje = "Estudiante registrado exitosamente";
                    resultado.PersonaID = estudiante.EstudianteId;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    resultado.Exitoso = false;
                    resultado.Mensaje = "Error al guardar en la base de datos";
                    var detalle = ex.InnerException?.InnerException?.Message
                                 ?? ex.InnerException?.Message
                                 ?? ex.Message;
                    resultado.Errores.Add(detalle);
                    //resultado.Errores.Add(ex.InnerException?.Message ?? ex.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    resultado.Exitoso = false;
                    resultado.Mensaje = "Error al registrar el estudiante";
                    resultado.Errores.Add(ex.Message);
                }
                 
            }

            return resultado;
        }

        // =============================================
        // VERIFICACIONES DE DUPLICADOS
        // =============================================
        public bool ExisteIdentificacion(string identificacion)
        {
            return _db.Estudiante
                .Any(e => e.Identificacion == identificacion);
        }

        public bool ExisteEmail(string email)
        {
            var emailNormalizado = email.Trim().ToLower();
            return  _db.Estudiante.Any(e => e.Email.ToLower() == emailNormalizado);
        }

        // =============================================
        // OBTENER UBICACIÓN GEOGRÁFICA
        // =============================================
        public IEnumerable<Provincia> ObtProvincias()
        {
            return _db.Provincia
                .OrderBy(p => p.Nom_Provincia)
                .ToList();
        }

        public IEnumerable<Canton> ObtCantonesPorProvincia(int provinciaId)
        {
            return _db.Canton
                .Where(c => c.ProvinciaId == provinciaId)
                .OrderBy(c => c.Nom_Canton)
                .ToList();
        }

        public IEnumerable<Distrito> ObtDistritosPorCanton(int cantonId)
        {
            return _db.Distrito
                .Where(d => d.CantonId == cantonId)
                .OrderBy(d => d.Nom_Distrito)
                .ToList();
        }

        // =============================================
        // OBTENER INFORMACIÓN ACADÉMICA
        // =============================================
        public IEnumerable<Cuatrimestre> ObtCuatrimestresActivos()
        {
            return _db.Cuatrimestre
                .Where(c => c.Ind_Estado == "A")
                .OrderByDescending(c => c.Anio)
                .ThenByDescending(c => c.Numero)
                .ToList();
        }

        public IEnumerable<Curso> ObtCursosPorCuatrimestre(int cuatrimestreId)
        {
            return _db.Curso
                .Where(c => c.Ind_Estado=="A" &&
                    c.CursoCuatrimestre.Any(cc =>
                        cc.CuatrimestreId == cuatrimestreId &&
                        cc.Ind_Estado == "A"))
                .OrderBy(c => c.Codigo)
                .ToList();
        }
        /// <summary>
        /// Obtiene un estudiante con su información completa (incluye ubicación)
        /// </summary>
        public  Estudiante ObtenerEstudianteCompleto( int estudianteId)
        {
            return _db.Estudiante
                .Include("Distrito.Canton.Provincia")
                .Include("EstudianteCursos.CursoCuatrimestre.Curso")
                .Include("EstudianteCursos.CursoCuatrimestre.Cuatrimestre")
                .FirstOrDefault(e => e.EstudianteId == estudianteId);
        }

        /// <summary>
        /// Busca estudiantes por nombre, apellido o identificación
        /// </summary>
        public  Estudiante BuscarEstudiantes( string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
                return new Estudiante();

            var criterioBusqueda = criterio.Trim().ToLower();

            return _db.Estudiante.Where(e =>
                e.Nombre.ToLower().Contains(criterioBusqueda) ||
                e.Apellidos.ToLower().Contains(criterioBusqueda) ||
                e.Identificacion.Contains(criterioBusqueda) ||
                e.Email.ToLower().Contains(criterioBusqueda)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los cursos matriculados de un estudiante en un cuatrimestre
        /// </summary>
        public List<Curso> ObtCursosMatriculados( int estudianteId, int cuatrimestreId)
        {
            return _db.EstudianteCurso
                .Where(ec =>
                    ec.EstudianteId == estudianteId &&
                    ec.CursoCuatrimestre.CuatrimestreId == cuatrimestreId &&
                    ec.Ind_Estado == "A")
                .Select(ec => ec.CursoCuatrimestre.Curso)
                .ToList();
        }

        /// <summary>
        /// Obtiene estadísticas de un cuatrimestre
        /// </summary>
        public object ObtEstadisticasCuatrimestreAsync(  int cuatrimestreId)
        {
            var stats = _db.EstudianteCurso
                .Where(ec => ec.CursoCuatrimestre.CuatrimestreId == cuatrimestreId)
                .GroupBy(ec => 1)
                .Select(g => new
                {
                    TotalMatriculas = g.Count(),
                    EstudiantesUnicos = g.Select(ec => ec.EstudianteId).Distinct().Count(),
                    CursosOfrecidos = g.Select(ec => ec.CursoCuatrimestre.CursoId).Distinct().Count()
                })
                .FirstOrDefault();

            return stats ?? new { TotalMatriculas = 0, EstudiantesUnicos = 0, CursosOfrecidos = 0 };
        }
    }
}