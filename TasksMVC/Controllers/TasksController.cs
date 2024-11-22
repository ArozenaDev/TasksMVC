using AutoMapper;
using AutoMapper.QueryableExtensions;
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
		private readonly IMapper mapper;

		public TasksController(ApplicationDbContext context, IUserService userService, IMapper mapper) 
		{
			this.context = context;
			this.userService = userService;
			this.mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<List<TaskDTO>>> Get()
		{
			var userId = userService.GetUserId();
			var tasks = await context.Tasks
				.Where(t => t.CreatorId == userId)
				.OrderBy(t => t.Order)
				.ProjectTo<TaskDTO>(mapper.ConfigurationProvider)
				.ToListAsync();

			return tasks;
		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<Tarea>> Get(int id)
		{
			var userId = userService.GetUserId();

			var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.CreatorId == userId);

			if (task is null)
			{
				return NotFound();
			}

			return task;
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

		[HttpPut("{id:int}")]
		public async Task<IActionResult> EditTask(int id, [FromBody] EditTaskDTO editTaskDTO)
		{
			var userId = userService.GetUserId();

			var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.CreatorId == userId);

			if (task is null)
			{
				return NotFound();
			}

			task.Name = editTaskDTO.Title;
			task.Description = editTaskDTO.Description;

			await context.SaveChangesAsync();

			return Ok();
		}

		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id)
		{
			var userId = userService.GetUserId();

			var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.CreatorId == userId);

			if (task is null)
			{
				return NotFound();
			}

			context.Remove(task);
			await context.SaveChangesAsync();
			return Ok();
		}

		[HttpPost("ordenar")]
		public async Task<IActionResult> Order([FromBody] int[] ids)
		{
			var userId = userService.GetUserId();

			var tasks = await context.Tasks
				.Where(t => t.CreatorId == userId).ToListAsync();

			var tasksId = tasks.Select(t => t.Id);

			var notBelonginIds = ids.Except(tasksId).ToList();

			if (notBelonginIds.Any())
			{
				return Forbid();
			}

			var tasksDictionary = tasks.ToDictionary(x => x.Id);

			for (int i = 0; i < ids.Length; i++)
			{
				var id = ids[i];
				var task = tasksDictionary[id];
				task.Order = i + 1;
			}

		await context.SaveChangesAsync();

			return Ok();
		}
	}
}
