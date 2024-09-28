using System;
using GogApp.Data;
using GogApp.Interfaces;
using GogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GogApp.Repository;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext context;

    public ReportRepository(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }
    public async Task<bool> Add(Report report)
    {
        context.Add(report);
        return await SaveAsync();
    }

    public async Task<bool> Delete(Report report)
    {
        context.Remove(report);
        return await SaveAsync();
    }

    public async Task<Report> GetReportByIdAsync(int Id)
    {
        return await context.Reports.FirstOrDefaultAsync(i => i.Id == Id);
    }

    public async Task<Report> GetReportByIdAsyncNoTracking(int Id)
    {
        return await context.Reports.AsNoTracking().FirstOrDefaultAsync(i => i.Id == Id);
    }

    public async Task<bool> SaveAsync()
    {
        var saved = context.SaveChanges();
        return saved > 0 ? true : false;
    }

    public async Task<bool> Update(Report report)
    {
        context.Update(report);
        return await SaveAsync();
    }
}
