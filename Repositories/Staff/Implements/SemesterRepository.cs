using DataTranferObjects.Staff.Semester;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Implements
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly FpttrackingSystemContext _context;
        public SemesterRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<Semester?> findActive()
        {
            return await _context.Semesters.FirstOrDefaultAsync(x => x.IsActive == true);
        }

        public async Task<List<Semester>> getAllSemesters()
        {
            return await _context.Semesters
                 .Include(s => s.SemesterWeeks)
                 .Include(s => s.SemesterVacations)
                 .OrderByDescending(x => x.StartAt)
                 .ToListAsync();
        }

        public async Task<Semester?> GetDeliveriesBySemester(int id)
        {
            return await _context.Semesters
                .Include(s => s.Deliverables)
                .ThenInclude(d => d.Milestone)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<Semester?> GetGroupsBySemester(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Semester?> GetMilestonesBySemester(int id)
        {
            return await _context.Semesters
                .Include(s => s.Deliverables)
                    .ThenInclude(d => d.Milestone)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Semester?> GetSemesterByIdAsync(int id)
        {
            return await _context.Semesters
                .Include(s => s.SemesterWeeks)
                .Include(s => s.SemesterVacations)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Semester?> GetSemesterByNow()
        {
            return await _context.Semesters.FirstOrDefaultAsync(s => s.IsActive == true);
        }

        public async Task<bool> AddVacationsAsync(List<SemesterVacationRequestDto> vacations)
        {
            var entities = vacations.Select(v => new SemesterVacation
            {
                SemesterId = v.SemesterId,
                StartAt = v.StartDate,
                EndAt = v.EndDate,
                Description = v.Description
            }).ToList();

            await _context.SemesterVacations.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<Semester?> FindByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return await _context.Semesters
                .FirstOrDefaultAsync(s => s.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public async Task<bool> UpdateVacationAsync(int id, SemesterUpdateVacationRequestDto dto)
        {
            var vacation = await _context.SemesterVacations.FirstOrDefaultAsync(v => v.Id == id);
            if (vacation == null)
                return false;

            vacation.SemesterId = id;
            vacation.StartAt = dto.StartDate;
            vacation.EndAt = dto.EndDate;
            vacation.Description = dto.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SemesterInfoDto>> GetSemestersBySupervisorAsync(int supervisorUserId)
        {
            var semesters = await _context.GroupUsers
                .Where(gu => gu.UserId == supervisorUserId && gu.Role == "Supervisor")
                .Select(gu => gu.Group!.Semester)
                .Distinct()
                .Select(s => new SemesterInfoDto
                {
                    Name = s.Name,
                    IsActive = s.IsActive,
                    Description = s.Description,
                    StartAt = s.StartAt,
                    EndAt = s.EndAt
                })
                .ToListAsync();

            return semesters;
        }

        public async Task<List<SemesterVacationDto>> GetBySemesterIdAsync(int semesterId)
        {
            var vacations = await _context.SemesterVacations
                .Where(v => v.SemesterId == semesterId)
                .OrderBy(v => v.StartAt)
                .Select(v => new SemesterVacationDto
                {
                    id = v.Id,
                    StartDate = v.StartAt ?? DateTime.MinValue,
                    EndDate = v.EndAt ?? DateTime.MinValue,
                    Description = v.Description
                })
                .ToListAsync();

            return vacations;
        }

        public async Task<List<SemesterVacation>> GetVacationsBySemesterAsync(int semesterId)
        {
            return await _context.SemesterVacations
                .Where(v => v.SemesterId == semesterId)
                .OrderBy(v => v.StartAt)
                .ToListAsync();
        }

    }
}
