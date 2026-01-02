namespace BancoChu.Application.Interfaces
{
    public interface IBusinessDayApplication
    {
        Task<bool> IsBusinessDayAsync(DateTime date);
    }
}
