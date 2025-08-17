namespace library_management_system
{
    public class BookModel
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public override string ToString()
        {
            return $"{Title} by {Author} ({Genre}) - {Quantity} available";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (BookModel)obj;
            return Title == other.Title &&
                   Author == other.Author &&
                   Genre == other.Genre &&
                   Quantity == other.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Author, Genre, Quantity);
        }
    }
}