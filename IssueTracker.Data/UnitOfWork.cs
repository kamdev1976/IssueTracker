using IssueTracker.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace IssueTracker.Data
{
    /// <summary>
    /// Implements the Unit of Work pattern to manage repository instances and commit transactions 
    /// within a single HTTP request lifecycle using an injected Entity Framework DbContext.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PrimaryIssueEntities _context;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The concrete PrimaryIssueEntities instance injected by DI.</param>
        public UnitOfWork(PrimaryIssueEntities context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves or creates a generic repository instance for the specified entity type.
        /// </summary>
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new Repository<TEntity>(_context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<TEntity>)_repositories[type];
        }

        /// <summary>
        /// Saves all pending changes to the database in a single atomic operation.
        /// </summary>
        public int Complete()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
         .SelectMany(x => x.ValidationErrors)
         .Select(x => $"{x.PropertyName}: {x.ErrorMessage}");

                var fullErrorMessage = string.Join("; ", errorMessages);

                // Use a specific validation exception type instead of generic Exception
                throw new System.ComponentModel.DataAnnotations.ValidationException($"Validation failed: {fullErrorMessage}", ex);
            }
        }

        /// <summary>
        /// Releases all resources used by the underlying DbContext.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}