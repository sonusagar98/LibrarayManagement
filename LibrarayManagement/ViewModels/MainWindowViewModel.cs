using GalaSoft.MvvmLight.Command;
using LibrarayManagement.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Input;

namespace LibrarayManagement.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<Book> Books { get; set; }

        public ObservableCollection<Book> DisplayedBooks { get; set; }
        public ICommand AddNewBookCommand { get; set; }

        string IsFreezed = @"C:\Users\sonu1\source\repos\LibrarayManagement\freezed.txt";

        public string BookTitle { get; set; }

        public string Author { get; set; }
        public string Publication { get; set; }

        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _searchedText = "";
        public string SearchedText
        {
            get { return _searchedText; }
            set { SetProperty(ref _searchedText, value); }
        }

        private string _selectedBookIndex;
        public string SelectedBookIndex
        {
            get { return _selectedBookIndex; }
            set
            {
                SetProperty(ref _selectedBookIndex, value);
                DeleteBook.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand<string> text;
        public DelegateCommand<string> DisplaySearchBookResult =>
            text ?? (text = new DelegateCommand<string>(ExecuteDisplaySearchBookResult, CanExecuteDisplaySearchBookResult));


        void DisplayBooks(string parameter)
        {
            string text = parameter;
            DisplayedBooks.Clear();
            for (int i = 0; i < Books.Count; i++)
            {
                Book currentBook = Books[i];
                if (currentBook.Title.Contains(text) || currentBook.Author.Contains(text) || currentBook.Publication.Contains(text))
                {
                    DisplayedBooks.Add(currentBook);
                }
            }

        }
        void ExecuteDisplaySearchBookResult(string parameter)
        {
            DisplayBooks(parameter);
        }

        bool CanExecuteDisplaySearchBookResult(string parameter)
        {
            return true;
        }


        private DelegateCommand<object> _deleteBook;
        public DelegateCommand<object> DeleteBook =>
            _deleteBook ?? (_deleteBook = new DelegateCommand<object>(ExecuteCommandName, CanExecuteCommandName));

        void ExecuteCommandName(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            int index = Convert.ToInt32(parameter);
            int removeIndexInDisplayBooks = index;
            if (index < 0)
            {
                return;
            }


            for (int i = 0; i < Books.Count; i++)
            {
                if (DisplayedBooks.Contains(Books[i]))
                {
                    index -= 1;
                    if (index == -1)
                    {
                        Books.RemoveAt(i);
                    }
                }
                if (index < 0)
                    break;
            }
            DisplayedBooks.RemoveAt(removeIndexInDisplayBooks);
            DisplayBooks(SearchedText);
        }

        bool CanExecuteCommandName(object parameter)
        {
            return parameter != null;
        }

        private DelegateCommand _makeUnresponsive;
        public DelegateCommand MakeUnresponsive =>
            _makeUnresponsive ?? (_makeUnresponsive = new DelegateCommand(ExecuteMakeUnresponsive));

        void ExecuteMakeUnresponsive()
        {


            try
            {
                File.Create(IsFreezed).Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception WatchdogMonitor4: " + ex.StackTrace);
            }

            while (true) ;

        }

        public MainWindowViewModel()
        {
            Books = new ObservableCollection<Book>();
            DisplayedBooks = new ObservableCollection<Book>();
            AddNewBookCommand = new RelayCommand(() => {
                this.Books.Add(new Book(this.BookTitle, this.Author, this.Publication));
                DisplayBooks(SearchedText);
            });
            LoadSomeBooks();
            string EmptyString = "";
            DisplayBooks(EmptyString);

            if (File.Exists(IsFreezed))
            {
                File.Delete(IsFreezed);
            }

            Thread heatbeatThread = new Thread(new ThreadStart(StartHeartbeatThread));
            heatbeatThread.IsBackground = true;
            heatbeatThread.Start();
        }

        private void StartHeartbeatThread()
        {
            string HeartBeatFile = @"C:\Users\sonu1\source\repos\LibrarayManagement\heartbeat.txt";

            while (!File.Exists(IsFreezed))
            {
                try
                {
                    File.Create(HeartBeatFile).Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception WatchdogMonitor4: " + ex.StackTrace);
                }
                Thread.Sleep(1000);
            }


        }

        private void LoadSomeBooks()
        {
            Book newBook1 = new Book("atoms", "james", "pearson");
            Book newBook2 = new Book("happy", "haiidn", "marsh");
            Book newBook3 = new Book("program", "hacker", "fun");
            Book newBook4 = new Book("hello", "suaain", "greet");
            Book newBook5 = new Book("book", "author", "pub");
            Books.Add(newBook1);
            Books.Add(newBook2);
            Books.Add(newBook3);
            Books.Add(newBook4);
            Books.Add(newBook5);
        }
    }
}
