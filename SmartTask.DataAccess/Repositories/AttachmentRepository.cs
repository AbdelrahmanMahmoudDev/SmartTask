using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.DataAccess.Data;
using Attachment = SmartTask.Core.Models.Attachment;


namespace SmartTask.DataAccess.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly SmartTaskContext _context;

        public AttachmentRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Attachment>> GetAllAsync()
        {
            return await _context.Attachments
                .Include(a => a.Task) // استخدام الخاصية الحقيقية (ليس alias)
                .Include(a => a.UploadedBy)
                .ToListAsync();
        }

        public async Task<Attachment> GetByIdAsync(int id)
        {
            return await _context.Attachments
                .Include(a => a.Task)
                .Include(a => a.UploadedBy)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Attachment>> GetByTaskIdAsync(int taskId)
        {
            return await _context.Attachments
                .Include(a => a.UploadedBy)
                .Where(a => a.TaskId == taskId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attachment>> GetByUploadedByIdAsync(string uploadedById)
        {
            return await _context.Attachments
                .Include(a => a.Task)
                .Where(a => a.UploadedById == uploadedById)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Attachment> AddAsync(Attachment attachment)
        {
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task UpdateAsync(Attachment attachment)
        {
            _context.Entry(attachment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment != null)
            {
                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync();
            }
            await Task.CompletedTask; 
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Attachments.AnyAsync(a => a.Id == id);
        }
    }
}