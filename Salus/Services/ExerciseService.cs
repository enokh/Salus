using Microsoft.EntityFrameworkCore;
using Salus.Data;
using Salus.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salus.Services
{
    public class ExerciseService
    {
        private readonly SalusDbContext _db;

        public ExerciseService(SalusDbContext db)
        {
            _db = db;
        }

        public async Task<List<Exercise>> GetExercisesAsync(int profileId)
            => await _db.Exercises.Where(e => e.ProfileId == profileId).OrderBy(e => e.Name).ToListAsync();

        public async Task<Exercise> AddExerciseAsync(int profileId, string name)
        {
            var exercise = new Exercise { ProfileId = profileId, Name = name };
            _db.Exercises.Add(exercise);
            await _db.SaveChangesAsync();
            return exercise;
        }

        public async Task RenameExerciseAsync(int id, string newName)
        {
            var exercise = await _db.Exercises.FindAsync(id);
            if (exercise != null)
            {
                exercise.Name = newName;
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteExerciseAsync(int id)
        {
            var exercise = await _db.Exercises.FindAsync(id);
            if (exercise != null)
            {
                _db.Exercises.Remove(exercise);
                await _db.SaveChangesAsync();
            }
        }
    }
}
