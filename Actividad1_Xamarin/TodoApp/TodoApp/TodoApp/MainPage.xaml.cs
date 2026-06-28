using System;
using System.IO;
using SQLite;
using TodoApp.Models;
using Xamarin.Forms;

namespace TodoApp
{
    public partial class MainPage : ContentPage
    {
        SQLiteConnection _db;

        public MainPage()
        {
            InitializeComponent();
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "tareas.db3");
            _db = new SQLiteConnection(path);
            _db.CreateTable<TaskItem>();
            RefreshList();
        }

        void RefreshList()
        {
            TaskList.ItemsSource = _db.Table<TaskItem>().ToList();
        }

        void OnAddClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                DisplayAlert("Error", "El nombre no puede estar vacío.", "OK");
                return;
            }

            _db.Insert(new TaskItem
            {
                Name = NameEntry.Text.Trim(),
                Description = DescEntry.Text?.Trim()
            });

            NameEntry.Text = "";
            DescEntry.Text = "";
            RefreshList();
        }

        async void OnTaskSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is TaskItem task)
            {
                TaskList.SelectedItem = null;
                bool eliminar = await DisplayAlert(
                    task.Name, task.Description, "Eliminar", "Cancelar");
                if (eliminar)
                {
                    _db.Delete(task);
                    RefreshList();
                }
            }
        }
    }
}