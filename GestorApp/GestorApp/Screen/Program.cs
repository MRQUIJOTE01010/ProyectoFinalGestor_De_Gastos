using System;
using System.Linq;
using Spectre.Console;
using GestorGastos.Service;
using GestorGastos.Models;

namespace GestorGastos.Screen
{
    class Program
    {
        private static readonly ExpenseService _expenseService = new ExpenseService();

        static void Main(string[] args)
        {
            AnsiConsole.Write(new FigletText("Gestor de Gastos").Centered().Color(Color.Green));
            MainMenu();
        }

        static void MainMenu()
        {
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecciona una opción:")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Ver gastos",
                        "Agregar gasto",
                        "Editar gasto",
                        "Eliminar gasto",
                        "Ver total de gastos",
                        "Salir"
                    }));

            switch (option)
            {
                case "Ver gastos":
                    ShowExpenses();
                    break;
                case "Agregar gasto":
                    AddExpense();
                    break;
                case "Editar gasto":
                    EditExpense();
                    break;
                case "Eliminar gasto":
                    DeleteExpense();
                    break;
                case "Ver total de gastos":
                    ShowTotal();
                    break;
                case "Salir":
                    Environment.Exit(0);
                    break;
            }

            // Regresar al menú después de la acción
            AnsiConsole.MarkupLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
            MainMenu();
        }

        static void ShowExpenses()
        {
            var expenses = _expenseService.GetAllExpenses().ToList();

            if (!expenses.Any())
            {
                AnsiConsole.MarkupLine("[red]No hay gastos registrados.[/]");
                return;
            }

            // Construir tabla con Spectre.Console
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Fecha");
            table.AddColumn("Categoría");
            table.AddColumn("Descripción");
            table.AddColumn(new TableColumn("Monto").Alignment(Justify.Right));

            foreach (var e in expenses)
            {
                table.AddRow(
                    e.Id.ToString(),
                    e.Date.ToString("dd/MM/yyyy"),
                    e.Category?.Name ?? "Sin categoría",
                    e.Description,
                    e.Amount.ToString("C") // Formato moneda
                );
            }

            AnsiConsole.Write(table);
        }

        static void AddExpense()
        {
            var categories = _expenseService.GetAllCategories().ToList();
            if (!categories.Any())
            {
                AnsiConsole.MarkupLine("[red]No hay categorías disponibles. Contacta al administrador.[/]");
                return;
            }

            var category = AnsiConsole.Prompt(
                new SelectionPrompt<Category>()
                    .Title("Selecciona una [green]categoría[/]:")
                    .UseConverter(c => c.Name)
                    .AddChoices(categories));

            var amount = AnsiConsole.Ask<decimal>("Ingresa el [green]monto[/]:");
            var description = AnsiConsole.Ask<string>("Ingresa una [green]descripción[/]:");
            var date = AnsiConsole.Ask<DateTime>("Ingresa la [green]fecha[/] (dd/MM/yyyy):");

            var expense = new Expense
            {
                Amount = amount,
                Description = description,
                Date = date,
                CategoryId = category.Id
            };

            try
            {
                _expenseService.AddExpense(expense);
                AnsiConsole.MarkupLine("[green]Gasto agregado correctamente.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
        }

        static void EditExpense()
        {
            var expenses = _expenseService.GetAllExpenses().ToList();
            if (!expenses.Any())
            {
                AnsiConsole.MarkupLine("[red]No hay gastos para editar.[/]");
                return;
            }

            var expense = AnsiConsole.Prompt(
                new SelectionPrompt<Expense>()
                    .Title("Selecciona el [yellow]gasto a editar[/]:")
                    .UseConverter(e => $"{e.Id} - {e.Date:dd/MM/yyyy} - {e.Description} - {e.Amount:C}")
                    .AddChoices(expenses));

            // Mostrar datos actuales
            AnsiConsole.MarkupLine($"Editando gasto ID: [yellow]{expense.Id}[/]");
            var newAmount = AnsiConsole.Ask<decimal>($"Nuevo monto (actual: {expense.Amount:C}):", expense.Amount);
            var newDescription = AnsiConsole.Ask<string>($"Nueva descripción (actual: {expense.Description}):", expense.Description);
            var newDate = AnsiConsole.Ask<DateTime>($"Nueva fecha (actual: {expense.Date:dd/MM/yyyy}):", expense.Date);

            // Categorías
            var categories = _expenseService.GetAllCategories().ToList();
            var newCategory = AnsiConsole.Prompt(
                new SelectionPrompt<Category>()
                    .Title("Selecciona una nueva categoría:")
                    .UseConverter(c => c.Name)
                    .AddChoices(categories));

            expense.Amount = newAmount;
            expense.Description = newDescription;
            expense.Date = newDate;
            expense.CategoryId = newCategory.Id;

            try
            {
                _expenseService.UpdateExpense(expense);
                AnsiConsole.MarkupLine("[green]Gasto actualizado correctamente.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
        }

        static void DeleteExpense()
        {
            var expenses = _expenseService.GetAllExpenses().ToList();
            if (!expenses.Any())
            {
                AnsiConsole.MarkupLine("[red]No hay gastos para eliminar.[/]");
                return;
            }

            var expense = AnsiConsole.Prompt(
                new SelectionPrompt<Expense>()
                    .Title("Selecciona el [red]gasto a eliminar[/]:")
                    .UseConverter(e => $"{e.Id} - {e.Date:dd/MM/yyyy} - {e.Description} - {e.Amount:C}")
                    .AddChoices(expenses));

            if (AnsiConsole.Confirm($"¿Estás seguro de eliminar el gasto ID {expense.Id}?"))
            {
                _expenseService.DeleteExpense(expense.Id);
                AnsiConsole.MarkupLine("[green]Gasto eliminado.[/]");
            }
        }

        static void ShowTotal()
        {
            var total = _expenseService.GetTotalExpenses();
            AnsiConsole.MarkupLine($"Total de gastos: [bold yellow]{total:C}[/]");
        }
    }
}

