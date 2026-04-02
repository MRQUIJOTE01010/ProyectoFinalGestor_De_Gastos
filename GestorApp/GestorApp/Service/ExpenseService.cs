using System;
using System.Collections.Generic;
using GestorGastos.Models;
using GestorGastos.Repository;

namespace GestorGastos.Service
{
    public class ExpenseService
    {
        private readonly IExpenseRepository _repository;

        public ExpenseService()
        {
            _repository = new ExpenseRepository();
        }

        public IEnumerable<Expense> GetAllExpenses()
        {
            return _repository.GetAll();
        }

        public Expense GetExpenseById(int id)
        {
            return _repository.GetById(id);
        }

        public void AddExpense(Expense expense)
        {
            // Validaciones básicas
            if (expense.Amount <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.");
            if (string.IsNullOrWhiteSpace(expense.Description))
                throw new ArgumentException("La descripción es obligatoria.");
            if (expense.Date == default)
                throw new ArgumentException("Debe especificar una fecha válida.");

            _repository.Add(expense);
        }

        public void UpdateExpense(Expense expense)
        {
            // Validaciones
            if (expense.Amount <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.");
            if (string.IsNullOrWhiteSpace(expense.Description))
                throw new ArgumentException("La descripción es obligatoria.");

            _repository.Update(expense);
        }

        public void DeleteExpense(int id)
        {
            _repository.Delete(id);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _repository.GetCategories();
        }

        public decimal GetTotalExpenses()
        {
            return _repository.GetAll().Sum(e => e.Amount);
        }
    }
}
