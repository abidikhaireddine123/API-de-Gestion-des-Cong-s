using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace ApplicationCore.GenericServices.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }
        // Récupère tous les éléments avec filtrage et tri optionnels
        public async Task<IQueryable<T>> GetAllAsyncwithfilter(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool noTracking = false)
        {
            try
            {
                IQueryable<T> query = _dbSet;

                // Désactive le suivi des entités si noTracking est true
                if (noTracking)
                {
                    query = query.AsNoTracking();
                }

                // Applique le filtre si fourni
                if (filter != null)
                {
                    query = query.Where(filter);
                }

                // Applique le tri si fourni
                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                // Retourne la requête sans l'exécuter (déferred execution)
                return await Task.FromResult(query);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des éléments : {ex.Message}", ex);
            }
        }

        // Récupère un élément par son ID
        public async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'élément avec ID {id} : {ex.Message}", ex);
            }
        }

        // Récupère un élément par ses clés primaires (pour les clés composites)
        public async Task<T?> GetByIdAsync(params object[] keyValues)
        {
            try
            {
                return await _dbSet.FindAsync(keyValues);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'élément avec ID(s) {string.Join(", ", keyValues)} : {ex.Message}", ex);
            }
        }

        // Récupère un élément en fonction d'un prédicat
        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(predicate) ?? throw new Exception("Aucun élément trouvé.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'élément : {ex.Message}", ex);
            }
        }

        // Récupère un élément en fonction d'un prédicat (version synchrone)
        public T Get(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _dbSet.FirstOrDefault(predicate) ?? throw new Exception("Aucun élément trouvé.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'élément : {ex.Message}", ex);
            }
        }

        // Ajoute un nouvel élément
        public async Task AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout de l'élément : {ex.Message}", ex);
            }
        }

        // Ajoute une liste d'éléments
        public async Task AddListAsync(IEnumerable<T> entities)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout de plusieurs éléments : {ex.Message}", ex);
            }
        }

        // Met à jour un élément
        public async Task UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour de l'élément : {ex.Message}", ex);
            }
        }

        // Supprime un élément
        public async Task DeleteAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de l'élément : {ex.Message}", ex);
            }
        }

        // Supprime une liste d'éléments
        public async Task DeleteListAsync(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de plusieurs éléments : {ex.Message}", ex);
            }
        }

        // Enregistre les changements dans la base de données
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'enregistrement des changements : {ex.Message}", ex);
            }
        }
    }
}