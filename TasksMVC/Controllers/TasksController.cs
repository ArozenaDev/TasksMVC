using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksMVC.Entities;
using TasksMVC.Models;
using TasksMVC.Services;

namespace TasksMVC.Controllers
{
	[Route("api/tasks")]
	public class TasksController: ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly IUserService userService;

		public TasksController(ApplicationDbContext context, IUserService userService) 
		{
			this.context = context;
			this.userService = userService;
		}

		[HttpGet]
		public async Task<List<TaskDTO>> Get()
		{
			var userId = userService.GetUserId();
			var tasks = await context.Tasks
				.Where(t => t.CreatorId == userId)
				.OrderBy(t => t.Order)
				.Select(t => new TaskDTO 
				{
					Id = t.Id, 
					Name = t.Name
				})
				.ToListAsync();

			return tasks;
		}

		[HttpPost]
		public async Task<ActionResult<Tarea>> Post([FromBody] string title)
		{
			var userId = userService.GetUserId();

			var existingTasks = await context.Tasks.AnyAsync(t => t.CreatorId == userId);

			var order = 0;
			if (existingTasks)
			{
				order = await context.Tasks.Where(t => t.CreatorId == userId).Select(t => t.Order).MaxAsync();
			}

			var task = new Tarea
			{
				Name = title,
				CreatorId = userId,
				Date = DateTime.UtcNow,
				Order = order + 1
			};

			context.Add(task);
			await context.SaveChangesAsync();

			return task;
		}
	}
}
