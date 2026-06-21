
using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public class Tramite
{
    //utilizo encapsulamiento (private set),Propiedades con set privado para proteger el encapsulamiento
    public Guid Id { get; private set; }
    public Guid ExpedienteId { get; private set; }
    public EtiquetaTramite Etiqueta { get; private set; }
    public ContenidoTramite Contenido{ get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltModificacion { get; private set; }
    public Guid UsuarioUltModificacion { get; private set; }

    //1.Constructor Privado (Centraliza lógica y asigna ID),Se usa tanto para la creación nueva como para la reconstrucción.
    private Tramite(Guid id ,Guid idExp, ContenidoTramite contT, DateTime fechaCreacion, DateTime fechaUltModificacion, Guid usuarioUltCambio, EtiquetaTramite etiqueta)
    {
        if (id == Guid.Empty)
            throw new DominioException("El Id del tramite no puede ser un Guid vacío.");
        if (idExp == Guid.Empty)
            throw new DominioException("El Id del expediente no puede ser un Guid vacío.");
        Id = id;
        ExpedienteId = idExp;
        Contenido = contT ?? throw new DominioException("La caratula es obligatoria");
        FechaCreacion = fechaCreacion;
        FechaUltModificacion = fechaUltModificacion;
        Etiqueta = etiqueta;
        UsuarioUltModificacion = usuarioUltCambio;
    }


    //2.Constructor Público (NUEVOS: Autogenera ID y delega) , No pide el Id, lo autogenera para garantizar autonomía.

    public Tramite(Guid idExp , ContenidoTramite conT):this(Guid.NewGuid(),idExp, conT, DateTime.Now, DateTime.Now, Guid.Empty, EtiquetaTramite.EscritoPresentado){}

    // 3. Factory Method (Para mecanismos de persistencia),Permite "hidratar"(para no perder información, en algún momento querremos recuperarla.) un objeto con datos que vienen de la persistencia.
    public static Tramite ReconstruirTramite(Guid id ,Guid idExp, ContenidoTramite conT, DateTime fechaCreacion, DateTime fechaUltModificacion, Guid usuarioUltCambio, EtiquetaTramite etiqueta)
    {
        return new Tramite(id, idExp, conT, fechaCreacion, fechaUltModificacion, usuarioUltCambio, etiqueta);
    }

    public void ActualizarUsuarioModificacion(Guid usuario)
    {
        UsuarioUltModificacion = usuario; 
    }

    public void ModificarTramite(EtiquetaTramite etiqueta, ContenidoTramite contenido, Guid usuario)
    {
        Etiqueta = etiqueta;
        Contenido = contenido;
        FechaUltModificacion = DateTime.Now;
        UsuarioUltModificacion = usuario;
    }

    public override string ToString()
    {
        return $"Id de expediente: {ExpedienteId}, id de tramite: {Id}, tipo de tramite(etiqueta): {Etiqueta}, contenido del tramite: {Contenido}  ";
    }
}
