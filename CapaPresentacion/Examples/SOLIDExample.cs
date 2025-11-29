using CapaEntidad;
using CapaEntidad.Interfaces;
using CapaNegocio.Containers;
using CapaNegocio.Servicios;
using CapaNegocio.Validaciones;
using CapaNegocio.Validaciones.Strategies;
using CapaDatos.Implementaciones;
using System;
using System.Collections.Generic;

namespace CapaPresentacion.Examples
{
    /// <summary>
    /// Ejemplo completo que demuestra la implementación de los principios SOLID
    /// </summary>
    public class SOLIDExample
    {
        public static void DemostrarPrincipiosSOLID()
        {
            Console.WriteLine("=== DEMOSTRACIÓN DE PRINCIPIOS SOLID ===\n");

            // 1. SRP (Single Responsibility Principle)
            DemostrarSRP();

            // 2. OCP (Open/Closed Principle)
            DemostrarOCP();

            // 3. DIP (Dependency Inversion Principle)
            DemostrarDIP();

            Console.WriteLine("=== FIN DE LA DEMOSTRACIÓN ===");
        }

        /// <summary>
        /// Demuestra el principio de Responsabilidad Única (SRP)
        /// Cada clase tiene una sola razón para cambiar
        /// </summary>
        private static void DemostrarSRP()
        {
            Console.WriteLine("1. PRINCIPIO DE RESPONSABILIDAD ÚNICA (SRP)");
            Console.WriteLine("=============================================");

            // Cada clase tiene una responsabilidad específica:
            // - ConexionSql: Solo maneja la conexión a la base de datos
            // - ValidacionProducto: Solo valida productos
            // - GeneradorCodigoProducto: Solo genera códigos
            // - ExportadorDatos: Solo exporta datos

            var conexion = new ConexionSql();
            var validacion = new ValidacionProducto();
            var repositorio = new RepositorioProducto(conexion);
            var servicio = new ServicioProducto(repositorio, validacion);
            var generador = new GeneradorCodigoProducto(servicio);
            var exportador = new ExportadorDatos(servicio);

            Console.WriteLine("✓ Cada clase tiene una responsabilidad específica");
            Console.WriteLine("✓ ConexionSql: Solo maneja conexiones");
            Console.WriteLine("✓ ValidacionProducto: Solo valida productos");
            Console.WriteLine("✓ GeneradorCodigoProducto: Solo genera códigos");
            Console.WriteLine("✓ ExportadorDatos: Solo exporta datos\n");
        }

        /// <summary>
        /// Demuestra el principio Abierto/Cerrado (OCP)
        /// Las clases están abiertas para extensión pero cerradas para modificación
        /// </summary>
        private static void DemostrarOCP()
        {
            Console.WriteLine("2. PRINCIPIO ABIERTO/CERRADO (OCP)");
            Console.WriteLine("==================================");

            // Crear validación extensible
            var validacionExtensible = new ValidacionProductoExtensible();
            
            // Agregar estrategias de validación sin modificar la clase base
            validacionExtensible.AgregarEstrategia(new ValidacionProductoBasica());
            validacionExtensible.AgregarEstrategia(new ValidacionProductoAvanzada());

            // Crear repositorio extensible
            var conexion = new ConexionSql();
            var repositorioExtensible = new RepositorioProductoExtensible(conexion);

            // Agregar extensiones sin modificar la clase base
            repositorioExtensible.AgregarExtension(new LoggingExtension());
            repositorioExtensible.AgregarExtension(new AuditExtension());

            Console.WriteLine("✓ ValidacionProductoExtensible: Extensible con estrategias");
            Console.WriteLine("✓ RepositorioProductoExtensible: Extensible con extensiones");
            Console.WriteLine("✓ Se pueden agregar nuevas funcionalidades sin modificar código existente\n");
        }

        /// <summary>
        /// Demuestra el principio de Inversión de Dependencias (DIP)
        /// Los módulos de alto nivel no dependen de módulos de bajo nivel
        /// </summary>
        private static void DemostrarDIP()
        {
            Console.WriteLine("3. PRINCIPIO DE INVERSIÓN DE DEPENDENCIAS (DIP)");
            Console.WriteLine("=================================================");

            // Configurar contenedor de dependencias
            var container = new SimpleDIContainer();
            
            // Registrar dependencias
            container.Registrar<IConexion, ConexionSql>(true);
            container.Registrar<IRepositorioProducto, RepositorioProducto>();
            container.Registrar<IValidacionProducto, ValidacionProducto>();
            container.Registrar<IServicioProducto, ServicioProducto>();

            // Resolver dependencias
            var servicio = container.Resolver<IServicioProducto>();

            Console.WriteLine("✓ Dependencias inyectadas a través de interfaces");
            Console.WriteLine("✓ Alto nivel (ServicioProducto) no depende de bajo nivel (RepositorioProducto)");
            Console.WriteLine("✓ Ambos dependen de abstracciones (interfaces)");
            Console.WriteLine("✓ Fácil intercambio de implementaciones\n");
        }
    }

    /// <summary>
    /// Extensión de ejemplo para logging (OCP)
    /// </summary>
    public class LoggingExtension : IRepositorioExtension<Producto>
    {
        public void AntesDeRegistrar(Producto obj)
        {
            Console.WriteLine($"[LOG] Intentando registrar producto: {obj.Nombre}");
        }

        public void DespuesDeRegistrar(Producto obj, int idGenerado)
        {
            Console.WriteLine($"[LOG] Producto registrado con ID: {idGenerado}");
        }

        public void AntesDeEditar(Producto obj)
        {
            Console.WriteLine($"[LOG] Intentando editar producto: {obj.Nombre}");
        }

        public void DespuesDeEditar(Producto obj, bool resultado)
        {
            Console.WriteLine($"[LOG] Producto editado: {(resultado ? "Exitoso" : "Fallido")}");
        }

        public void AntesDeEliminar(Producto obj)
        {
            Console.WriteLine($"[LOG] Intentando eliminar producto ID: {obj.IdProducto}");
        }

        public void DespuesDeEliminar(Producto obj, bool resultado)
        {
            Console.WriteLine($"[LOG] Producto eliminado: {(resultado ? "Exitoso" : "Fallido")}");
        }
    }

    /// <summary>
    /// Extensión de ejemplo para auditoría (OCP)
    /// </summary>
    public class AuditExtension : IRepositorioExtension<Producto>
    {
        public void AntesDeRegistrar(Producto obj)
        {
            Console.WriteLine($"[AUDIT] Registro de producto iniciado: {obj.Codigo}");
        }

        public void DespuesDeRegistrar(Producto obj, int idGenerado)
        {
            Console.WriteLine($"[AUDIT] Producto registrado en auditoría: {idGenerado}");
        }

        public void AntesDeEditar(Producto obj)
        {
            Console.WriteLine($"[AUDIT] Edición de producto iniciada: {obj.Codigo}");
        }

        public void DespuesDeEditar(Producto obj, bool resultado)
        {
            Console.WriteLine($"[AUDIT] Edición registrada en auditoría: {resultado}");
        }

        public void AntesDeEliminar(Producto obj)
        {
            Console.WriteLine($"[AUDIT] Eliminación de producto iniciada: {obj.IdProducto}");
        }

        public void DespuesDeEliminar(Producto obj, bool resultado)
        {
            Console.WriteLine($"[AUDIT] Eliminación registrada en auditoría: {resultado}");
        }
    }
}
