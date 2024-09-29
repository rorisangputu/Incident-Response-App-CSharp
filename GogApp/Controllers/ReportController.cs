using System;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GogApp.Controllers;

public class ReportController : Controller
{
    private readonly IReportRepository reportRepo;

    public ReportController(IReportRepository reportRepository)
    {
        reportRepo = reportRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int projectId, string content, string title)
    {
        // Validate input
        if (string.IsNullOrEmpty(content))
        {
            // Handle error
            TempData["ErrorMessage"] = "Report content cannot be empty.";
            return RedirectToAction("Details", "Project", new { id = projectId });
        }

        var report = new Report
        {
            ProjectId = projectId,
            Title = title,
            Content = content,
            CreatedAt = DateTime.Now
        };

        // Save report to the database using your repository or DbContext
        await reportRepo.Add(report);

        TempData["SuccessMessage"] = "Report added successfully.";
        return RedirectToAction("Detail", "Project", new { id = projectId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var report = await reportRepo.GetReportByIdAsyncNoTracking(id);
        if (report == null)
        {
            return NotFound();
        }

        var editReportViewModel = new EditReportViewModel
        {
            Id = report.Id,

            Content = report.Content,
            CreatedAt = report.CreatedAt,
            ProjectId = report.ProjectId
        };

        return View(editReportViewModel);
    }

    // POST: Report/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditReportViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var report = await reportRepo.GetReportByIdAsyncNoTracking(model.Id);
        if (report == null)
        {
            return NotFound();
        }

        // Update the report fields
        report.Content = model.Content;
        report.CreatedAt = DateTime.Now;  // Optionally update the creation date or use another field

        // Save changes
        await reportRepo.Update(report);

        TempData["SuccessMessage"] = "Report updated successfully.";
        return RedirectToAction("Detail", "Project", new { id = model.ProjectId });
    }

    // GET: Report/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var report = await reportRepo.GetReportByIdAsync(id);
        if (report == null)
        {
            return NotFound();
        }

        return View(report);
    }

    // POST: Report/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var report = await reportRepo.GetReportByIdAsync(id);
        if (report == null)
        {
            return NotFound();
        }

        // Delete the report
        await reportRepo.Delete(report);

        TempData["SuccessMessage"] = "Report deleted successfully.";
        return RedirectToAction("Detail", "Project", new { id = report.ProjectId });
    }

}