﻿namespace TicTacToe.Data.Units
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using TicTacToe.Data.Models;
    using TicTacToe.Data.Repositories;

    public class UnitSystem : IUnitSystem
    {
        private readonly DbContext context;
        private readonly IDictionary<Type, object> respositories;

        public UnitSystem(DbContext ticTacToeContext)
        {
            this.context = ticTacToeContext;
            this.respositories = new Dictionary<Type, object>();
        }

        public IRepository<User> Users
        {
            get 
            {
                return this.GetRepository<User>();
            }
        }

        public IRepository<Game> Games
        {
            get
            {
                return this.GetRepository<Game>();
            }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this.respositories.ContainsKey(typeof(T)))
            {
                Type type = typeof(GenericRepository<T>);

                this.respositories.Add(typeof(T), Activator.CreateInstance(type, this.context));
            }

            return (IRepository<T>)this.respositories[typeof(T)];
        }
    }
}
