
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;

namespace SGE.Infraestructura;

public class TramiteTxtRepository: ITramiteRepository
{
    private readonly string _nombreArchivo = "Tramite.txt";
    public void AgregarTramite(Tramite Tram)
    {
    // Guardamos cada dato en una línea nueva.
    using var sw = new StreamWriter(_nombreArchivo, true);
    sw.WriteLine(Tram.Id);
    sw.WriteLine(Tram.ExpedienteId);
    sw.WriteLine(Tram.Contenido);
    sw.WriteLine(Tram.FechaCreacion);
    sw.WriteLine(Tram.FechaUltModificacion);
    sw.WriteLine(Tram.UsuarioUltModificacion);
    sw.WriteLine(Tram.Etiqueta);
    }

    public IEnumerable<Tramite> ListarTramitesPorExpediente(Guid expedienteId)
    {
        var resultado = new List<Tramite>();

        if (!File.Exists(_nombreArchivo))
            return resultado;

        using var sr = new StreamReader(_nombreArchivo);

        while (!sr.EndOfStream)
        {
            // Leemos exactamente las mismas líneas que escribimos en AgregarTramite
            var idStr                  = sr.ReadLine() ?? "";
            var expedienteIdStr        = sr.ReadLine() ?? "";
            var contenidoStr           = sr.ReadLine() ?? "";
            var fechaCreacionStr       = sr.ReadLine() ?? "";
            var fechaUltModificacionStr = sr.ReadLine() ?? "";
            var usuarioUltimoCambioStr = sr.ReadLine() ?? "";
            var etiquetaStr            = sr.ReadLine() ?? "";

            var id                  = Guid.Parse(idStr);
            var expId               = Guid.Parse(expedienteIdStr);
            var contenido           = new ContenidoTramite(contenidoStr);
            var fechaCreacion       = DateTime.Parse(fechaCreacionStr);
            var fechaUltModificacion = DateTime.Parse(fechaUltModificacionStr);
            var usuarioUltimoCambio = Guid.Parse(usuarioUltimoCambioStr);
            var etiqueta            = Enum.Parse<EtiquetaTramite>(etiquetaStr);

            var tramite = Tramite.ReconstruirTramite(
                id,
                expId,
                contenido,
                fechaCreacion,
                fechaUltModificacion,
                usuarioUltimoCambio,
                etiqueta
            );

            if (expId == expedienteId)
                resultado.Add(tramite);
        }

        return resultado;
    }

    public IEnumerable<Tramite> ListarTodosLosTramites() // podria usar ListarTramitesPorExpediente(guid.Empty) que listaria todos los tramites ya que no hay un expediente con guid.Empty
    {                                                    // pero hacer un metodo que retorne todos los tramites me parece que es mas claro y limpio
        var resultado = new List<Tramite>();

        if (!File.Exists(_nombreArchivo))
            return resultado;

        using var sr = new StreamReader(_nombreArchivo); // se usa para leer los campos del archivo de texto 

        while (!sr.EndOfStream)
        {
            // Leemos exactamente las mismas líneas que escribimos en AgregarTramite
            var idStr                  = sr.ReadLine() ?? "";
            var expedienteIdStr        = sr.ReadLine() ?? "";
            var contenidoStr           = sr.ReadLine() ?? "";
            var fechaCreacionStr       = sr.ReadLine() ?? "";
            var fechaUltModificacionStr = sr.ReadLine() ?? "";
            var usuarioUltimoCambioStr = sr.ReadLine() ?? "";
            var etiquetaStr            = sr.ReadLine() ?? "";

            var id                  = Guid.Parse(idStr);
            var expId               = Guid.Parse(expedienteIdStr);
            var contenido           = new ContenidoTramite(contenidoStr);
            var fechaCreacion       = DateTime.Parse(fechaCreacionStr);
            var fechaUltModificacion = DateTime.Parse(fechaUltModificacionStr);
            var usuarioUltimoCambio = Guid.Parse(usuarioUltimoCambioStr);
            var etiqueta            = Enum.Parse<EtiquetaTramite>(etiquetaStr);

            var tramite = Tramite.ReconstruirTramite(
                id,
                expId,
                contenido,
                fechaCreacion,
                fechaUltModificacion,
                usuarioUltimoCambio,
                etiqueta
            );

            resultado.Add(tramite);
        }

        return resultado;
    }

    public Tramite obtenerTramitePorId(Guid id)
    {
        var tramite = ListarTodosLosTramites().FirstOrDefault(t => t.Id == id); // traigo todos los tramites para luego buscar el que corresponda al id, si no lo encuentra devuelve null
        if (tramite == null)
            throw new RepositoryException($"No se encontró el tramite con Id {id}.");
        return tramite;
    }

    public void ModificarTramite(Tramite tramite)
    {
        // Para modificar, leemos todos, modificamos el que corresponde, y reescribimos todo el archivo.
        var tramites = ListarTodosLosTramites().ToList(); // traigo todos los tramites para luego modificar el que corresponda al id del tramite que me pasan por parametro
        var index = tramites.FindIndex(t => t.Id == tramite.Id);
        if (index == -1)
            throw new RepositoryException($"No se encontró el tramite con Id {tramite.Id} para modificar.");
        tramites[index] = tramite;
        File.Delete(_nombreArchivo);
        foreach (var t in tramites)
            AgregarTramite(t);
    }

    public void TramiteBaja(Guid id)
    {
        // Para eliminar, leemos todos, eliminamos el que corresponde, y reescribimos todo el archivo.
        var tramites = ListarTodosLosTramites().ToList(); // traigo todos los tramites para luego eliminar el que corresponda al id
        var index = tramites.FindIndex(t => t.Id == id);
        if (index == -1)
            throw new RepositoryException($"No se encontró el tramite con Id {id} para eliminar.");
        tramites.RemoveAt(index);
        File.Delete(_nombreArchivo);
        foreach (var t in tramites)
            AgregarTramite(t);
    }

    public void EliminarPorExpedienteId(Guid expedienteId)
    {
        // Para eliminar por expediente, leemos todos, eliminamos los que corresponden, y reescribimos todo el archivo.
        var tramites = ListarTodosLosTramites().ToList(); // traigo todos los tramites para luego eliminar los que correspondan al expedienteId
        tramites.RemoveAll(t => t.ExpedienteId == expedienteId);
        File.Delete(_nombreArchivo);
        foreach (var t in tramites)
            AgregarTramite(t);
    }
}
