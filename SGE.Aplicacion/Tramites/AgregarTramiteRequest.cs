using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;
// Sirve para que el servidor entienda qué quiere el usuario y con qué datos.
public record class AgregarTramiteRequest
(
    Guid ExpId,
    Guid UsuarioUltModificacion,
    ContenidoTramite ContenidoTramite
);
