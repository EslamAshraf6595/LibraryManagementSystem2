namespace LibraryManagementSystem2
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public bool AvailabilityStatus { get; set; }
        public int NumberOfCopies { get; set; }

        public Book() { }

        public Book(int bookID, string title, string author, string genre, string isbn, bool availability, int copies)
        {
            BookID = bookID;
            Title = title;
            Author = author;
            Genre = genre;
            ISBN = isbn;
            AvailabilityStatus = availability;
            NumberOfCopies = copies;
        }
    }
}
