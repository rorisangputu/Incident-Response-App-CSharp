using System;
using GogApp.Models;

namespace GogApp.Interfaces;

public interface IReportRepository
{
    Task<Report> GetReportByIdAsync(int Id);
    Task<Report> GetReportByIdAsyncNoTracking(int Id);
    Task<bool> Add(Report report);
    Task<bool> Delete(Report report);
    Task<bool> Update(Report report);
    Task<bool> SaveAsync();
}
