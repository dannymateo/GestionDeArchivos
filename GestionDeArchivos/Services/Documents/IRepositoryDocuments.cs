namespace GestionDeArchivos.Services.Documents
{
    public interface IRepositoryDocuments
    {
        Task<bool> UpdateStatus(int id);
    }
}
