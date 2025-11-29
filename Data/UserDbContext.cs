using Korvan_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Korvan_API.Data
{
	public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
	{
		public DbSet<User> Users { get; set; }
	}
}
