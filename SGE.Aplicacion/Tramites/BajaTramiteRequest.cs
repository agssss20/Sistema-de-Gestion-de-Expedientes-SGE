
namespace SGE.Aplicacion.Tramites;
// Sirve para que el servidor entienda qué quiere el usuario y con qué datos.
public record class BajaTramiteRequest (
    Guid IdExp,
    Guid UsuarioUltModificacion,
    Guid IdTramite
);

