using System;
using System.Collections.Generic;
using GestorGastos.Models;

namespace GestorGastos.Repository
{
    public interface IExpenseRepository
    {
        IEnumerable<Expense> GetAll();
        Expense GetById(int id);
        void Add(Expense expense);
        void Update(Expense expense);
        void Delete(int id);
        IEnumerable<Category> GetCategories();
    }
}

