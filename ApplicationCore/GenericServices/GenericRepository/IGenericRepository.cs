using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApplicationCore.GenericServices.GenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        // Récupère tous les éléments avec filtrage et tri optionnels
        Task<IQueryable<T>> GetAllAsyncwithfilter(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool noTracking = false);

        // Récupère un élément par son ID
        Task<T?> GetByIdAsync(int id);

        // Récupère un élément par ses clés primaires (pour les clés composites)
        Task<T?> GetByIdAsync(params object[] keyValues);

        // Récupère un élément en fonction d'un prédicat
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        // Récupère un élément en fonction d'un prédicat (version synchrone)
        T Get(Expression<Func<T, bool>> predicate);

        // Ajoute un nouvel élément
        Task AddAsync(T entity);

        // Ajoute une liste d'éléments
        Task AddListAsync(IEnumerable<T> entities);

        // Met à jour un élément
        Task UpdateAsync(T entity);

        // Supprime un élément
        Task DeleteAsync(T entity);

        // Supprime une liste d'éléments
        Task DeleteListAsync(IEnumerable<T> entities);

        // Enregistre les changements dans la base de données
        Task SaveChangesAsync();
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    }
}