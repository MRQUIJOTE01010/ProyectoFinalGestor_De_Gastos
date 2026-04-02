using System.Collections.Generic;

namespace GestorGastos.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Relación con gastos
        public ICollection<Expense> Expenses { get; set; }
    }
}

