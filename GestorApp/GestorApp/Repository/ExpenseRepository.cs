using System;
using System.Collections.Generic;
using System.Linq;
using GestorGastos.Database;
using GestorGastos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorGastos.Repository

{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _context;

        public ExpenseRepository()
        {
            _context = new AppDbContext();
            // Crea la base de datos si no existe
            _context.Database.EnsureCreated();
        }

        public IEnumerable<Expense> GetAll()
        {
            return _context.Expenses
                .Include(e => e.Category)
                .OrderByDescending(e => e.Date)
                .ToList();
        }

        public Expense GetById(int id)
        {
            return _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefault(e => e.Id == id);
        }

        public void Add(Expense expense)
        {
            _context.Expenses.Add(expense);
            _context.SaveChanges();
        }

        public void Update(Expense expense)
        {
            _context.Entry(expense).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var expense = _context.Expenses.Find(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
    }
}
