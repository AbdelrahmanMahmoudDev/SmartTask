using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.ServiceDto;
using SmartTask.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.Services
{
    public class TaskService
    {
        private ITaskRepository _taskRepository;
        private ICommentRepository _commentRepository;
        private IAttachmentRepository _attachmentRepository;
        private ITaskDependencyRepository _taskDependencyRepository;
        public TaskService(ITaskRepository taskRepository, ICommentRepository commentRepository
            , IAttachmentRepository attachmentRepository,ITaskDependencyRepository taskDependencyRepository)
        {
            _taskRepository = taskRepository;
            _commentRepository = commentRepository;
            _attachmentRepository = attachmentRepository;
            _taskDependencyRepository = taskDependencyRepository;
        }
        public async Task<Core.Models.Task> Details(int id)
        {
            var task =await _taskRepository.GetWithDetailsAsync(id);
            return task;
        }
        public async void AddComment(int taskId, string authorId, string content)
        {
            var comment = new Comment
            {
                TaskId = taskId,
                AuthorId = authorId,
                Content = content.Trim(),
                CreatedAt = DateTime.Now
            };
            _commentRepository.AddAsync(comment);
        }
        public async System.Threading.Tasks.Task AddAttachment(int taskId, IFormFile file,string userid,string webRootPath)
        {

            var uploadsFolder = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                TaskId = taskId,
                FileName = file.FileName,
                FilePath = $"/uploads/{uniqueFileName}",
                UploadedById = userid,
                CreatedAt = DateTime.Now
            };

           await _attachmentRepository.AddAsync(attachment);
        }

        public async Task<List<Core.Models.Task>> TasksForUserInProject(int projectId,string userid)
        {
            var tasks=(await _taskRepository.GetByProjectIdAsync(projectId)).Where(t=>t.Assignments.Select(u => u.UserId == userid).FirstOrDefault()).ToList();
            return tasks;
        }
        public async Task<List<Core.Models.Task>> TasksForProject(int projectId)
        {
            var tasks = (await _taskRepository.GetByProjectIdAsync(projectId)).ToList();
            return tasks;
        }
        public async Task<List<Core.Models.Task>> TasksForUser(string userid)
        {
            var tasks = (await _taskRepository.GetByAssignedToIdAsync(userid)).ToList();
            return tasks;
        }
        public async Task<int> NumofDepend(int taskid)
        {
            return (await _taskDependencyRepository.GetByPredecessorIdAsync(taskid))
                .Count();
        }
        public async Task<Core.Models.Task> GetTask(int taskid)
        {
            return (await _taskRepository.GetByIdAsync(taskid));
        }
        public async System.Threading.Tasks.Task DeleteDepend(int taskid)
        {
            var depend = (await _taskDependencyRepository.GetByPredecessorIdAsync(taskid)).ToList();
                await _taskDependencyRepository.DeleteRangeAsync(depend);

            var depend2 = (await _taskDependencyRepository.GetBySuccessorIdAsync(taskid)).ToList();
            await _taskDependencyRepository.DeleteRangeAsync(depend2);

        }
        public async System.Threading.Tasks.Task Delete(int taskid)
        {
            await _taskRepository.DeleteAsync(taskid);
        }

        public async Task<List<TaskDenpendDto>> Loadnodes(int id)
        {
            var graph = new Dictionary<int, List<int>>();
            var visited = new HashSet<int>();
            var task =await _taskRepository.GetByIdAsync(id);
            var allTasks =await _taskRepository.GetByProjectIdAsync(task.ProjectId);

            var taskdepn=(await _taskDependencyRepository.GetAllAsync()).ToList()
                .Where(x => x.Predecessor.ProjectId == task.ProjectId).ToList();

            taskdepn.ForEach(t =>
                {
                    if (!graph.ContainsKey(t.PredecessorId))
                    {
                        graph[t.PredecessorId] = new List<int>();
                    }
                    graph[t.PredecessorId].Add(t.SuccessorId);
                });
            
            visited.DFS(id, graph);

            var notReachable = allTasks.ToList().ExceptBy(visited, e => e.Id);
            var existingDeps =(await _taskDependencyRepository.GetBySuccessorIdAsync(id)).Select(e => e.PredecessorId).ToList();

            var taskViewDeps = notReachable.Select(n => new TaskDenpendDto
            {
                TaskId = n.Id,
                Name = n.Title,
                IsSelected = existingDeps.Contains(n.Id)
            }).ToList();

            return taskViewDeps;
        }

        public async System.Threading.Tasks.Task SaveSelectedTasks(int SelectedTaskId, List<int> selectedTaskIds)
        {
            var existingDependencies =await _taskDependencyRepository.GetBySuccessorIdAsync(SelectedTaskId);

            foreach (var selectedId in selectedTaskIds)
            {
                if (!existingDependencies.Any(td => td.PredecessorId == selectedId))
                {
                    await _taskDependencyRepository.AddAsync(new TaskDependency
                    {
                        SuccessorId = SelectedTaskId,
                        PredecessorId = selectedId
                    });
                }

            }

            foreach (var dependency in existingDependencies)
            {
                if (!selectedTaskIds.Contains(dependency.PredecessorId))
                {
                    await _taskDependencyRepository.DeleteAsync(dependency.PredecessorId);
                }
            }
        }

    }
}
