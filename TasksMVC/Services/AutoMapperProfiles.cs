using AutoMapper;
using TasksMVC.Entities;
using TasksMVC.Models;

namespace TasksMVC.Services
{
	public class AutoMapperProfiles: Profile
	{
		public AutoMapperProfiles() 
		{
			CreateMap<Tarea, TaskDTO>();
		}
	}
}
