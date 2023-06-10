namespace VolopayAssignment.Model
{
    public class Purchase
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string Department { get; set; }
        public string Software { get; set; }
        public int Seats { get; set; }
        public decimal Amount { get; set; }
    }
}
