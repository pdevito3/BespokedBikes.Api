namespace Application.Interfaces.Salesperson
{
    using Application.Dtos.Salesperson;
    using Application.Wrappers;
    using System.Threading.Tasks;
    using Domain.Entities;

    public interface ISalespersonRepository
    {
        PagedList <Salesperson> GetSalespersons(SalespersonParametersDto SalespersonParameters);
        Task<Salesperson> GetSalespersonAsync(int SalespersonId);
        Salesperson GetSalesperson(int SalespersonId);
        void AddSalesperson(Salesperson salesperson);
        void DeleteSalesperson(Salesperson salesperson);
        void UpdateSalesperson(Salesperson salesperson);
        bool Save();
    }
}